namespace TollFeeCalculator
{
    public static class HolidayHandler
    {
        private static readonly int[] TollFreeMonths = { 7 }; // July

        public static HashSet<(int Month, int Day)> GetTollFreeDates(int year)
        {
            var dates = new HashSet<(int Month, int Day)>
            {
                (1, 1),   // Nyårsdagen
                (1, 6),   // Trettondedag jul
                (5, 1),   // Första maj
                (6, 6),   // Nationaldagen
                (12, 25), // Juldagen
                (12, 26)  // Annandag jul
            };

            // Påskdagen: Rätt komplex att räkna ut (se computum algoritm easter). Därmed tas ett godtyckligt datum.
            var easter = new DateOnly(year, 4, 5);

            // Långfredagen + dagen före
            var goodFriday = easter.AddDays(-2);
            dates.Add((goodFriday.Month, goodFriday.Day));
            dates.Add((goodFriday.AddDays(-1).Month, goodFriday.AddDays(-1).Day));

            // Annandag påsk + dagen före
            var easterMonday = easter.AddDays(1);
            dates.Add((easterMonday.Month, easterMonday.Day));
            dates.Add((easter.Month, easter.Day));

            // Kristi himmelsfärdsdag + dagen före
            var ascension = easter.AddDays(39);
            dates.Add((ascension.Month, ascension.Day));
            dates.Add((ascension.AddDays(-1).Month, ascension.AddDays(-1).Day));

            // Midsommardagen
            var midsummer = new DateOnly(year, 6, 20);
            while (midsummer.DayOfWeek != DayOfWeek.Saturday)
                midsummer = midsummer.AddDays(1);

            dates.Add((midsummer.Month, midsummer.Day));
            dates.Add((midsummer.AddDays(-1).Month, midsummer.AddDays(-1).Day));

            // Alla helgons dag
            var allSaints = new DateOnly(year, 10, 31);
            while (allSaints.DayOfWeek != DayOfWeek.Saturday)
                allSaints = allSaints.AddDays(1);

            dates.Add((allSaints.Month, allSaints.Day));
            dates.Add((allSaints.AddDays(-1).Month, allSaints.AddDays(-1).Day));

            // Dagen före de fasta helgdagarna
            foreach (var (month, day) in dates.ToArray())
            {
                var date = new DateOnly(year, month, day);
                var previous = date.AddDays(-1);

                dates.Add((previous.Month, previous.Day));
            }

            return dates;
        }

        public static bool IsTollFreeDate(DateTime date)
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return true;

            if (TollFreeMonths.Contains(date.Month))
                return true;

            var tollFree = GetTollFreeDates(date.Year);
            return tollFree.Contains((date.Month, date.Day));
        }
    }
}