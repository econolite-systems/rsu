// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Common.Scheduler.Base.Timers;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Snmp.Rsu.Extensions;

public static class Defined
{
    public static IServiceCollection AddSnmpService(this IServiceCollection services) => services
        .AddTransient<IPeriodicTimerFactory, PeriodicTimerFactory>()
        .AddTransient<IPollProducer<IPollResponse>, PollProducer>()
        .AddSingleton<IPollingService, PollingService>()
        .AddTransient<ISnmpServer, SnmpServer>()
        .AddSingleton<IRsuFactory, RsuFactory>()
        .AddSingleton<ISnmpService, SnmpService>()
    ;
}