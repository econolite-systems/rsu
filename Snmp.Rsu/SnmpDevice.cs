// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Models.Rsu.Configuration;
//using Lextm.SharpSnmpLib;

namespace Econolite.Ode.Snmp.Rsu;

public class SnmpDevice
{
    public Guid DeviceId { get; set; } = Guid.Empty;
    public string Target { get; set; } = string.Empty;
    public string Community { get; set; } = "public";
    public SnmpVersion SnmpVersion { get; set; } = SnmpVersion.V1;
    public int Timeout { get; set; } = 1000;
    public int Retry { get; set; } = 0;
    public int MaxVariables { get; set; } = 6;
    public int PollRate { get; set; } = 60;
    public string User { get; set; } = string.Empty;
    public string ContextName { get; set; } = string.Empty;
    public AuthenticationProviderType Authentication { get; set; } = AuthenticationProviderType.None;
    public string AuthPhrase { get; set; } = string.Empty;
    public PrivacyProviderType Privacy { get; set; } = PrivacyProviderType.None;
    public string PrivPhrase { get; set; } = string.Empty;
    public bool RequireStandbyModeOnSet { get; set; } = false;

    public static SnmpDevice Empty { get; } = new SnmpDevice();
}