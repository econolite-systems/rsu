// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Models.Rsu.Status;

namespace Econolite.Ode.Snmp.Rsu.Commands;

public class StoreRepeatMessageCommandResponse : CommandResponse
{
    public StoreRepeatMessage? Message { get; set; }
}