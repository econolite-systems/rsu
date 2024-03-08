// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Snmp.Rsu;

public class RsuFactory : IRsuFactory
{
    private readonly ISnmpServer _snmpServer;
    private readonly IPollingService _pollingService;
    private readonly IPollProducer<IPollResponse> _pollProducer;
    private readonly ILoggerFactory _loggerFactory;

    public RsuFactory(ISnmpServer snmpServer, IPollingService pollingService, IPollProducer<IPollResponse> pollProducer, ILoggerFactory loggerFactory)
    {
        _snmpServer = snmpServer;
        _pollingService = pollingService;
        _pollProducer = pollProducer;
        _loggerFactory = loggerFactory;
    }
    
    public RsuDevice Create(Models.Rsu.Configuration.Rsu rsu)
    {
        return new RsuDevice(rsu.ToSnmpDevice(), _snmpServer, _pollingService, _pollProducer, _loggerFactory.CreateLogger<RsuDevice>());;
    }
}