// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Models.Rsu.Status;
using Econolite.Ode.Persistence.Common.Repository;

namespace Econolite.Ode.Repository.Rsu;

public interface IRsuStatusRepository : IRepository<RsuStatus, Guid>
{
    /// <summary>
    ///     Finds Road Side Unit statuses that match a given list of device IDs with a timestamp between the
    ///     given start and end dates. If no device IDs are given, returns all statuses with a timestamp between the
    ///     start and end dates for any device. If the end date is not given, returns all statuses with a timestamp
    ///     after the given start date for the given device IDs.
    /// </summary>
    /// <param name="deviceIds">List of device IDs to filter by or empty list to filter none</param>
    /// <param name="startDate">Mandatory start date for filtering status entries</param>
    /// <param name="endDate">Optional end date for filtering status entries</param>
    /// <returns>A list of rsu status objects</returns>
    Task<List<RsuStatus>> Find(List<Guid> deviceIds, DateTime startDate, DateTime? endDate);

}