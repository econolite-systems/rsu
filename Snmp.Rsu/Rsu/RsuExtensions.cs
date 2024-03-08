// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Snmp.Rsu;

public static class RsuExtensions
{
    public static SnmpDevice ToSnmpDevice(this Models.Rsu.Configuration.Rsu rsu)
    {
        return new SnmpDevice()
        {
            Authentication = rsu.Authentication,
            AuthPhrase = rsu.Password,
            Community = rsu.Community,
            ContextName = rsu.ContextName,
            DeviceId = rsu.Id,
            MaxVariables = rsu.MaxVariables,
            PollRate = rsu.PollRate,
            Privacy = rsu.Privacy,
            PrivPhrase = rsu.PrivacyPhrase,
            RequireStandbyModeOnSet = rsu.RequireStandbyOnSet,
            SnmpVersion = rsu.SnmpVersion,
            User = rsu.Username,
            Target = rsu.Target,
            Timeout = rsu.Timeout * 1000,
        };
    }
}