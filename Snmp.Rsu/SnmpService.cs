// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Collections.Concurrent;
using System.Data.Common;
using Econolite.Ode.Models.Rsu;
using Econolite.Ode.Models.Rsu.Status;
using Econolite.Ode.Snmp.Rsu.Commands;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Snmp.Rsu;

public interface ISnmpService
{
    IEnumerable<Guid> GetRsus();
    RsuDevice GetRsu(Guid deviceId);
    Task UpdateConfigAsync(IEnumerable<Models.Rsu.Configuration.Rsu> rsus);
    Task<ICommandResponse> SetTimMessage(Guid deviceId, TimMessageSnmp snmp, int? index);
    Task<StoreRepeatMessage?> GetSrmRow(Guid deviceId, int index);
    Task<IEnumerable<StoreRepeatMessage>?> GetSrm(Guid deviceId);
    Task<ICommandResponse> DeleteRow(Guid deviceId, int index);
}

public class SnmpService : ISnmpService
{
    private IDictionary<Guid, RsuDevice> _devices = new ConcurrentDictionary<Guid, RsuDevice>();
    private readonly IPollingService _pollingService;
    private readonly IRsuFactory _rsuFactory;

    public SnmpService(IPollingService pollingService, IRsuFactory rsuFactory, ILoggerFactory loggerFactory)
    {
        _pollingService = pollingService;
        _rsuFactory = rsuFactory;
    }

    public IEnumerable<Guid> GetRsus()
    {
        return _devices.Values.Select(d => (d.Id));
    }
    
    public RsuDevice GetRsu(Guid deviceId)
    {
        if (_devices.TryGetValue(deviceId, out var device))
        {
            return device;
        }
        else
        {
            return null;
        }
    }
    
    public async Task UpdateConfigAsync(IEnumerable<Models.Rsu.Configuration.Rsu> rsus)
    {
        _devices.Clear();
        _pollingService.Clear();

        foreach (var device in ToRsuDevices(rsus).ToArray())
        {
            _devices[device.Id] = device;
        }
        
        await Task.CompletedTask;
    }

    public async Task<ICommandResponse> SetTimMessage(Guid deviceId, TimMessageSnmp snmp, int? index)
    {
        if (_devices.TryGetValue(deviceId, out var device))
        {
            return await device.SetTimMessage(snmp, index);
        }else
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = new SnmpDevice() { DeviceId = deviceId },
                Error = $"Device {deviceId} not found"
            };
        }
    }

    public async Task<StoreRepeatMessage?> GetSrmRow(Guid deviceId, int index)
    {
        if (_devices.TryGetValue(deviceId, out var device))
        {
            return await device.GetRow(index);
        }else
        {
            return null;
        }
    }

    public async Task<IEnumerable<StoreRepeatMessage>?> GetSrm(Guid deviceId)
    {
        if (_devices.TryGetValue(deviceId, out var device))
        {
            return await device.GetSrmData();
        }else
        {
            return null;
        }
        
    }
    
    public async Task<ICommandResponse> DeleteRow(Guid deviceId, int index)
    {
        if (_devices.TryGetValue(deviceId, out var device))
        {
            return await device.DeleteRow(index);
        }else
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = new SnmpDevice() { DeviceId = deviceId },
                Error = $"Device {deviceId} not found"
            };
        }
        
    }

    private IEnumerable<RsuDevice> ToRsuDevices(IEnumerable<Models.Rsu.Configuration.Rsu> rsus)
    {
        return rsus.Select(rsu => _rsuFactory.Create(rsu));
    }
}