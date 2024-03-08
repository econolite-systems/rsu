// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Rsu.Configuration;

namespace Configuration.Snmp.Rsu.Extensions;

public static class Mapping
{
    public static IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu> ToRsu(this IEnumerable<EntityNode> nodes)
        => nodes.Select(node => node.ToRsu());
    
    private static Econolite.Ode.Models.Rsu.Configuration.Rsu ToRsu(this EntityNode node) => new()
    {
        Id = node.Id,
        Target = node.IPAddress,
        Username = node.Username,
        Password = node.Password,
        PrivacyPhrase = node.PrivacyPhrase,
        Retries = node.Retries ?? 0,
        Timeout = node.Timeout ?? 10000,
        PollRate = node.PollRate ?? 60,
        Authentication = node.Authentication!.ToAuthenticationProviderType(),
        Privacy = node.Privacy!.ToPrivacyProviderType(),
        RequireStandbyOnSet = node.RequireStandbyOnSet ?? false,
    };
    
}