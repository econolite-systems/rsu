// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Models.Rsu.Configuration;

public enum AuthenticationProviderType
{
    None,
    MD5,
    SHA,
    SHA256,
    SHA384,
    SHA512
}

public static class AuthenticationProviderTypeExtension
{
    public static string ToSnmpString(this AuthenticationProviderType provider)
    {
        return provider switch
        {
            AuthenticationProviderType.None => "None",
            AuthenticationProviderType.MD5 => "MD5",
            AuthenticationProviderType.SHA => "SHA",
            AuthenticationProviderType.SHA256 => "SHA256",
            AuthenticationProviderType.SHA384 => "SHA384",
            AuthenticationProviderType.SHA512 => "SHA512",
            _ => "None"
        };
    }
    
    public static AuthenticationProviderType ToAuthenticationProviderType(this string provider)
    {
        return provider switch
        {
            "None" => AuthenticationProviderType.None,
            "MD5" => AuthenticationProviderType.MD5,
            "SHA" => AuthenticationProviderType.SHA,
            "SHA256" => AuthenticationProviderType.SHA256,
            "SHA384" => AuthenticationProviderType.SHA384,
            "SHA512" => AuthenticationProviderType.SHA512,
            _ => AuthenticationProviderType.None
        };
    }
}