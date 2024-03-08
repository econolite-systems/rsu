// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Asn1J2735.J2735;
using Econolite.Asn1J2735.J2735.TimStorage;
using Econolite.Asn1J2735.Tim;
using Econolite.Ode.Commands.Rsu.Messaging;
using Econolite.Ode.Domain.Asn1.J2735;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Rsu.Status;
using Econolite.Ode.Monitoring.Metrics;
using Econolite.Ode.Snmp.Rsu;
using Econolite.Ode.Snmp.Rsu.Commands;
using Action = Econolite.Ode.Commands.Rsu.Messaging.Action;
using TimMessageSnmp = Econolite.Ode.Models.Rsu.TimMessageSnmp;

namespace Service.Rsu.Management;

public class CommandRequestHandler : BackgroundService
{
    private readonly ISource<RsuCommandRequest> _rsuCommandRequestSource;
    private readonly ISink<RsuCommandResponse> _rsuCommandResponseSink;
    private readonly ISink<RawTimMessageJsonResponse> _rawTimMessageJsonResponseSink;
    private readonly ISnmpService _snmpService;

    private readonly ILogger<CommandRequestHandler> _logger;
    private readonly IMetricsCounter _messageCounter;

    public CommandRequestHandler(ISource<RsuCommandRequest> rsuCommandRequestSource, ISink<RsuCommandResponse> rsuCommandResponseSink, ISink<RawTimMessageJsonResponse> rawTimMessageJsonResponseSink, IMetricsFactory metricsFactory, ISnmpService snmpService, ILogger<CommandRequestHandler> logger)
    {
        _rsuCommandRequestSource = rsuCommandRequestSource;
        _rsuCommandResponseSink = rsuCommandResponseSink;
        _rawTimMessageJsonResponseSink = rawTimMessageJsonResponseSink;
        _snmpService = snmpService;
        _logger = logger;
        _messageCounter = metricsFactory.GetMetricsCounter("Rsu command request");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting to consume RsuCommandRequest");
        try
        {
            await _rsuCommandRequestSource.ConsumeOnAsync(async result =>
            {
                _messageCounter.Increment();
                if (result.Type == "TimCommandRequest")
                {
                    await HandleTimCommandRequestAsync(result, stoppingToken);
                }
            }, stoppingToken);
        }
        finally
        {
            _logger.LogInformation("Ending RsuCommandRequest consumption");
        }
    }

    private async Task HandleTimCommandRequestAsync(ConsumeResult<Guid, RsuCommandRequest> result, CancellationToken stoppingToken)
    {
        try
        {
            var timCommandRequest = result.ToObject<TimCommandRequest>();
            if (timCommandRequest == null)
            {
                _logger.LogError("TimCommandRequest is null");
                return;
            }

            var response = new TimCommandResponse()
            {
                TimeStamp = DateTime.UtcNow,
                Id = timCommandRequest.Id,
                RsuId = timCommandRequest.RsuId
            };

            if (timCommandRequest.Action is Action.Delete)
            {
                if (!timCommandRequest.Index.HasValue)
                {
                    response.Error = "Row index is required for delete message";
                    await _rsuCommandResponseSink.SinkAsync(response.Id, response, stoppingToken);
                    return;
                }

                var deleteResponse =
                    await _snmpService.DeleteRow(timCommandRequest.RsuId, timCommandRequest.Index.Value);
                response.Error = deleteResponse.Error;
                response.IsSuccess = string.IsNullOrEmpty(deleteResponse.Error);
                response.Index = timCommandRequest.Index;
                response.IsBroadcasting = false;
                response.Action = timCommandRequest.Action;
                await _rsuCommandResponseSink.SinkAsync(response.Id, response, stoppingToken);
                return;
            }

            var timMessage = timCommandRequest.ToTimMessageSnmp();
            var commandResponse =
                await _snmpService.SetTimMessage(timCommandRequest.RsuId, timMessage, timCommandRequest.Index);
            response = commandResponse.ToTimCommandResponse(timCommandRequest.Id);
            response.Action = timCommandRequest.Action;
            
            if (response.IsBroadcasting)
            {
                var device = _snmpService.GetRsu(timCommandRequest.RsuId);
                var ip = device.Device.Target;
                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                var message = new RawTimMessageJsonResponse()
                {
                    TimMessageContent = new []
                    {
                        new RawMessageContent()
                        {
                            Metadata = new RawMessageMetadata()
                            {
                                OriginRsu = ip,
                                UtcTimestamp = timestamp
                            },
                            Payload = timMessage.Payload
                        }
                    }
                };
                await _rawTimMessageJsonResponseSink.SinkAsync(timCommandRequest.Id, message, stoppingToken);
            }
            
            await _rsuCommandResponseSink.SinkAsync(timCommandRequest.Id, response, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}

public static class TimBroadcastExtensions
{
    public static TimMessageSnmp ToTimMessageSnmp(this TimCommandRequest timBroadcast)
    {
        var stopTime = timBroadcast.DeliveryStart.AddMinutes(timBroadcast.DeliveryDuration.Minutes);
        return new TimMessageSnmp
        {
            RsuId = timBroadcast.Id.ToString(),
            Msgid = (int)J2735MsgId.TIM,
            Mode = timBroadcast.IsAlternating ? (int)TransmitMode.Alternating : (int)TransmitMode.Continuous,
            Channel = timBroadcast.Channel,
            Interval = timBroadcast.Interval,
            Deliverystart = timBroadcast.DeliveryStart.ToHexString(),
            Deliverystop = stopTime.ToHexString(),
            Enable = timBroadcast.Enable ? 1 : 0,
            Payload = timBroadcast.ToEncodedString()
        };
    }
    
    private static string ToEncodedString(this TimCommandRequest timBroadcast)
    {
        var timMessage = new TimMessage()
        {
            TimeStamp = timBroadcast.DeliveryStart,
            DataFrames = new []
            {
                new TimDataFrameMessage()
                {
                    Duration = timBroadcast.DeliveryDuration.Minutes,
                    MsgId = new MsgIdType() { RoadSignID = new RoadSignId()
                    {
                        Position = timBroadcast.Location.ToPosition3d(),
                        MutcdCode = MUTCDCode.Warning,
                        ViewAngle = "0000000000000000"
                    }},
                    StartTime = timBroadcast.DeliveryStart,
                    Content = new Content()
                    {
                        Advisory = new ItisCodesAndText()
                        {
                            Items = timBroadcast.Payload.Select(code => new Item()
                            {
                                Itis = code
                            }).ToArray()
                        }
                    },
                    Regions = new []{new GeographicalPath()}
                }
            }
        };

        return timMessage.Encode();
    }
    
    public static GeographicalPath ToGeographicalPath(this TimCommandRequest request, double[][][] region)
    {
        return new GeographicalPath()
        {
            Anchor = request.Location.ToPosition3d(),
            ClosedPath = true,
            Description = request.ToGeometryType()
        };
    }
    
    public static GeometryType ToGeometryType(this TimCommandRequest request)
    {
        return new GeometryType()
        {
            Path = request.ToOffsetSystem()
        };
    }
    
    public static OffsetSystem ToOffsetSystem(this TimCommandRequest request)
    {
        return new OffsetSystem()
        {
            Offset = request.ToNodeList()
        };
    }
    
    public static NodeList ToNodeList(this TimCommandRequest request)
    {
        return new NodeList()
        {
            Xy = request.ToNodeListXY()
        };
    }
    
    public static NodeListXY ToNodeListXY(this TimCommandRequest request)
    {
     return new NodeListXY()
        {
            Nodes = request.ToNodeSetXY()
        };
    }
    
    public static NodeSetXY ToNodeSetXY(this TimCommandRequest request)
    {
        return new NodeSetXY()
        {
            NodeXY = request.Region[0].ToNodeXY()
        };
    }
    
    public static List<NodeXY> ToNodeXY(this double[][] region)
    {
        return region.Select(p => p.ToNodeXY()).ToList();
    }
    
    public static NodeXY ToNodeXY(this double[] location)
    {
        return new NodeXY()
        {
            Delta = new NodeOffsetPointXY()
            {
                NodeLatLon = new NodeLLmD64b()
                {
                    Lat = location[1],
                    Lon = location[0]
                }
            }
        };
    }
    
    public static Position3d ToPosition3d(this double[] location)
    {
        return new Position3d()
        {
            Latitude = location[1],
            Longitude = location[0]
        };
    }
    
    public static TimCommandResponse ToTimCommandResponse(this ICommandResponse commandResponse, Guid id)
    {
        var timCommandResponse = new TimCommandResponse()
        {
            Id = id,
            TimeStamp = DateTime.UtcNow,
            RsuId = commandResponse.Device.DeviceId,
            Error = commandResponse.Error
        };

        if (!string.IsNullOrEmpty(commandResponse.Error)) return timCommandResponse;
        
        var response = commandResponse as StoreRepeatMessageCommandResponse;
        if (response?.Message == null)
        {
            return timCommandResponse;
        }
        timCommandResponse.Action = timCommandResponse.Action;
        timCommandResponse.Index = response.Message.Id;
        timCommandResponse.DeliveryStart = response.Message.DeliveryStart;
        timCommandResponse.DeliveryDuration = response.Message.DeliveryStop.Subtract(response.Message.DeliveryStart);
        timCommandResponse.IsBroadcasting = !response.Message.Enable;
        timCommandResponse.IsSuccess = true;

        return timCommandResponse;
    }
}