// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Text.Json.Serialization;

namespace Econolite.Ode.Commands.Rsu.Messaging;

public class TimCommandResponse : RsuCommandResponse
{
    public new string CommandType { get; set; } = "TimCommandResponse";
    public Action Action { get; set; } = Action.Create;
    public Guid Id { get; set; } = Guid.Empty;
    public Guid RsuId { get; set; } = Guid.Empty;
    public int? Index { get; set; }
    public DateTime DeliveryStart { get; set; }
    public TimeSpan DeliveryDuration { get; set; } = TimeSpan.FromMinutes(5);
    public bool IsBroadcasting { get; set; }
    public string Error { get; set; } = string.Empty;
}

public class RawTimMessageJsonResponse
{
    [JsonPropertyName("TimMessageContent")]
    public IEnumerable<RawMessageContent> TimMessageContent { get; set; } = Array.Empty<RawMessageContent>();
}

public class RawMessageContent
{
    [JsonPropertyName("metadata")]
    public RawMessageMetadata Metadata { get; set; } = new RawMessageMetadata();
    [JsonPropertyName("payload")]
    public string Payload { get; set; } = string.Empty;
}

public class RawMessageMetadata
{
    [JsonPropertyName("utctimestamp")]
    public string UtcTimestamp { get; set; } = string.Empty;
    [JsonPropertyName("originRsu")]
    public string OriginRsu { get; set; } = string.Empty;
}