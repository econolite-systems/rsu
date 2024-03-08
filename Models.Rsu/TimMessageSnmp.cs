// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Models.Rsu;

public class TimMessageSnmp
{
    public string RsuId { get; set; }
    public int Msgid { get; set; }
    public int Mode { get; set; }
    public int Channel { get; set; }
    public int Interval { get; set; }
    public string Deliverystart { get; set; }
    public string Deliverystop { get; set; }
    public int Enable { get; set; }
    public int Status { get; set; }
    public string Payload { get; set; }
}