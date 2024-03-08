// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Commands.Rsu.Messaging;

public class RsuCommandResponse
{
    public string CommandType { get; set; } = "";
    public DateTime TimeStamp { get; set; }
    public bool IsSuccess { get; set; }
}