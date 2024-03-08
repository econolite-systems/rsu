// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using Econolite.Ode.Models.Rsu.Status;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;

namespace Econolite.Ode.Snmp.Rsu;

public class Device : IPoll
{
    protected readonly SnmpDevice _device;
    protected readonly ISnmpServer _server;
    protected readonly IPollingService _pollingService;
    protected readonly IPollProducer<IPollResponse> _producer;

    public Device(SnmpDevice device, ISnmpServer server, IPollingService pollingService, IPollProducer<IPollResponse> producer)
    {
        _device = device;
        _server = server;
        _pollingService = pollingService;
        _producer = producer;
        _pollingService.RegisterPoll(this);
    }

    public Guid Id => _device.DeviceId;

    public virtual async Task Poll()
    {
        throw new NotImplementedException();
    }

    protected static async Task<List<Variable>> GetDataAsync(ISnmpServer server, SnmpDevice device, IList<Variable> vList)
    {
        using var cts = new CancellationTokenSource(device.Timeout);
        var chunks = vList.Chunk(device.MaxVariables);
        var result = new List<Variable>();
        foreach (var vChunkList in chunks)
        {
            var snmpResults = await server.Get(device, vChunkList, cts.Token);
            result.AddRange(snmpResults);
        }

        return result;
    }
    
    protected static async Task<List<Variable>> SetDataAsync(ISnmpServer server, SnmpDevice device, IList<Variable> vList)
    {
        using var cts = new CancellationTokenSource(device.Timeout);
        var chunks = vList.Chunk(device.MaxVariables);
        var result = new List<Variable>();
        foreach (var vChunkList in chunks)
        {
            var snmpResults = await server.Set(device, vChunkList, cts.Token);
            result.AddRange(snmpResults);
        }

        return result;
    }
}