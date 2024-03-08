// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Commands.Rsu.Messaging;

public class TimCommandRequest : RsuCommandRequest
{
    public new string CommandType { get; set; } = "TimCommandRequest";
    public Action Action { get; set; } = Action.Create;
    public Guid Id { get; set; } = Guid.Empty;
    public Guid RsuId { get; set; } = Guid.Empty;
    public int? Index { get; set; }
    public bool IsAlternating { get; set; }
    public int Channel { get; set; } = 178;
    public int Interval { get; set; } = 5000;
    public DateTime DeliveryStart { get; set; }
    public TimeSpan DeliveryDuration { get; set; } = TimeSpan.FromMinutes(5);
    public bool Enable { get; set; }
    public double[] Location { get; set; } = Array.Empty<double>();
    public double[][][] Region { get; set; } = Array.Empty<double[][]>();
    public int[] Payload { get; set; } = Array.Empty<int>();
}