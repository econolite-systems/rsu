// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Net;
using System.Security.Cryptography;
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Models.Rsu.Configuration;

public class Rsu : RsuAdd, IIndexedEntity<Guid>
{
    public Rsu()
    {
        Authentication = AuthenticationProviderType.SHA;
        Privacy = PrivacyProviderType.AES;
        SnmpVersion = SnmpVersion.V3;
        RequireStandbyOnSet = true;
    }

    public Guid Id { get; set; } = Guid.Empty;
}
