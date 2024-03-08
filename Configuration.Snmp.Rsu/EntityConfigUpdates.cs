// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Configuration.Snmp.Rsu.Extensions;
using Econolite.Ode.Domain.Configuration;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Entities.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Configuration.Snmp.Rsu;

public class EntityConfigUpdate : IEntityConfigUpdate
{
    private readonly IConfigurationProducer _configurationProducer;
    private readonly Guid _tenantId;

    public EntityConfigUpdate(IServiceProvider serviceProvider, IConfigurationProducer configurationProducer, IConfiguration configuration)
    {
        _configurationProducer = configurationProducer;
        _tenantId = Guid.Parse(configuration["TenantId"] ?? "Unknown");
    }
    
    public async Task Add(IEntityService service, EntityNode entity)
    {
        await SendRsuConfiguration(service);
    }

    public async Task Update(IEntityService service, EntityNode entity)
    {
        await SendRsuConfiguration(service);
    }

    public async Task Delete(IEntityService service, EntityNode entity)
    {
        await SendRsuConfiguration(service);
    }
    
    private async Task SendRsuConfiguration(IEntityService service)
    {
        var rsuNodes = await service.GetNodesByTypeAsync("Rsu");
        var rsus = rsuNodes.ToRsu();
        await _configurationProducer.ProduceAsync(_tenantId, rsus, CancellationToken.None);
    }
}