// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Data.SqlTypes;
using System.Net;
using System.Net.Sockets;
using Econolite.Ode.Models.Rsu.Configuration;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Snmp.Rsu;

public class SnmpServer : ISnmpServer
{
    private readonly ILogger _logger;

    public SnmpServer(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(GetType().Name);
    }

    public async Task<IList<Variable>> Get(SnmpDevice device, IList<Variable> vList, CancellationToken token)
    {
        var getRequestMessage = (VersionCode version, int nextMessageId, int nextRequestId, string user, string contextName,
                IList<Variable> variables, IPrivacyProvider privacyProvider, int maxMessageSize, ISnmpMessage message) => 
            new GetRequestMessage(version, nextMessageId, nextRequestId,
                new OctetString(user),
                new OctetString(contextName),
                variables, privacyProvider, maxMessageSize, message) as ISnmpMessage;
        
        var result = await HandleSnmp(device, vList, SnmpType.GetRequestPdu, getRequestMessage, token);
        return result;
    }
    
    public async Task<IList<Variable>> Set(SnmpDevice device, IList<Variable> vList, CancellationToken token)
    {
        var setRequestMessage = (VersionCode version, int nextMessageId, int nextRequestId, string user, string contextName,
            IList<Variable> variables, IPrivacyProvider privacyProvider, int maxMessageSize, ISnmpMessage message) => 
            new SetRequestMessage(version, nextMessageId, nextRequestId,
            new OctetString(user),
            new OctetString(contextName),
            variables, privacyProvider, maxMessageSize, message) as ISnmpMessage;
        
        var result = await HandleSnmp(device, vList, SnmpType.SetRequestPdu, setRequestMessage, token);
        return result;
    }
    
    public async Task<IList<Variable>> HandleSnmp(SnmpDevice device, IList<Variable> vList, SnmpType snmpType,
        Func<VersionCode, int, int, string, string, IList<Variable>, IPrivacyProvider, int, ISnmpMessage, ISnmpMessage>
            getSetRequestMessage, CancellationToken token)
    {
        try
        {
            var ip = await GetIpAddress(device.Target);
            if (Equals(ip, IPAddress.None))
            {
                throw OperationException.Create($"{device.Target} not found", ip);
            }

            var receiver = new IPEndPoint(ip, 161);

            if (device.SnmpVersion != SnmpVersion.V3)
            {
                var result = await Messenger.SetAsync(device.SnmpVersion == SnmpVersion.V1 ? VersionCode.V1 : VersionCode.V2, receiver, new OctetString(device.Community),
                    vList, token);
                return result;
            }

            if (string.IsNullOrEmpty(device.User))
            {
                _logger.LogError("User name need to be specified for v3");
                throw OperationException.Create("user name need to be specified", ip);
            }

            IAuthenticationProvider auth = device.Authentication != AuthenticationProviderType.None && device.AuthPhrase != null
                ? GetAuthenticationProviderByName(device.Authentication, device.AuthPhrase)
                : DefaultAuthenticationProvider.Instance;

            IPrivacyProvider priv = device.Privacy != PrivacyProviderType.None && device.PrivPhrase != null
                ? GetPrivacyProviderByName(device.Privacy, device.PrivPhrase, auth)
                : new DefaultPrivacyProvider(auth);

            Discovery discovery = Messenger.GetNextDiscovery(snmpType);
            ReportMessage report = await discovery.GetResponseAsync(receiver, token);

            ISnmpMessage request = getSetRequestMessage(VersionCode.V3, Messenger.NextMessageId,
                Messenger.NextRequestId, device.User,
                string.IsNullOrWhiteSpace(device.ContextName) ? string.Empty : device.ContextName,
                vList, priv, Messenger.MaxMessageSize, report);
            ISnmpMessage reply = await request.GetResponseAsync(receiver, token);

            if (reply is ReportMessage)
            {
                if (reply.Pdu().Variables.Count == 0)
                {
                    return reply.Pdu().Variables;
                }

                var id = reply.Pdu().Variables[0].Id;
                if (id != Messenger.NotInTimeWindow)
                {
                    var error = id.GetErrorMessage();
                    throw ErrorException.Create(
                        error,
                        receiver.Address,
                        reply);
                }

                // according to RFC 3414, send a second request to sync time.
                request = getSetRequestMessage(VersionCode.V3, Messenger.NextMessageId, Messenger.NextRequestId,
                    device.User,
                    string.IsNullOrWhiteSpace(device.ContextName) ? string.Empty : device.ContextName,
                    vList, priv, Messenger.MaxMessageSize, reply);
                reply = await request.GetResponseAsync(receiver, token);
            }
            else if (reply.Pdu().ErrorStatus.ToInt32() != 0) // != ErrorCode.NoError
            {
                throw ErrorException.Create(
                    "error in response",
                    receiver.Address,
                    reply);
            }

            return reply.Pdu().Variables;
        }
        catch (SnmpException e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
        catch (SocketException e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    private static async Task<IPAddress> GetIpAddress(string target)
    {
        bool parsed = IPAddress.TryParse(target, out var ip);
        if (!parsed)
        {
            var addresses = await Dns.GetHostAddressesAsync(target);

            foreach (IPAddress address in
                     addresses.Where(address => address.AddressFamily == AddressFamily.InterNetwork))
            {
                ip = address;
                break;
            }
        }

        return ip ?? IPAddress.None;
    }

    private static IAuthenticationProvider GetAuthenticationProviderByName(AuthenticationProviderType authentication, string phrase)
    {
        if (authentication == AuthenticationProviderType.MD5)
        {
            return new MD5AuthenticationProvider(new OctetString(phrase));
        }

        if (authentication == AuthenticationProviderType.SHA)
        {
            return new SHA1AuthenticationProvider(new OctetString(phrase));
        }

        if (authentication == AuthenticationProviderType.SHA256)
        {
            return new SHA256AuthenticationProvider(new OctetString(phrase));
        }

        if (authentication == AuthenticationProviderType.SHA384)
        {
            return new SHA384AuthenticationProvider(new OctetString(phrase));
        }

        if (authentication == AuthenticationProviderType.SHA512)
        {
            return new SHA512AuthenticationProvider(new OctetString(phrase));
        }

        throw new ArgumentException("unknown name", nameof(authentication));
    }

    private static IPrivacyProvider GetPrivacyProviderByName(PrivacyProviderType privacy, string phrase,
        IAuthenticationProvider auth)
    {
        if (privacy == PrivacyProviderType.DES)
        {
            return new DESPrivacyProvider(new OctetString(phrase), auth);
        }

        if (privacy == PrivacyProviderType.AES)
        {
            return new AESPrivacyProvider(new OctetString(phrase), auth);
        }

        if (privacy == PrivacyProviderType.AES192)
        {
            return new AES192PrivacyProvider(new OctetString(phrase), auth);
        }

        if (privacy == PrivacyProviderType.AES256)
        {
            return new AES256PrivacyProvider(new OctetString(phrase), auth);
        }

        throw new ArgumentException("unknown name", nameof(privacy));
    }
}