// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Status.Rsu;

namespace Econolite.Ode.Domain.Rsu;

public interface IRsuStatusProducer
{
    Task ProduceAsync(Guid id, RsuSystemStats stats, CancellationToken cancel);
}