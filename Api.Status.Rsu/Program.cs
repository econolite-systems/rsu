// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Common.Extensions;
using Econolite.Ode.Authorization.Extensions;
using Econolite.Ode.Domain.Entities.Extensions;
using Econolite.Ode.Domain.Rsu.Extensions;
using Econolite.Ode.Messaging.Extensions;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics.Extensions;
using Econolite.Ode.Persistence.Mongo;
using Econolite.Ode.Repository.Entities;
using Econolite.Ode.Repository.Rsu.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Api.Status.Rsu;
using Econolite.Ode.Status.Rsu.Messaging.Extensions;
using Monitoring.AspNet.Metrics;

var builder = WebApplication.CreateBuilder(args);
const string allOrigins = "_allOrigins";

//// Add services to the container.
//builder.Host.AddODELogging();

builder.Services.AddMessaging();
//builder.Services.AddLogging();

builder.Services.AddMetrics(builder.Configuration, "RSU Status")
    .ConfigureRequestMetrics(c =>
    {
        c.RequestCounter = "Requests";
        c.ResponseCounter = "Responses";
    })
    .AddUserEventSupport(builder.Configuration, _ =>
    {
        _.DefaultSource = "RSU Status";
        _.DefaultLogName = Econolite.Ode.Monitoring.Events.LogName.SystemEvent;
        _.DefaultCategory = Econolite.Ode.Monitoring.Events.Category.Server;
        _.DefaultTenantId = Guid.Empty;
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allOrigins,
        policy =>
        {
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMvc(config =>
{
    config.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddAuthenticationJwtBearer(builder.Configuration, builder.Environment.IsDevelopment());

builder.Services.AddSwaggerGen(c =>
{


    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme,
                },
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header,
            },
            new List<string>()
        },
    });
});

builder.Services.AddMongo();

builder.Services.AddRsuStatusRepo();
builder.Services.AddRsuStatusConsumer(options =>
{
    options.ConfigTopic = builder.Configuration.GetValue("Topics:RsuStatus", "rsu.status")!;
});

builder.Services.AddHostedService<WorkerRsuStatus>();

builder.Services.AddEntityRepo();
builder.Services.AddEntityTypeService();
builder.Services.AddEntityService();

builder.Services.AddRsuStatusService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.AddRequestMetrics();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

/*app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});*/

app.LogStartup();

app.Run();