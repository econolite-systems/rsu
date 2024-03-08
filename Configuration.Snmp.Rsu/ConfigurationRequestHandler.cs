// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Configuration.Snmp.Rsu.Extensions;
using Econolite.Ode.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Configuration.Snmp.Rsu;

public class ConfigurationRequestHandler : BackgroundService
{
    private readonly IEntityService _entityService;
    private readonly IConfigurationProducer _configurationProducer;
    private readonly IConfigurationRequestConsumer _configurationRequestConsumer;
    private readonly ILogger _logger;
    private readonly string _topic;
    private readonly Guid _tenantId;

    public ConfigurationRequestHandler(IServiceProvider serviceProvider, IConfigurationProducer configurationProducer, IConfigurationRequestConsumer configurationRequestConsumer, IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(GetType().Name);
        _entityService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IEntityService>();
        _configurationProducer = configurationProducer;
        _configurationRequestConsumer = configurationRequestConsumer;
        _topic = configuration["Topics:ConfigRequest"] ?? "Unknown";
        _tenantId = Guid.Parse(configuration["TenantId"] ?? "Unknown");
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting the main loop");
        try
        {
          _logger.LogInformation("Consuming config response from topic: {@}", _topic);
          await _configurationRequestConsumer.ConsumeOn(_topic, async data =>
          {
              var rsuNodes = await _entityService.GetNodesByTypeAsync("Rsu");
              var rsus = rsuNodes.ToRsu();
              await _configurationProducer.ProduceAsync(_tenantId, rsus, cancellationToken);
          }, cancellationToken);
        }
        finally
        {
          _logger.LogInformation("Ending the main loop");
        }
    }
}