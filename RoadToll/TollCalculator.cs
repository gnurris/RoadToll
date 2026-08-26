namespace TollFeeCalculator
{
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
            if (dates == null || dates.Length == 0 || IsTollFreeVehicle(vehicle))
            {
                return 0;
            }

            // Local helper that uses TollWindows directly
            int FeeFor(DateTime dt)
            {
                if (HolidayHandler.IsTollFreeDate(dt)) return 0;
                TimeOnly t = TimeOnly.FromDateTime(dt);
                foreach (var w in TollWindows)
                {
                    if (t >= w.Start && t <= w.End) return w.Fee;
                }
                return 0;
            }

            Array.Sort(dates); // ensure chronological order
            int totalFee = 0;

            DateTime intervalStart = dates[0];
            int intervalMaxFee = FeeFor(intervalStart);

            for (int i = 1; i < dates.Length; i++)
            {
                DateTime date = dates[i];
                int fee = FeeFor(date);
                var diff = date - intervalStart;

                if (diff.TotalMinutes <= 60)
                {
                    if (fee > intervalMaxFee)
                        intervalMaxFee = fee;
                }
                else
                {
                    totalFee += intervalMaxFee;
                    intervalStart = date;
                    intervalMaxFee = fee;
                }
            }


            totalFee += intervalMaxFee;
            return totalFee > 60 ? 60 : totalFee;
        }

        private bool IsTollFreeVehicle(Vehicle vehicle)
        {
            // Use the new Vehicle extension method to determine toll-free status.
            if (vehicle == null) return false;
            return vehicle.IsTollFree();
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
    }
}
