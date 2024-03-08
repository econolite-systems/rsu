// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Repository.Rsu;

public class RsuRepository : GuidDocumentRepositoryBase<Models.Rsu.Configuration.Rsu>, IRsuRepository
{
    public RsuRepository(IMongoContext context, ILogger<RsuRepository> logger) : base(context, logger)
    {
    }
}