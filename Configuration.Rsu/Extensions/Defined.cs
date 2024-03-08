// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Configuration.Rsu.Extensions;

public static class Defined
{
    public static IServiceCollection AddRsuService(this IServiceCollection services)
    {
        services.AddScoped<IRsuService, RsuService>();

        return services;
    }
}