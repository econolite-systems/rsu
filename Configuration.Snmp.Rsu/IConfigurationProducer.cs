// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Configuration.Snmp.Rsu;

public interface IConfigurationProducer
{
    Task ProduceAsync(Guid id, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu> rsus, CancellationToken cancel);
}