// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Collections.Concurrent;
using Econolite.Ode.Models.Rsu.Status;
using Econolite.Ode.Status.Rsu;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.Rsu;

public class RsuService : IRsuService
{
    private readonly IRsuStatusProducer _producer;
    private readonly ILogger _logger;

    private IDictionary<Guid, IEnumerable<StoreRepeatMessage>> RsuStatus { get; set; } =
        new ConcurrentDictionary<Guid, IEnumerable<StoreRepeatMessage>>();
    
    public RsuService(IRsuStatusProducer producer, ILoggerFactory loggerFactory)
    {
        _producer = producer;
        _logger = loggerFactory.CreateLogger(GetType().Name);
    }

    public void UpdateStatus(Guid id, RsuSystemStats status)
    {
        _logger.LogInformation("Add status for {Id}", id.ToString());
        _producer.ProduceAsync(id, status, CancellationToken.None);
    }
    
}