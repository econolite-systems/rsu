// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Commands.Rsu.Messaging.Extensions;

public static class Defined
{
    public static IServiceCollection AddRsuCommandRequestSink(this IServiceCollection services) => services
        .AddRsuCommandRequestSink(Consts.RSU_COMMAND_REQUEST_DEFAULT_CHANNEL);

    public static IServiceCollection AddRsuCommandRequestSink(this IServiceCollection services, IConfiguration configuration) => services
        .AddRsuCommandRequestSink(configuration[Consts.RSU_COMMAND_REQUEST_DEFAULT_CONFIGURATION_PATH] ?? Consts.RSU_COMMAND_REQUEST_DEFAULT_CHANNEL);

    public static IServiceCollection AddRsuCommandRequestSink(this IServiceCollection services, string channel) => services
        .AddRsuCommandRequestSink(options => options.DefaultChannel = channel);

    public static IServiceCollection AddRsuCommandRequestSink(this IServiceCollection services, Action<SinkOptions<RsuCommandRequest>> sinkOptions) => services
        .AddMessaging()
        .AddMessagingJsonSink(sinkOptions);

    public static IServiceCollection AddRsuCommandRequestSource(this IServiceCollection services) => services
        .AddRsuCommandRequestSource(Consts.RSU_COMMAND_REQUEST_DEFAULT_CHANNEL);

    public static IServiceCollection AddRsuCommandRequestSource(this IServiceCollection services, IConfiguration configuration) => services
        .AddRsuCommandRequestSource(configuration[Consts.RSU_COMMAND_REQUEST_DEFAULT_CONFIGURATION_PATH] ?? Consts.RSU_COMMAND_REQUEST_DEFAULT_CHANNEL);

    public static IServiceCollection AddRsuCommandRequestSource(this IServiceCollection services, string channel) => services
        .AddRsuCommandRequestSource(options => options.DefaultChannel = channel);

    public static IServiceCollection AddRsuCommandRequestSource(this IServiceCollection services, Action<SourceOptions<RsuCommandRequest>> sourceOptions) => services
        .AddMessaging()
        .AddMessagingJsonSource<RsuCommandRequest>(sourceOptions);

    public static IServiceCollection AddRsuCommandResponseSink(this IServiceCollection services) => services
    .AddRsuCommandResponseSink(Consts.RSU_COMMAND_RESPONSE_DEFAULT_CHANNEL);

    public static IServiceCollection AddRsuCommandResponseSink(this IServiceCollection services, IConfiguration configuration) => services
        .AddRsuCommandResponseSink(configuration[Consts.RSU_COMMAND_RESPONSE_DEFAULT_CONFIGURATION_PATH] ?? Consts.RSU_COMMAND_RESPONSE_DEFAULT_CHANNEL);

    public static IServiceCollection AddRsuCommandResponseSink(this IServiceCollection services, string channel) => services
        .AddRsuCommandResponseSink(options => options.DefaultChannel = channel);

    public static IServiceCollection AddRsuCommandResponseSink(this IServiceCollection services, Action<SinkOptions<RsuCommandResponse>> sinkOptions) => services
        .AddMessaging()
        .AddMessagingJsonSink(sinkOptions);

    public static IServiceCollection AddRsuCommandResponseSource(this IServiceCollection services) => services
        .AddRsuCommandResponseSource(Consts.RSU_COMMAND_RESPONSE_DEFAULT_CHANNEL);

    public static IServiceCollection AddRsuCommandResponseSource(this IServiceCollection services, IConfiguration configuration) => services
        .AddRsuCommandResponseSource(configuration[Consts.RSU_COMMAND_RESPONSE_DEFAULT_CONFIGURATION_PATH] ?? Consts.RSU_COMMAND_RESPONSE_DEFAULT_CHANNEL);

    public static IServiceCollection AddRsuCommandResponseSource(this IServiceCollection services, string channel) => services
        .AddRsuCommandResponseSource(options => options.DefaultChannel = channel);

    public static IServiceCollection AddRsuCommandResponseSource(this IServiceCollection services, Action<SourceOptions<RsuCommandResponse>> sourceOptions) => services
        .AddMessaging()
        .AddMessagingJsonSource<RsuCommandResponse>(sourceOptions);
    
    public static IServiceCollection AddRawTimJsonResponseSink(this IServiceCollection services) => services
        .AddRawTimJsonResponseSink(Consts.TIM_RAW_ENCODED_JSON_DEFAULT_CHANNEL);

    public static IServiceCollection AddRawTimJsonResponseSink(this IServiceCollection services, IConfiguration configuration) => services
        .AddRawTimJsonResponseSink(configuration[Consts.TIM_RAW_ENCODED_JSON_DEFAULT_CONFIGURATION_PATH] ?? Consts.TIM_RAW_ENCODED_JSON_DEFAULT_CHANNEL);

    public static IServiceCollection AddRawTimJsonResponseSink(this IServiceCollection services, string channel) => services
        .AddRawTimJsonResponseSink(options => options.DefaultChannel = channel);

    public static IServiceCollection AddRawTimJsonResponseSink(this IServiceCollection services, Action<SinkOptions<RawTimMessageJsonResponse>> sinkOptions) => services
        .AddMessaging()
        .AddMessagingJsonSink(sinkOptions);
}