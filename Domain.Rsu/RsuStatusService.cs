// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Domain.Configuration;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Rsu.Status;
using Econolite.Ode.Repository.Rsu;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.Rsu;

public class RsuStatusService : IRsuStatusService
{
    private readonly IEntityService _entityService;
    private readonly IRsuStatusRepository _rsuStatusRepo;
    private readonly ILogger<RsuStatusService> _logger;

    public RsuStatusService(IEntityService entityService, IRsuStatusRepository rsuStatusRepo, ILogger<RsuStatusService> logger)
    {
        _rsuStatusRepo = rsuStatusRepo;
        _entityService = entityService;
        _logger = logger;
    }

    public async Task<IEnumerable<RsuStatusDto>> Find(List<Guid> deviceId, DateTime startDate, DateTime? endDate)
    {
        var entityNames = await FindNames();
        var result = await _rsuStatusRepo.Find(deviceId, startDate, endDate);
        return result.Select(r => r.AdaptStatusDocumentWithListToDto(entityNames));
    }

    private async Task<IEnumerable<EntityNode>> FindNames()
    {
        var sensor = await _entityService.GetNodesByTypeAsync("Rsu");
        return sensor;
    }
}