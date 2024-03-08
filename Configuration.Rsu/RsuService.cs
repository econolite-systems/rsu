// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Configuration.Snmp.Rsu;
using Econolite.Ode.Repository.Rsu;

namespace Econolite.Ode.Configuration.Rsu;

public class RsuService : IRsuService
{
    private readonly IRsuRepository _rsuRepository;
    private readonly IConfigurationProducer _configurationProducer;

    public RsuService(IConfigurationProducer configurationProducer, IRsuRepository rsuRepository)
    {
        _rsuRepository = rsuRepository;
        _configurationProducer = configurationProducer;
    }

    public async Task<IEnumerable<Models.Rsu.Configuration.Rsu>> GetAllAsync()
    {
        return await _rsuRepository.GetAllAsync();
    }

    public async Task<Models.Rsu.Configuration.Rsu?> GetByIdAsync(Guid id)
    {
        return await _rsuRepository.GetByIdAsync(id);
    }

    public async Task<Models.Rsu.Configuration.Rsu?> Add(Models.Rsu.Configuration.RsuAdd add)
    {
        var rsu = (Models.Rsu.Configuration.Rsu)add;
        rsu.Id = new Guid();
        _rsuRepository.Add(rsu);
        var (success, _) = await _rsuRepository.DbContext.SaveChangesAsync();
        if (success)
        {
            await _configurationProducer.ProduceAsync(Guid.Empty, await GetAllAsync(), new CancellationToken());
        }
        return rsu;
    }

    public async Task<Models.Rsu.Configuration.Rsu?> Update(Models.Rsu.Configuration.Rsu update)
    {
        _rsuRepository.Update(update);
        var (success, _) = await _rsuRepository.DbContext.SaveChangesAsync();
        if (success)
        {
            await _configurationProducer.ProduceAsync(Guid.Empty, await GetAllAsync(), new CancellationToken());
        }
        return update;
    }

    public async Task<bool> Delete(Guid id)
    {
        _rsuRepository.Remove(id);
        var (success, _) = await _rsuRepository.DbContext.SaveChangesAsync();
        if (success)
        {
            await _configurationProducer.ProduceAsync(Guid.Empty, await GetAllAsync(), new CancellationToken());
        }
        return success;
    }
}