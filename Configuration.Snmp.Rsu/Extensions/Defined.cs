// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.Extensions;
using Econolite.Ode.Models.Entities.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Configuration.Snmp.Rsu.Extensions;

public static class Defined
{
    public static IServiceCollection AddConfigRequestProducer(this IServiceCollection services) => services.AddConfigRequestProducer(_ => { });

    public static IServiceCollection AddConfigRequestProducer(this IServiceCollection services, Action<ConfigurationRequestOptions> action) => services
        .AddMessaging()
        .Configure<ConfigurationRequestOptions>(action)
        .AddTransient<IBuildMessagingConfig<Guid, string>, BuildMessagingConfig<Guid, string>>()
        .AddTransient<IConfigurationRequestProducer, ConfigurationRequestProducer>();

    public static IServiceCollection AddConfigRequestConsumers(this IServiceCollection services, IConfiguration configuration) => services
        .Configure<KafkaConfigOptions<Guid, string>>(configuration.GetSection("Kafka"))
        .AddTransient<IBuildMessagingConfig<Guid, string>, BuildMessagingConfig<Guid, string>>()
        .AddTransient<IPayloadSpecialist<string>, JsonPayloadSpecialist<string>>()
        .AddTransient<IConsumeResultFactory<Guid, string>, ConsumeResultFactory<string>>()
        .AddTransient<IConfigurationRequestConsumer, ConfigurationRequestConsumer>();

    public static IServiceCollection AddConfigProducer(this IServiceCollection services,
        Action<ConfigurationOptions> action) => services
        .AddMessaging()
        .Configure<ConfigurationOptions>(action)
        .AddTransient<IBuildMessagingConfig<Guid, string>, BuildMessagingConfig<Guid, string>>()
        .AddTransient<IConfigurationProducer, ConfigurationProducer>()
        .AddTransient<IEntityConfigUpdate, EntityConfigUpdate>();
    
    public static IServiceCollection AddConfigConsumers(this IServiceCollection services, IConfiguration configuration) => services
        .Configure<KafkaConfigOptions<Guid, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>>>(configuration.GetSection("Kafka"))
        .AddTransient<IBuildMessagingConfig<Guid, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>>, BuildMessagingConfig<Guid, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>>>()
        .AddTransient<IPayloadSpecialist<IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>>, JsonPayloadSpecialist<IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>>>()
        .AddTransient<IConsumeResultFactory<Guid, IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>>, ConsumeResultFactory<IEnumerable<Econolite.Ode.Models.Rsu.Configuration.Rsu>>>()
        .AddTransient<IConfigurationConsumer, ConfigurationConsumer>();

    public static IServiceCollection AddRsuConfigHandler(this IServiceCollection services) => services.AddHostedService<ConfigurationRequestHandler>();
}