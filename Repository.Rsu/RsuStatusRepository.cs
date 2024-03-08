// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Models.Rsu.Status;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Econolite.Ode.Repository.Rsu;

public class RsuStatusRepository : GuidDocumentRepositoryBase<RsuStatus>, IRsuStatusRepository
{
    public RsuStatusRepository(IMongoContext context, ILogger<RsuStatusRepository> logger) : base(context, logger)
    {
    }

    public async Task<List<RsuStatus>> Find(List<Guid> deviceIds, DateTime startDate, DateTime? endDate)
    {
        var end = endDate ?? DateTime.UtcNow;

        return deviceIds.Any()
            ? await ExecuteDbSetFuncAsync(collection => collection.Find(w => deviceIds.Contains(w.DeviceId) && w.TimeStamp >= startDate && w.TimeStamp < end).SortByDescending(w => w.TimeStamp).ToListAsync())
            : await ExecuteDbSetFuncAsync(collection => collection.Find(w => w.TimeStamp >= startDate && w.TimeStamp < end).SortByDescending(w => w.TimeStamp).ToListAsync());
    }
}