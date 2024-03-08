// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Configuration.Snmp.Rsu;

public class ConfigurationRequestConsumer : ScalingConsumer<Guid, string>, IConfigurationRequestConsumer
{
    public ConfigurationRequestConsumer(
        IBuildMessagingConfig<Guid, string> buildMessagingConfig,
        IOptions<ScalingConsumerOptions<Guid, string>> options,
        IConsumeResultFactory<Guid, string> consumeResultFactory,
        ILogger<Consumer<Guid, string>> logger)
        : base(buildMessagingConfig, options, consumeResultFactory, logger)
    {
    }
}