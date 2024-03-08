// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Messaging;

namespace Configuration.Snmp.Rsu;

public interface IConfigurationConsumer : IScalingConsumer<Guid, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>>
{
}