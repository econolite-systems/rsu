// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Domain.Rsu;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Snmp.Rsu;

public class PollProducer : IPollProducer<IPollResponse>
{
    private readonly IRsuService _rsuService;
    private readonly ILogger _logger;

    public PollProducer(IRsuService rsuService, ILoggerFactory loggerFactory)
    {
        _rsuService = rsuService;
        _logger = loggerFactory.CreateLogger(GetType().Name);
    }
    
    public async Task Produce(IPollResponse item)
    {
        _logger.LogInformation(item.Device.User);
        _rsuService.UpdateStatus(item.Device.DeviceId,item.ToRsuSystemStats());
    }
}