// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Configuration.Snmp.Rsu;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Econolite.Ode.Snmp.Rsu;

namespace Service.Rsu.Management;

public class ConfigurationResponseHandler : BackgroundService
{
    private readonly IConfigurationConsumer _configurationConsumer;
    private readonly ISnmpService _snmpService;
    private readonly ILogger _logger;
    private readonly string _topic;
    private readonly Guid _tenantId;
    private readonly UserEventFactory _userEventFactory;
    private readonly IMetricsCounter _loopCounter;

    public ConfigurationResponseHandler(IConfigurationConsumer configurationConsumer, IConfiguration configuration, ILoggerFactory loggerFactory, ISnmpService snmpService, IMetricsFactory metricsFactory, UserEventFactory userEventFactory)
    {
        _logger = loggerFactory.CreateLogger(GetType().Name);
        _configurationConsumer = configurationConsumer;
        _snmpService = snmpService;
        _topic = configuration["Topics:ConfigResponse"] ?? "Unknown";
        _tenantId = Guid.Parse(configuration["Tenant:Id"] ?? "Unknown");
        _userEventFactory = userEventFactory;

        _loopCounter = metricsFactory.GetMetricsCounter("Responses");
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting the main loop");
        try
        {
            _logger.LogInformation("Consuming config response from topic: {@}", _topic);
            await _configurationConsumer.ConsumeOn(_topic, async data =>
            {
                _logger.LogInformation("Received Configuration");
                await _snmpService.UpdateConfigAsync(data.Value);
                _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Updated configuration: {0}", data.Value?.Count())));
                _loopCounter.Increment();
            }, stoppingToken);
        }
        finally
        {
            _logger.LogInformation("Ending the main loop");
        }
    }
}