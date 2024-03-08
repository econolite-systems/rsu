// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Models.Rsu.Status;

public static class StoreRepeatMessageExtensions
{
    public static DateTime ToDateTime(this string hex)
    {
        var sanitized = hex.Replace("0x", "");
        var length = sanitized.Length;
        var year = int.Parse(sanitized.Substring(0, 4), System.Globalization.NumberStyles.HexNumber);
        var month = int.Parse(sanitized.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        var day = int.Parse(sanitized.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        var hour = int.Parse(length >= 10 ? sanitized.Substring(8, 2) : "00", System.Globalization.NumberStyles.HexNumber);
        var minute = int.Parse(length >= 12 ? sanitized.Substring(10, 2) : "00", System.Globalization.NumberStyles.HexNumber);
        var second = 0;

        return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
    }
    
    public static string ToHexString(this DateTime time)
    {
        var year = time.Year.ToString("X4");
        var month = time.Month.ToString("X2");
        var day = time.Day.ToString("X2");
        var hour = time.Hour.ToString("X2");
        var minute = time.Minute.ToString("X2");

        return year + month + day + hour + minute;
    }
}