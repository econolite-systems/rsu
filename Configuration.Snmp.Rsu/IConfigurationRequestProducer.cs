// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Configuration.Snmp.Rsu;

public interface IConfigurationRequestProducer
{
    Task ProduceAsync(Guid id, CancellationToken cancel);
}