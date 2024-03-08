// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Models.Rsu.Status;

namespace Econolite.Ode.Domain.Rsu;

public interface IRsuStatusService
{
    Task<IEnumerable<RsuStatusDto>> Find(List<Guid> deviceId, DateTime startDate, DateTime? endDate);
}