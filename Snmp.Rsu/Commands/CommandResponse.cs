// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Snmp.Rsu.Commands;

public interface ICommandResponse
{
    SnmpDevice Device { get; set; }
    string Error { get; set; }
}

public class CommandResponse : ICommandResponse
{
    public SnmpDevice Device { get; set; } = SnmpDevice.Empty;
    public string Error { get; set; } = string.Empty;
}
