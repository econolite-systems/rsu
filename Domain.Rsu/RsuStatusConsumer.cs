// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Domain.Rsu;

public class RsuStatusConsumer : ScalingConsumer<Guid, string>, IRsuStatusConsumer
{
    public RsuStatusConsumer(
        IBuildMessagingConfig<Guid, string> buildMessagingConfig,
        IOptions<ScalingConsumerOptions<Guid, string>> options,
        IConsumeResultFactory<Guid, string> consumeResultFactory,
        ILogger<Consumer<Guid, string>> logger)
        : base(buildMessagingConfig, options, consumeResultFactory, logger)
    {
    }
}