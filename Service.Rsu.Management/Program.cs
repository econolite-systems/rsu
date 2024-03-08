// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Common.Extensions;
using Configuration.Snmp.Rsu.Extensions;
using Econolite.Ode.Commands.Rsu.Messaging.Extensions;
using Econolite.Ode.Domain.Asn1.J2735.Extensions;
using Econolite.Ode.Domain.Rsu.Extensions;
using Econolite.Ode.Modules;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics.Extensions;
using Econolite.Ode.Snmp.Rsu.Extensions;
using Module.Rsu.Modules;
using Service.Rsu.Management;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddODELogging();
builder.Services.RegisterModules(typeof(RsuManagement));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { });

builder.Services.AddMetrics(builder.Configuration, "RSU Management")
    .AddUserEventSupport(builder.Configuration, _ =>
    {
        _.DefaultSource = "RSU Management";
        _.DefaultLogName = Econolite.Ode.Monitoring.Events.LogName.SystemEvent;
        _.DefaultCategory = Econolite.Ode.Monitoring.Events.Category.Server;
        _.DefaultTenantId = Guid.Empty;
    });
builder.Services.AddAsn1J2735Service();
builder.Services.AddRsuStatusProducer(_ => { });
builder.Services.AddRsuService();
builder.Services.AddSnmpService();
builder.Services.AddConfigRequestProducer(options =>
    options.ConfigRequestTopic = builder.Configuration["Topics:ConfigRequest"]!);
builder.Services.AddConfigProducer(options =>
    options.ConfigResponseTopic = builder.Configuration["Topics:ConfigResponse"]!);
builder.Services.AddConfigConsumers(builder.Configuration);
builder.Services.AddConfigRequestConsumers(builder.Configuration);
builder.Services.AddRsuCommandRequestSource(builder.Configuration);
builder.Services.AddRsuCommandResponseSink(builder.Configuration);
builder.Services.AddRawTimJsonResponseSink(builder.Configuration);
builder.Services.AddHostedService<ConfigurationResponseHandler>();
builder.Services.AddHostedService<ConfigurationRequest>();
builder.Services.AddHostedService<CommandRequestHandler>();

var app = builder.Build();
app.LogStartup();
// Configure
app.MapEndpoints();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();