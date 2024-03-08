// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Lextm.SharpSnmpLib;

namespace Econolite.Ode.Snmp.Rsu;

public interface IPollRequest
{
    SnmpDevice Device { get; set; }
    IList<Variable> Variables { get; set; }
}