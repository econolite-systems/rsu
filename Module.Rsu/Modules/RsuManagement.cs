// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Asn1J2735.Tim;
using Econolite.Ode.Domain.Asn1.J2735;
using Econolite.Ode.Models.Rsu.Status;
using Econolite.Ode.Modules;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Econolite.Ode.Snmp.Rsu;
using TimMessageSnmp = Econolite.Ode.Models.Rsu.TimMessageSnmp;

namespace Module.Rsu.Modules;

public class RsuManagement : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection builder)
    {
        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(WebApplication app)
    {
        var metricsFactory = app.Services.GetRequiredService<IMetricsFactory>();
        var loopCounter = metricsFactory.GetMetricsCounter("TIMs");
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("RSU.API");
        var userEventFactory = app.Services.GetRequiredService<UserEventFactory>();

        app.MapGet("rsu", (ISnmpService snmpService) => snmpService.GetRsus());
        app.MapGet("rsu/{rsu}/srm", async (Guid rsu, ISnmpService snmpService) => await snmpService.GetSrm(rsu));
        app.MapPost("tim", async (RequestMessage message, IAsn1J2735Service asn1Service, ISnmpService snmpService) =>
        {
            var payload = asn1Service.EncodeTim(message.Tim);
            if (!string.IsNullOrEmpty(payload))
            {
                var timMessage = message.ToTimMessageSnmp(payload);
                await snmpService.SetTimMessage(message.Request.Rsu, timMessage, null);
            }
            else
            {
                logger.ExposeUserEvent(userEventFactory.BuildUserEvent(EventLevel.Warning, string.Format("Unable to parse received Tim message: {0}", message.Tim)));
            }

            loopCounter.Increment();
        });
        app.MapPut("tim", async (RequestMessage message, IAsn1J2735Service asn1Service, ISnmpService snmpService) =>
        {
            var payload = asn1Service.EncodeTim(message.Tim);
            if (!string.IsNullOrEmpty(payload))
            {
                var timMessage = message.ToTimMessageSnmp(payload);
                await snmpService.SetTimMessage(message.Request.Rsu, timMessage, null);
            }
            else
            {
                logger.ExposeUserEvent(userEventFactory.BuildUserEvent(EventLevel.Warning, string.Format("Unable to parse received Tim message: {0}", message.Tim)));
            }

            loopCounter.Increment();
        });
        app.MapDelete("rsu/{rsu}/srm/{row}", async (Guid rsu, int row, ISnmpService snmpService) =>
        {
            await snmpService.DeleteRow(rsu, row);
            loopCounter.Increment();
        });
        return app;
    }
}

public static class RsuManagementExtensions
{
    public static TimMessageSnmp ToTimMessageSnmp(this RequestMessage value, string payload)
    {
        return new TimMessageSnmp
        {
            Channel = value.Request.Snmp.Channel,
            Enable = value.Request.Snmp.Enable ? 1 : 0,
            Interval = value.Request.Snmp.Interval,
            Mode = value.Request.Snmp.Mode,
            Msgid = value.Request.Snmp.Msgid,
            RsuId = value.Request.Snmp.RsuId,
            Status = value.Request.Snmp.Status,
            Payload = payload,
            Deliverystart = value.Request.Snmp.DeliveryStart.ToHexString(),
            Deliverystop = value.Request.Snmp.DeliveryStop.ToHexString()
        };
    }
}