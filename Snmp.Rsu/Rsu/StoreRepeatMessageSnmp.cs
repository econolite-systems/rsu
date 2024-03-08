// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Models.Rsu.Status;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Econolite.Ode.Snmp.Rsu;

public static class StoreRepeatMessageSnmp
{
    public const string rsuSRMPsid          = "1.0.15628.4.1.4.1.2";
    public const string rsuSRMDsrcMsgId     = "1.0.15628.4.1.4.1.3";   
    public const string rsuSRMTxMode        = "1.0.15628.4.1.4.1.4";   
    public const string rsuSRMTxChannel     = "1.0.15628.4.1.4.1.5";   
    public const string rsuSRMTxInterval    = "1.0.15628.4.1.4.1.6";   
    public const string rsuSRMDeliveryStart = "1.0.15628.4.1.4.1.7";   
    public const string rsuSRMDeliveryStop  = "1.0.15628.4.1.4.1.8";   
    public const string rsuSRMPayload       = "1.0.15628.4.1.4.1.9";   
    public const string rsuSRMEnable        = "1.0.15628.4.1.4.1.10";   
    public const string rsuSRMStatus        = "1.0.15628.4.1.4.1.11";

    public static IList<Variable> CreateRowIndexVariables(int startingRow = 1, int length = 99)
    {
        var vList = new List<Variable>();
        for ( var i = startingRow; i <= (startingRow+length); i++ )
        {
            vList.Add(new Variable(new ObjectIdentifier("1.0.15628.4.1.4.1.11." + i)));
        }

        return vList;
    }
    
    public static IList<Variable> CreateVariableList(params int[] args)
    {
        var vList = new List<Variable>();
        foreach (var row in args)
        {
            vList.AddRange(StoreRepeatMessageSnmp.CreateVariableList(row));
        }
        return vList;
    }

    public static IList<Variable> CreateVariableList(int index)
    {
        var vList = new List<Variable>
        {
            new (ToObjectIdentifier(rsuSRMPsid, index)),
            new (ToObjectIdentifier(rsuSRMDsrcMsgId, index)),
            new (ToObjectIdentifier(rsuSRMTxMode, index)),
            new (ToObjectIdentifier(rsuSRMTxChannel, index)),
            new (ToObjectIdentifier(rsuSRMTxInterval, index)),
            new (ToObjectIdentifier(rsuSRMDeliveryStart, index)),
            new (ToObjectIdentifier(rsuSRMDeliveryStop, index)),
            new (ToObjectIdentifier(rsuSRMPayload, index)),
            new (ToObjectIdentifier(rsuSRMEnable, index)),
            new (ToObjectIdentifier(rsuSRMStatus, index))
        };

        return vList;
    }

    private static ObjectIdentifier ToObjectIdentifier(string oid, int index)
    {
        return new ObjectIdentifier($"{oid}.{index}");
    }

    public static IEnumerable<StoreRepeatMessage> ToSRMData(
        this IEnumerable<Variable> vList)
    {
        var list = vList.Select(v =>
            {
                var split = v.Id.ToString().Split(".");
                var id = int.Parse(split.Last());
                var oid = string.Join(".", split.SkipLast(1));
                var data = v.Data;
                return (oid, id, data);
            })
            .GroupBy(v => v.id).Select(g =>
                g.ToSRMRow()
            );
        return list;
    }
    
    public static StoreRepeatMessage ToSRMRow(
        this IGrouping<int, (string oid, int id, ISnmpData data)> g)
    {

        var row = new StoreRepeatMessage()
        {
            Id = g.Key,
            Psid = g.ToPsid(),
            MsgId = g.ToMsgid(),
            TxMode = g.ToTxMode(),
            TxChannel = g.ToTxChannel(),
            TxInterval = g.ToTxInterval(),
            DeliveryStart = g.ToDeliveryStart(),
            DeliveryStop = g.ToDeliveryStop(),
            Payload = g.ToPayload(),
            Enable = g.ToEnable()
        };

        return row;
    }
    
    public static string ToPsid(this IGrouping<int, (string oid, int id, ISnmpData data)> g)
    {
        var value = g.FirstOrDefault(v => v.oid == rsuSRMPsid);
        return value.data.ToPsid();
    }

    public static string ToPsid(this ISnmpData value)
    {
        return value.ToHexString();
    }
    
    public static J2735MsgId ToMsgid(this IGrouping<int, (string oid, int id, ISnmpData data)> g)
    {
        var value = g.FirstOrDefault(v => v.oid == rsuSRMDsrcMsgId);
        return value.data.ToMsgid();
    }

    public static J2735MsgId ToMsgid(this ISnmpData value)
    {
        return (J2735MsgId) value.ToInt();
    }
    
    public static TransmitMode ToTxMode(this IGrouping<int, (string oid, int id, ISnmpData data)> g)
    {
        var value = g.FirstOrDefault(v => v.oid == rsuSRMTxMode);
        return value.data.ToTxMode();
    }

    public static TransmitMode ToTxMode(this ISnmpData value)
    {
        return (TransmitMode) value.ToInt();
    }
    
    public static int ToTxChannel(this IGrouping<int, (string oid, int id, ISnmpData data)> g)
    {
        var value = g.FirstOrDefault(v => v.oid == rsuSRMTxChannel);
        return value.data.ToTxChannel();
    }

    public static int ToTxChannel(this ISnmpData value)
    {
        return value.ToInt();
    }
    
    public static TimeSpan ToTxInterval(this IGrouping<int, (string oid, int id, ISnmpData data)> g)
    {
        var value = g.FirstOrDefault(v => v.oid == rsuSRMTxInterval);
        return value.data.ToTxInterval();
    }

    public static TimeSpan ToTxInterval(this ISnmpData value)
    {
        return TimeSpan.FromMilliseconds(value.ToInt());
    }
    
    public static DateTime ToDeliveryStart(this IGrouping<int, (string oid, int id, ISnmpData data)> g)
    {
        var value = g.FirstOrDefault(v => v.oid == rsuSRMDeliveryStart);
        return value.data.ToDateTime();
    }

    public static DateTime ToDeliveryStop(this IGrouping<int, (string oid, int id, ISnmpData data)> g)
    {
        var value = g.FirstOrDefault(v => v.oid == rsuSRMDeliveryStop);
        return value.data.ToDateTime();
    }

    public static string ToPayload(this IGrouping<int, (string oid, int id, ISnmpData data)> g)
    {
        var value = g.FirstOrDefault(v => v.oid == rsuSRMPayload);
        return value.data.ToPayload();
    }

    public static string ToPayload(this ISnmpData value)
    {
        return value.ToHexString();
    }
    
    public static bool ToEnable(this IGrouping<int, (string oid, int id, ISnmpData data)> g)
    {
        var value = g.FirstOrDefault(v => v.oid == rsuSRMTxChannel);
        return value.data.ToEnable();
    }

    public static bool ToEnable(this ISnmpData value)
    {
        return value.ToInt() == 1;
    }
    
    public static DateTime ToDateTime(this ISnmpData value)
    {
        return value.ToHexString().ToDateTime();
    }

    public static int ToInt(this ISnmpData data)
    {
        return (((data) as Integer32)!).ToInt32();
    }

    public static string ToHexString(this ISnmpData data)
    {
        return ((OctetString) data).ToHexString();
    }
    
    public static List<List<T>> Chunk<T>(this List<T> list, int chunkSize)
    {
        if (chunkSize <= 0)
        {
            throw new ArgumentException("Chunk size should be greater than 0.");
        }

        var chunkedList = new List<List<T>>();

        for (int i = 0; i < list.Count; i += chunkSize)
        {
            var chunk = list.GetRange(i, Math.Min(chunkSize, list.Count - i));
            chunkedList.Add(chunk);
        }

        return chunkedList;
    }

}