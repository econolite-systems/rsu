// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Configuration.Rsu;

public interface IRsuService
{
    Task<IEnumerable<Models.Rsu.Configuration.Rsu>> GetAllAsync();
    Task<Models.Rsu.Configuration.Rsu?> GetByIdAsync(Guid id);
    Task<Models.Rsu.Configuration.Rsu?> Add(Models.Rsu.Configuration.RsuAdd add);
    Task<Models.Rsu.Configuration.Rsu?> Update(Models.Rsu.Configuration.Rsu update);
    Task<bool> Delete(Guid id);
}