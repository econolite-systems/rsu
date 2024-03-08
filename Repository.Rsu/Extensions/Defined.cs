// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Persistence.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Repository.Rsu.Extensions;

public static class Defined
{
    public static IServiceCollection AddRsuRepo(this IServiceCollection services)
    {
        services.AddScoped<IRsuRepository, RsuRepository>();

        return services;
    }
    
    public static IServiceCollection AddRsuStatusRepo(this IServiceCollection services)
    {
        services.AddScoped<IRsuStatusRepository, RsuStatusRepository>();

        return services;
    }
}