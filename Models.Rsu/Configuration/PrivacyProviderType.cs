// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Models.Rsu.Configuration;

public enum PrivacyProviderType
{
    None,
    DES,
    AES,
    AES192,
    AES256
}

public static class PrivacyProviderTypeExtensions
{
    public static string ToString(this PrivacyProviderType provider)
    {
        return provider switch
        {
            PrivacyProviderType.None => "None",
            PrivacyProviderType.DES => "DES",
            PrivacyProviderType.AES => "AES",
            PrivacyProviderType.AES192 => "AES192",
            PrivacyProviderType.AES256 => "AES256",
            _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
        };
    }
    
    public static PrivacyProviderType ToPrivacyProviderType(this string provider)
    {
        return provider switch
        {
            "None" => PrivacyProviderType.None,
            "DES" => PrivacyProviderType.DES,
            "AES" => PrivacyProviderType.AES,
            "AES192" => PrivacyProviderType.AES192,
            "AES256" => PrivacyProviderType.AES256,
            _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
        };
    }
}