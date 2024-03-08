// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Lextm.SharpSnmpLib;

namespace Econolite.Ode.Snmp.Rsu;

public class PollRequest : IPollRequest
{
    public SnmpDevice Device { get; set; } = SnmpDevice.Empty;
    
    public Guid RequestId { get; set; } = Guid.Empty;

    public IList<Variable> Variables { get; set; } = new List<Variable>();
}