// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Messaging;

namespace Econolite.Ode.Domain.Rsu;

public interface IRsuStatusConsumer : IScalingConsumer<Guid, string>
{
    
}