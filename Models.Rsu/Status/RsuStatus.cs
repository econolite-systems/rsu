// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Models.Entities;
using Econolite.Ode.Persistence.Common.Interfaces;
using Econolite.Ode.Status.Rsu;

namespace Econolite.Ode.Models.Rsu.Status;

public class RsuStatus : IIndexedEntity<Guid>
{
    public Guid Id { get; set; } = Guid.Empty;
    
    public Guid DeviceId { get; set; } = Guid.Empty;
    public DateTime TimeStamp { get; set; }
    public bool IsConnected { get; set; }
    public bool IsConfigured { get; set; }
    public string Error { get; set; } = string.Empty;
}

public class RsuStatusDto : RsuStatus
{
    public string Name { get; set; } = string.Empty;
}

public static class RsuStatusExtensions
{
    public static RsuStatus ToRsuStatus(this RsuSystemStats stats, Guid id)
    {
        return new RsuStatus
        {
            Id = Guid.NewGuid(),
            DeviceId = id,
            TimeStamp = DateTime.UtcNow,
            IsConnected = stats.IsConnected,
            IsConfigured = stats.ChannelStatus == ChanStatus.BothOp,
            Error = stats.Error
        };
    }

    public static RsuStatusDto AdaptStatusDocumentWithListToDto(this RsuStatus rsuStatusMessage, IEnumerable<EntityNode> rsuCollection)
    {
        var rsu = rsuCollection.FirstOrDefault(x => x.Id == rsuStatusMessage.DeviceId);

        return AddRsuEntity(rsuStatusMessage, rsu);
    }

    public static RsuStatusDto AddRsuEntity(this RsuStatus status, EntityNode? rsu)
    {

        return new RsuStatusDto
        {
            Id = Guid.NewGuid(),
            DeviceId = status.DeviceId,
            TimeStamp = status.TimeStamp,
            IsConnected = status.IsConnected,
            IsConfigured = status.IsConfigured,
            Error = status.Error,
            Name = rsu == null ? "Unknown" : rsu.Name,
        };
    }
}