using System;
using System.Globalization;
using TollFeeCalculator;

public class TollCalculator
{

    /**
     * Calculate the total toll fee for one day
     *
     * @param vehicle - the vehicle
     * @param dates   - date and time of all passes on one day
     * @return - the total toll fee for that day
     */

    public int GetTollFee(Vehicle vehicle, DateTime[] dates)
    {
        if (dates.Length == 0)
        {
            return 0;
        }
        DateTime intervalStart = dates[0];
        int totalFee = 0;
        foreach (DateTime date in dates)
        {
            int nextFee = GetTollFee(date, vehicle);
            int tempFee = GetTollFee(intervalStart, vehicle);

            long diffInMillies = date.Millisecond - intervalStart.Millisecond;
            long minutes = diffInMillies/1000/60;

            if (minutes <= 60)
            {
                if (totalFee > 0) totalFee -= tempFee;
                if (nextFee >= tempFee) tempFee = nextFee;
                totalFee += tempFee;
            }
            else
            {
                totalFee += nextFee;
            }
        }
        if (totalFee > 60) totalFee = 60;
        return totalFee;
    }

    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        if (vehicle == null) return false;
        string vehicleType = vehicle.GetVehicleType();
        // Return true if vehicleType matches any TollFreeVehicles
        return Enum.TryParse<TollFreeVehicles>(vehicleType, ignoreCase: true, out _);
    }

    private readonly struct TollWindow
    {
        public TimeOnly Start { get; }
        public TimeOnly End { get; }
        public int Fee { get; }

        public TollWindow(TimeOnly start, TimeOnly end, int fee)
        {
            Start = start;
            End = end;
            Fee = fee;
        }
    }

    private static readonly TollWindow[] TollWindows = new[]
    {
        new TollWindow(new TimeOnly(6, 0),  new TimeOnly(6, 29),  8),
        new TollWindow(new TimeOnly(6, 30), new TimeOnly(6, 59),  13),
        new TollWindow(new TimeOnly(7, 0),  new TimeOnly(7, 59),  18),
        new TollWindow(new TimeOnly(8, 0),  new TimeOnly(8, 29),  13),
        new TollWindow(new TimeOnly(8, 30), new TimeOnly(14, 59), 8),
        new TollWindow(new TimeOnly(15, 0), new TimeOnly(15, 29), 13),
        new TollWindow(new TimeOnly(15, 30),new TimeOnly(16, 59), 18),
        new TollWindow(new TimeOnly(17, 0), new TimeOnly(17, 59), 13),
        new TollWindow(new TimeOnly(18, 0), new TimeOnly(18, 29), 8)
    };

    public int GetTollFee(DateTime date, Vehicle vehicle)
    {
        if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

        TimeOnly timeOfDay = TimeOnly.FromDateTime(date);

        // Lookup fee by scanning configured windows (small fixed list => simple and readable).
        foreach (var w in TollWindows)
        {
            if (timeOfDay >= w.Start && timeOfDay <= w.End)
                return w.Fee;
        }

        return 0;
    }

    private Boolean IsTollFreeDate(DateTime date)
    {
        int year = date.Year;
        int month = date.Month;
        int day = date.Day;

        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

        if (month == 1 && day == 1 ||
            month == 3 && (day == 28 || day == 29) ||
            month == 4 && (day == 1 || day == 30) ||
            month == 5 && (day == 1 || day == 8 || day == 9) ||
            month == 6 && (day == 5 || day == 6 || day == 21) ||
            month == 7 ||
            month == 11 && day == 1 ||
            month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
        {
            return true;
        }
        return false;
    }

    private enum TollFreeVehicles
    {
        Motorbike = 0,
        Tractor = 1,
        Emergency = 2,
        Diplomat = 3,
        Foreign = 4,
        Military = 5
    }
}