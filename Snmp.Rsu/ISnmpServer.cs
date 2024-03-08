// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Lextm.SharpSnmpLib;

namespace Econolite.Ode.Snmp.Rsu;

public interface ISnmpServer
{
    Task<IList<Variable>> Get(SnmpDevice device, IList<Variable> vList, CancellationToken token);
    Task<IList<Variable>> Set(SnmpDevice device, IList<Variable> vList, CancellationToken token);
}
