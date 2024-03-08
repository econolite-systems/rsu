// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Models.Rsu.Configuration;

public enum SnmpVersion
{
        //
    // Summary:
    //     SNMP v1.
    V1,
    //
    // Summary:
    //     SNMP v2.
    V2,
    //
    // Summary:
    //     SNMP v3.
    V3
}

public static class SnmpVersionExtensions
{
    public static string ToSnmpString(this SnmpVersion version)
    {
        return version switch
        {
            SnmpVersion.V1 => "V1",
            SnmpVersion.V2 => "V2",
            SnmpVersion.V3 => "V3",
            _ => throw new ArgumentOutOfRangeException(nameof(version), version, null)
        };
    }
    
    public static SnmpVersion ToSnmpVersion(this string version)
    {
        return version switch
        {
            "V1" => SnmpVersion.V1,
            "V2" => SnmpVersion.V2,
            "V3" => SnmpVersion.V3,
            _ => throw new ArgumentOutOfRangeException(nameof(version), version, null)
        };
    }
}