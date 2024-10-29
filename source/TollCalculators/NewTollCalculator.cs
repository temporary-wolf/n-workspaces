namespace TollCalculators;

public static class NewTollCalculator
{

    public static readonly TimeZoneInfo SwedenTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");

    public static Int32 GetDailyTollFee(VehicleTypes vehicleType, DateTimeOffset[] dates)
    {
        if (dates.Length == 0)
        {
            return 0;
        }

        if (IsTollFreeVehicle(vehicleType))
        {
            return 0;
        }

        AssertDateTimes(dates);
        AssertAllDatesAreInTheSameDay(dates);

        var date = dates[0];
        if (IsTollFreeDate(date))
        {
            return 0;
        }

        var dailyFee = CalculateDailyPassages(dates);

        return dailyFee;
    }

    private static int CalculateDailyPassages(DateTimeOffset[] dates)
    {
        var sortedDates = dates
            .Order()
            .SkipWhile(date => GetTollFeeForDate(date) == 0)
            .ToList();
        if (sortedDates.Count == 0)
        {
            return 0;
        }

        var totalCost = 0;
        var currentIntervalStart = sortedDates[0];
        var currentIntervalCost = GetTollFeeForDate(sortedDates[0]);
        foreach (var date in sortedDates.Skip(1))
        {
            var cost = GetTollFeeForDate(date);
            if (cost == 0)
            {
                continue;
            }
            var timeSinceLastTimeWindow = date - currentIntervalStart;
            if (timeSinceLastTimeWindow >= TimeSpan.FromMinutes(60))
            {
                totalCost += currentIntervalCost;
                currentIntervalStart = date;
                currentIntervalCost = cost;
            }
            else
            {
                currentIntervalCost = Math.Max(currentIntervalCost, cost);
            }
        }
        // handle last time interval
        totalCost += currentIntervalCost;
        return Math.Min(totalCost, 60);
    }

    private static int GetTollFeeForDate(DateTimeOffset swedishDateTimeOffset)
    {
        return (swedishDateTimeOffset.Hour, swedishDateTimeOffset.Minute) switch
        {
            (< 6, _) => 0,
            (6, <= 29) => 8,
            (6, >= 30) => 13,
            (7, _) => 18,
            (8, <= 29) => 13,
            (8, >= 30) => 8,
            (>= 9 and <= 14, _) => 8,
            (15, <= 29) => 13,
            (15, >= 30) => 18,
            (16, _) => 18,
            (17, _) => 13,
            (18, <= 29) => 8,
            _ => 0
        };
    }


// Using nager here, but would probably create my own library to check holidays if it was for the government
    private static bool IsTollFreeDate(DateTimeOffset date)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return true;
        }
    
        if (date.Month == 7)
        {
            return true;
        }

        var isPublicHoliday = IsPublicHolidayInSweden(date);
        if (isPublicHoliday)
        {
            return true;
        }

        var isDayBeforePublicHoliday = IsPublicHolidayInSweden(date.Date.AddDays(1));
        if (isDayBeforePublicHoliday)
        {
            return true;
        }
    
        return false;
    }

    // Use some inhouse library or some public packages for this one
    private static bool IsPublicHolidayInSweden(DateTimeOffset date)
    {
        return false;
    }
    

    private static void AssertDateTimes(DateTimeOffset[] dailyDates)
    {
        foreach (var dateTimeOffset in dailyDates)
        {
            AssertSwedenTimeZone(dateTimeOffset);
        }
    }
    
    private static void AssertAllDatesAreInTheSameDay(DateTimeOffset[] dailyDates)
    {
        if (dailyDates.Length < 1)
        {
            return;
        }
        
        var allSameDay = dailyDates.All(date => date.Date == dailyDates[0].Date);
        if (!allSameDay)
        {
            throw new Exception("All daily tolls are not in the same day");
        }
    }

    private static void AssertSwedenTimeZone(DateTimeOffset datetime)
    {
        
        var isSwedishDatetime = datetime.Offset == SwedenTimeZone.GetUtcOffset(datetime);
        if (!isSwedishDatetime)
        {
            throw new Exception($"The time zone '{datetime}' is not a Swedish Time");
        }
    }

    private static bool IsTollFreeVehicle(VehicleTypes vehicleType)
    {
        return vehicleType != VehicleTypes.Car;
    }

    public enum VehicleTypes
    {
        Motorbike = 0,
        Tractor = 1,
        Emergency = 2,
        Diplomat = 3,
        Foreign = 4,
        Military = 5,
        Car = 6,
    }
}
