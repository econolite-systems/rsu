// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Globalization;
using Lextm.SharpSnmpLib;

namespace Econolite.Ode.Snmp.Rsu.Rsu;

public static class Snmpv2_TC
{
    public static string ToDateAndTime(ISnmpData data)
        {
            if (data.TypeCode == SnmpType.OctetString)
            {
                var octetString = (OctetString)data;
                var raw = octetString.GetRaw();

                int year = (raw[0] * 256) + raw[1];
                int month = raw[2];
                int day = raw[3];
                int hour = raw[4];
                int minute = raw[5];
                int second = raw[6];
                int decisecond = raw[7];

                if (month < 13 && day < 32 && hour < 24 && second < 61 && decisecond < 10)
                {
                    var local = new DateTime(year, month, day, hour, minute, second, decisecond * 100, DateTimeKind.Utc);
                    if (raw.Length == 11)
                    {
                        var sign = (char)raw[8];
                        int hoursFromUtc = raw[9];
                        int minutesFromUtc = raw[10];
                        if (sign == '+')
                        {
                        }
                        else if (sign == '-')
                        {
                            hoursFromUtc = -1 * hoursFromUtc;
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format("invalid data: {0}", octetString.ToHexString()));
                        }

                        if (sign == '+')
                        {
                        }
                        else if (sign == '-')
                        {
                            minutesFromUtc = -1 * minutesFromUtc;
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format("invalid data: {0}", octetString.ToHexString()));
                        }

                        var offset = new TimeSpan(hoursFromUtc, minutesFromUtc, 0);

                        var timeZones = TimeZoneInfo.GetSystemTimeZones();
                        foreach (var timeZone in timeZones)
                        {
                            if (timeZone.BaseUtcOffset == offset)
                            {
                                return string.Format("{0} {1}", local, timeZone.ToString());
                            }
                        }
                    }

                    return local.ToString(CultureInfo.InvariantCulture);
                }

                throw new InvalidOperationException(string.Format("invalid data: {0}", octetString.ToHexString()));
            }

            throw new InvalidOperationException(string.Format("wrong type: {0}", data.GetType().FullName));
        }
}