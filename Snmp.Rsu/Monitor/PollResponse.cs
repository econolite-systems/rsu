// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Lextm.SharpSnmpLib;

namespace Econolite.Ode.Snmp.Rsu;

public class PollResponse : IPollResponse
{
    public SnmpDevice Device { get; set; } = SnmpDevice.Empty;
    public string Error { get; set; } = string.Empty;
    public IList<Variable> Variables { get; set; } = new List<Variable>();
}

public interface IPollResponse
{
    SnmpDevice Device { get; set; }
    string Error { get; set; }
    IList<Variable> Variables { get; set; }
}
