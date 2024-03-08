// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Persistence.Common.Repository;

namespace Econolite.Ode.Repository.Rsu;

public interface IRsuRepository : IRepository<Models.Rsu.Configuration.Rsu, Guid>
{
}