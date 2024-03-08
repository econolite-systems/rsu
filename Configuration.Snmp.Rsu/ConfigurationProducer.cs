// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Configuration.Snmp.Rsu;

public class ConfigurationProducer : IConfigurationProducer
{
    private readonly IBuildMessagingConfig _buildMessagingConfig;
    private readonly ConfigurationOptions _configOptions;
    private readonly IOptions<ProducerOptions<Guid, string>> _producerOptions;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Confluent.Kafka.IProducer<string, string> _producer;

    public ConfigurationProducer(IBuildMessagingConfig buildMessagingConfig, IOptions<ConfigurationOptions> configOptions, IOptions<ProducerOptions<Guid, string>> producerOptions, ILoggerFactory loggerFactory)
    {
        _producer = new ProducerBuilder<string, string>(
                buildMessagingConfig.BuildProducerClientConfig(producerOptions.Value))
            .AddLogging(loggerFactory.CreateLogger(GetType().Name))
            .Build();
        _buildMessagingConfig = buildMessagingConfig;
        _configOptions = configOptions.Value;
        _producerOptions = producerOptions;
        _loggerFactory = loggerFactory;
    }
    
    public async Task ProduceAsync(Guid id, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu> rsus, CancellationToken cancel)
    {
        var headers = new Headers
        {
            { "tenantId", Encoding.ASCII.GetBytes(id.ToString()) }
        };
        var json = 
        await _producer.ProduceAsync(_configOptions.ConfigResponseTopic, new Confluent.Kafka.Message<string, string>
        {
            Key = id.ToString(),
            Value = JsonSerializer.Serialize(rsus, JsonPayloadSerializerOptions.Options),
            Headers = headers
        }, cancel);
    }
}

public class ConfigurationOptions
{
    public string ConfigResponseTopic { get; set; } = "config.response";
}