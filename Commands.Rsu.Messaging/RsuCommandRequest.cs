// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Commands.Rsu.Messaging;

public class RsuCommandRequest
{
    public string CommandType { get; set; } = "";
    public DateTime TimeStamp { get; set; }
}