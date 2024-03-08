// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Configuration.Snmp.Rsu;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Microsoft.Extensions.Options;

namespace Service.Rsu.Management;

public class ConfigurationRequest : BackgroundService
{
    private readonly IConfigurationRequestProducer _configurationRequestProducer;
    private readonly ConfigurationRequestOptions _options;
    private readonly ILogger<ConfigurationRequest> _logger;
    private readonly UserEventFactory _userEventFactory;
    private readonly IMetricsCounter _loopCounter;

    public ConfigurationRequest(IConfigurationRequestProducer configurationRequestProducer, IOptions<ConfigurationRequestOptions> options, IMetricsFactory metricsFactory, UserEventFactory userEventFactory, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ConfigurationRequest>();
        _configurationRequestProducer = configurationRequestProducer;
        _options = options?.Value ?? new ConfigurationRequestOptions();
        _userEventFactory = userEventFactory;

        _loopCounter = metricsFactory.GetMetricsCounter("Requests");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Sending config request to {@}", _options);
            await _configurationRequestProducer.ProduceAsync(Guid.Empty, stoppingToken);
            _logger.LogInformation("Config request sent");

            _loopCounter.Increment();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to send config request");

            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Error, "Failed to send config request"));
        }
    }
}