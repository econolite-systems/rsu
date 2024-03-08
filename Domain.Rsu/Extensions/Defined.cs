// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Domain.Rsu.Extensions;

public static class Defined
{
    public static IServiceCollection AddRsuService(this IServiceCollection services) => services
        .AddSingleton<IRsuService, RsuService>()
    ;

    public static IServiceCollection AddRsuStatusService(this IServiceCollection services) => services
    .AddScoped<IRsuStatusService, RsuStatusService>();

    public static IServiceCollection AddRsuStatusProducer(this IServiceCollection services, Action<RsuStatusProducerOptions> action) => services
        .AddMessaging()
        .Configure(action)
        .AddTransient<IBuildMessagingConfig<Guid, string>, BuildMessagingConfig<Guid, string>>()
        .AddTransient<IRsuStatusProducer, RsuStatusProducer>();
}