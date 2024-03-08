// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Configuration.Snmp.Rsu;

public class ConfigurationConsumer : ScalingConsumer<Guid, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>>, IConfigurationConsumer
{
    public ConfigurationConsumer(
        IBuildMessagingConfig<Guid, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>> buildMessagingConfig,
        IOptions<ScalingConsumerOptions<Guid, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>>> options,
        IConsumeResultFactory<Guid, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>> consumeResultFactory,
        ILogger<Consumer<Guid, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>>> logger)
        : base(buildMessagingConfig, options, consumeResultFactory, logger)
    {
    }
}