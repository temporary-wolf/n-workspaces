using NewTollCalculator;
using NewTollCalculator.OldTollCalculator;

namespace Tests;

public class CalculatorTests
{
    [Theory]
    [MemberData(nameof(TestData))]
    public void TestOldCalculator_ShouldReturnExpectedResult(DateTime[] dates, int expected)
    {
        // Arrange
        var processor = new TollCalculator();

        // Act
        int result = processor.GetTollFee(new Car(), dates);

        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [MemberData(nameof(TestDataDateTimeOffset))]
    public void TestNewCalculator_ShouldReturnExpectedResult(DateTimeOffset[] dates, int expected)
    {
        // Act
        int result = TollCalculators.NewTollCalculator.GetDailyTollFee(TollCalculators.NewTollCalculator.VehicleTypes.Car, dates);
        
        // Assert
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> TestData()
    {
        return new List<object[]>
        {
            new object[] { GenerateDayData(2024, 1, 1), 31 },
            new object[] { GenerateDayData(2024, 2, 4), 0 },
            new object[] { GenerateDayData(2024, 3, 10), 0 }, // Sunday
            new object[] { GenerateDayData(2024, 4, 6), 0 },  // Saturday
            new object[] { GenerateDayData(2024, 5, 15), 31 },
            new object[] { GenerateDayData(2024, 6, 1), 0 },  // Saturday
            new object[] { GenerateDayData(2024, 7, 21), 0 }, // Sunday
            new object[] { GenerateDayData(2024, 8, 18), 0 }, // Sunday
            new object[] { GenerateDayData(2024, 9, 9), 31 },
            new object[] { GenerateDayData(2024, 10, 5), 0 }, // Saturday
            new object[] { GenerateDayData(2024, 11, 11), 31 },
            new object[] { GenerateDayData(2024, 12, 25), 31 }, // Christmas Day (Wednesday in 2024)
            new object[] { GenerateDayData(2024, 1, 14), 0 }, // Sunday
            new object[] { GenerateDayData(2024, 2, 17), 0}, // Saturday
            new object[] { GenerateDayData(2024, 3, 18), 31 },
            new object[] { GenerateDayData(2024, 4, 20), 0 }, // Saturday
            new object[] { GenerateDayData(2024, 5, 19), 0 }, // Sunday
            new object[] { GenerateDayData(2024, 6, 23), 0 }, // Sunday
            new object[] { GenerateDayData(2024, 7, 3), 31 },
            new object[] { GenerateDayData(2024, 8, 24), 0 }  // Saturday
        };
    }
    
    public static IEnumerable<object[]> TestDataDateTimeOffset()
    {
        return new List<object[]>
        {
            new object[]
            { GenerateDayDataDateSwedishTime(), 21 },
 
        };
    }

    private static DateTime[] GenerateDayData(int year, int month, int day, int passages=5)
    {
        var dates = new List<DateTime>();
        var increment = (24 / passages);
        for (int hour = 0; hour < 24; hour+=increment)
        {
            dates.Add(new DateTime(year, month, day, hour, 0, 0));
        }
        return dates.ToArray();
    }
    
    private static DateTimeOffset[] GenerateDayDataDateSwedishTime()
    {
        var dates = new DateTime[] { new(2024, 2, 2, 8, 0, 0), new (2024, 2, 2, 8, 30, 0), new (2024, 2, 2, 12, 0, 0) };
        return dates.Select(date => new DateTimeOffset(date, TollCalculators.NewTollCalculator.SwedenTimeZone.GetUtcOffset(date))).ToArray();
    }
}