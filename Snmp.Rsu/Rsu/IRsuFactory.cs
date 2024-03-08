// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Snmp.Rsu;

public interface IRsuFactory
{
    RsuDevice Create(Models.Rsu.Configuration.Rsu rsu);
}