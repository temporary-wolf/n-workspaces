namespace TollCalculators;

public static class NewTollCalculator
{
    
    public static readonly TimeZoneInfo SwedenTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");
    public static Int32 GetDailyTollFee(VehicleTypes vehicleType, DateTimeOffset[] dailyDates)
    {
        return 0;
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