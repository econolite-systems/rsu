// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Models.Rsu.Status;

public class StoreRepeatMessageTable
{
    public Guid RsuId { get; set; }
    public IList<StoreRepeatMessage> Rows { get; set; } = new List<StoreRepeatMessage>();
}