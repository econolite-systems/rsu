// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Status.Rsu;

namespace Econolite.Ode.Domain.Rsu;

public interface IRsuService
{
    public void UpdateStatus(Guid id, RsuSystemStats status);
}