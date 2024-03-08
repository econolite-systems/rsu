// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Status.Rsu;
using Lextm.SharpSnmpLib;

namespace Econolite.Ode.Snmp.Rsu;

public static class RsuMib
{
    public const string rsuMIB = "1.0.15628.4.1";

    public static ObjectIdentifier ToObjectIdentifier(string oid)
    {
        return new ObjectIdentifier(oid);
    }
}

public static class RsuSystemStatus
{
    public const string rsuSystemStats = $"{RsuMib.rsuMIB}.16";
    public const string rsuTimeSincePowerOn = $"{rsuSystemStats}.1.0";
    public const string rsuTotalRunTime = $"{rsuSystemStats}.2.0";
    public const string rsuLastRestartTime = $"{rsuSystemStats}.6.0";
    public const string rsuSysDescription = $"{RsuMib.rsuMIB}.17";
    public const string rsuMibVersion = $"{rsuSysDescription}.1.0";
    public const string rsuFirmwareVersion = $"{rsuSysDescription}.2.0";
    public const string rsuLocationDesc = $"{rsuSysDescription}.3.0";
    public const string rsuID = $"{rsuSysDescription}.4.0";
    public const string rsuManufacturer = $"{rsuSysDescription}.5.0";
    public const string rsuSystemStatus = $"{RsuMib.rsuMIB}.19";
    public const string rsuChanStatus = $"{rsuSystemStatus}.1.0";
    
    public static IList<Variable> CreateVariableList()
    {
        var vList = new List<Variable>
        {
            new (RsuMib.ToObjectIdentifier(rsuTimeSincePowerOn)),
            new (RsuMib.ToObjectIdentifier(rsuTotalRunTime)),
            new (RsuMib.ToObjectIdentifier(rsuLastRestartTime)),
            new (RsuMib.ToObjectIdentifier(rsuMibVersion)),
            new (RsuMib.ToObjectIdentifier(rsuFirmwareVersion)),
            new (RsuMib.ToObjectIdentifier(rsuLocationDesc)),
            new (RsuMib.ToObjectIdentifier(rsuID)),
            new (RsuMib.ToObjectIdentifier(rsuManufacturer)),
            new (RsuMib.ToObjectIdentifier(rsuChanStatus))
        };

        return vList;
    }

    public static RsuSystemStats ToRsuSystemStats(this IPollResponse response)
    {
        if ( response.Error != string.Empty)
        {
            return new RsuSystemStats()
            {
                IsConnected = false,
                Error = response.Error,
            };
        }
        
        return new RsuSystemStats()
        {
            IsConnected = response.Error == string.Empty,
            TimeSincePowerOn = response.ToRsuTimeSincePowerOn(),
            TotalRunTime = response.ToRsuTotalRunTime(),
            LastRestartTime = response.ToRsuLastRestartTime(),
            MibVersion = response.ToRsuMibVersion(),
            FirmwareVersion = response.ToRsuFirmwareVersion(),
            LocationDescription = response.ToRsuLocationDesc(),
            RsuId = response.ToRsuID(),
            Manufacturer = response.ToRsuManufacturer(),
            ChannelStatus = response.ToRsuChanStatus()
        };
    }
    
    public static TimeSpan ToRsuTimeSincePowerOn(this IPollResponse response)
    {
        var variable = response.Variables.FirstOrDefault(v => v.Id.ToString() == rsuTimeSincePowerOn);
        var seconds = variable?.Data.ToSeconds() ?? TimeSpan.Zero;
        return seconds;
    }
    
    public static TimeSpan ToRsuTotalRunTime(this IPollResponse response)
    {
        var variable = response.Variables.FirstOrDefault(v => v.Id.ToString() == rsuTotalRunTime);
        var seconds = variable?.Data.ToSeconds() ?? TimeSpan.Zero;
        return seconds;
    }
    
    public static DateTime ToRsuLastRestartTime(this IPollResponse response)
    {
        var variable = response.Variables.FirstOrDefault(v => v.Id.ToString() == rsuLastRestartTime);
        return variable?.Data.ToDateTime() ?? DateTime.MinValue;
    }
    
    public static string ToRsuMibVersion(this IPollResponse response)
    {
        var variable = response.Variables.FirstOrDefault(v => v.Id.ToString() == rsuMibVersion);
        return variable?.Data.ToString() ?? string.Empty;
    }
    
    public static string ToRsuFirmwareVersion(this IPollResponse response)
    {
        var variable = response.Variables.FirstOrDefault(v => v.Id.ToString() == rsuFirmwareVersion);
        return variable?.Data.ToString() ?? string.Empty;
    }
    
    public static string ToRsuLocationDesc(this IPollResponse response)
    {
        var variable = response.Variables.FirstOrDefault(v => v.Id.ToString() == rsuLocationDesc);
        return variable?.Data.ToString() ?? string.Empty;
    }
    
    public static string ToRsuID(this IPollResponse response)
    {
        var variable = response.Variables.FirstOrDefault(v => v.Id.ToString() == rsuID);
        return variable?.Data.ToString() ?? string.Empty;
    }
    
    public static string ToRsuManufacturer(this IPollResponse response)
    {
        var variable = response.Variables.FirstOrDefault(v => v.Id.ToString() == rsuManufacturer);
        return variable?.Data.ToString() ?? string.Empty;
    }
    
    public static ChanStatus ToRsuChanStatus(this IPollResponse response)
    {
        var variable = response.Variables.FirstOrDefault(v => v.Id.ToString() == rsuChanStatus);
        var status = (ChanStatus) variable?.Data.ToInt();
        return status;
    }
    
    public static TimeSpan ToSeconds(this ISnmpData data)
    {
     var counter = data as Counter32;
     var seconds = new TimeSpan(0, 0, 0, int.Parse(counter?.ToString() ?? "0"));
     return seconds;
    }
}