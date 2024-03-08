// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Messaging;

namespace Configuration.Snmp.Rsu;

public interface IConfigurationRequestConsumer : IScalingConsumer<Guid, string>
{
}