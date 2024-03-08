// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.Extensions;
using Econolite.Ode.Status.Rsu;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Domain.Rsu;

public class RsuStatusProducer : IRsuStatusProducer
{
    private readonly IBuildMessagingConfig _buildMessagingConfig;
    private readonly IOptions<ProducerOptions<Guid, string>> _producerOptions;
    private readonly ILoggerFactory _loggerFactory;
    private readonly RsuStatusProducerOptions _configOptions;
    private readonly Confluent.Kafka.IProducer<string, string> _producer;

    public RsuStatusProducer(IBuildMessagingConfig buildMessagingConfig, IOptions<RsuStatusProducerOptions> configOptions, IOptions<ProducerOptions<Guid, string>> producerOptions, ILoggerFactory loggerFactory)
    {
        _producer = new ProducerBuilder<string, string>(
                buildMessagingConfig.BuildProducerClientConfig(producerOptions.Value))
            .AddLogging(loggerFactory.CreateLogger(GetType().Name))
            .Build();
        _buildMessagingConfig = buildMessagingConfig;
        _producerOptions = producerOptions;
        _loggerFactory = loggerFactory;
        _configOptions = configOptions.Value;
    }
    
    public async Task ProduceAsync(Guid id, RsuSystemStats stats, CancellationToken cancel)
    {
        var headers = new Headers
        {
            { "tenantId", Encoding.ASCII.GetBytes(id.ToString()) }
        };
        var json = 
            await _producer.ProduceAsync(_configOptions.RsuStatusTopic, new Confluent.Kafka.Message<string, string>
            {
                Key = id.ToString(),
                Value = JsonSerializer.Serialize(stats, JsonPayloadSerializerOptions.Options),
                Headers = headers
            }, cancel);
    }
}

public class RsuStatusProducerOptions
{
    public string RsuStatusTopic { get; set; } = "rsu.status";
}