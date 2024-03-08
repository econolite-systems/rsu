// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Models.Rsu.Configuration;

public class RsuAdd : SnmpDevice
{ }

public class SnmpDevice
{
    public string Target { get; set; } = string.Empty;
    public SnmpVersion SnmpVersion { get; set; } = SnmpVersion.V3;
    public string Community { get; set; } = "public";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? PrivacyPhrase { get; set; }
    public string ContextName { get; set; } = string.Empty;
    public int Retries { get; set; } = 0;
    public int Timeout { get; set; } = 10000;
    public int PollRate { get; set; } = 60;
    public int MaxVariables { get; set; } = 6;
    public AuthenticationProviderType Authentication { get; set; } = AuthenticationProviderType.None;
    public PrivacyProviderType Privacy { get; set; } = PrivacyProviderType.None;
    public bool RequireStandbyOnSet { get; set; } = false;
}