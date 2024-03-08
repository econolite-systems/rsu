// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using FluentAssertions;

namespace Econolite.Ode.Models.Rsu.Status;

public class StoreRepeatMessageExtensionsTests
{
    [Theory]
    [InlineData("0x07E7031A", 2023, 3, 26)]
    [InlineData("0x07E7031A0A", 2023, 3, 26, 10)]
    [InlineData("0x07E7031A0A1E", 2023, 3, 26, 10, 30)]
    [InlineData("07E7031A", 2023, 3, 26)]
    [InlineData("07E7031A0A", 2023, 3, 26, 10)]
    [InlineData("07E7031A0A1E", 2023, 3, 26, 10, 30)]
    public void ToDateTime_Should_Parse_HexString_Correctly(string hex, int year, int month, int day, int hour = 0, int minute = 0)
    {
        // Act
        DateTime result = hex.ToDateTime();

        // Assert
        result.Year.Should().Be(year);
        result.Month.Should().Be(month);
        result.Day.Should().Be(day);
        result.Hour.Should().Be(hour);
        result.Minute.Should().Be(minute);
        result.Second.Should().Be(0);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Theory]
    [InlineData(2023, 3, 26, 0, 0, "07E7031A0000")]
    [InlineData(2023, 3, 26, 10, 0, "07E7031A0A00")]
    [InlineData(2023, 3, 26, 10, 30, "07E7031A0A1E")]
    public void ToHexString_Should_Convert_DateTime_To_HexString(int year, int month, int day, int hour, int minute, string expectedHex)
    {
        // Arrange
        DateTime time = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);

        // Act
        string result = time.ToHexString();

        // Assert
        result.Should().Be(expectedHex);
    }
}
