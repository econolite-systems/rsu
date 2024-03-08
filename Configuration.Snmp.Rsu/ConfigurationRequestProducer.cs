// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Text;
using Confluent.Kafka;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Configuration.Snmp.Rsu;

public class ConfigurationRequestProducer : IConfigurationRequestProducer
{
    private readonly IBuildMessagingConfig _buildMessagingConfig;
    private readonly ConfigurationRequestOptions _configRequestOptions;
    private readonly IOptions<ProducerOptions<Guid, string>> _producerOptions;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Confluent.Kafka.IProducer<string, string> _producer;

    public ConfigurationRequestProducer(IBuildMessagingConfig buildMessagingConfig, IOptions<ConfigurationRequestOptions> configRequestOptions, IOptions<ProducerOptions<Guid, string>> producerOptions, ILoggerFactory loggerFactory)
    {
        _producer = new ProducerBuilder<string, string>(
            buildMessagingConfig.BuildProducerClientConfig(producerOptions.Value))
            .AddLogging(loggerFactory.CreateLogger(GetType().Name))
            .Build();
        _buildMessagingConfig = buildMessagingConfig;
        _configRequestOptions = configRequestOptions.Value;
        _producerOptions = producerOptions;
        _loggerFactory = loggerFactory;
    }
    
    public async Task ProduceAsync(Guid id, CancellationToken cancel)
    {
        var headers = new Headers
        {
            { "tenantId", Encoding.ASCII.GetBytes(id.ToString()) }
        };
        await _producer.ProduceAsync(_configRequestOptions.ConfigRequestTopic, new Message<string, string>
        {
            Key = id.ToString(),
            Value = "RequestConfig",
            Headers = headers
        }, cancel);
    }
}

public class ConfigurationRequestOptions
{
    public string ConfigRequestTopic { get; set; } = "config.request";
}