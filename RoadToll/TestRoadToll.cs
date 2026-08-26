namespace TollFeeCalculator
{
    internal class TestRoadToll
    {
        // Test date components used throughout tests (full date represents a regular work day)
        private const int TestYear = 2013;
        private const int TestMonth = 1;
        private const int TestDay = 2;

        private static int _passed;
        private static int _failed;

        private static void Main()
        {
            TestTollIntervals();
            TestTollFreeDates();
            TestTollFreeVehicles();
            TestSixtyMinuteRule();
            TestDailyMaximum();
            TestEdgeCases();

            Console.WriteLine();
            Console.WriteLine($"Passed: {_passed}");
            Console.WriteLine($"Failed: {_failed}");

            if (_failed > 0)
                Environment.ExitCode = 1;
        }

        private static void TestTollIntervals()
        {
            Car car = new Car();
            Console.WriteLine("Toll intervals (basic)");
            AssertFee(8, new DateTime(TestYear, TestMonth, TestDay, 6, 0, 0), car);         // default vehicle: Car
            AssertFee(13, new DateTime(TestYear, TestMonth, TestDay, 6, 30, 0), car);
            AssertFee(18, new DateTime(TestYear, TestMonth, TestDay, 7, 0, 0), car);
            AssertFee(13, new DateTime(TestYear, TestMonth, TestDay, 8, 0, 0), car);
            AssertFee(8, new DateTime(TestYear, TestMonth, TestDay, 9, 0, 0), car);
            AssertFee(18, new DateTime(TestYear, TestMonth, TestDay, 15, 30, 0), car);
            AssertFee(0, new DateTime(TestYear, TestMonth, TestDay, 18, 30, 0), car);
        }

        private static void TestTollFreeDates()
        {
            Console.WriteLine("Toll-free dates (basic)");
            Car car = new Car();

            // Saturday
            AssertFee(0, new DateTime(2013, 1, 5, 7, 0, 0), car);

            // July (any day in July is toll-free)
            AssertFee(0, new DateTime(2013, 7, 15, 10, 0, 0), car);

            // Holiday (New Year's Day)
            AssertFee(0, new DateTime(2013, 1, 1, 7, 0, 0), car);

            // Day before holiday
            AssertFee(0, new DateTime(2013, 12, 24, 7, 0, 0), car);
        }

        private static void TestTollFreeVehicles()
        {
            Console.WriteLine("Toll-free vehicles (basic)");
            Motorbike mb = new Motorbike();
            // Motorbike explicitly in domain model
            AssertFee(0, new DateTime(TestYear, TestMonth, TestDay, 6, 0, 0), mb);
            AssertFee(0, new DateTime(TestYear, TestMonth, TestDay, 6, 30, 0), mb);
            AssertFee(0, new DateTime(TestYear, TestMonth, TestDay, 7, 0, 0), mb);
            AssertFee(0, new DateTime(TestYear, TestMonth, TestDay, 8, 0, 0), mb);
            AssertFee(0, new DateTime(TestYear, TestMonth, TestDay, 9, 0, 0), mb);
            AssertFee(0, new DateTime(TestYear, TestMonth, TestDay, 15, 30, 0), mb);
            AssertFee(0, new DateTime(TestYear, TestMonth, TestDay, 18, 30, 0), mb);
        }

        private static void TestSixtyMinuteRule()
        {
            Console.WriteLine("60-minute rule (basic)");

            var calculator = new TollCalculator();
            var car = new Car();

            // Two passages within 60 minutes, only highest fee 
            AssertEqual(
                18,
                calculator.GetTollFee(
                    car,
                    new[]
                    {
                        new DateTime(TestYear, TestMonth, TestDay, 7, 0, 0),
                        new DateTime(TestYear, TestMonth, TestDay, 7, 30, 0)
                    }));
        }

        private static void TestDailyMaximum()
        {
            Console.WriteLine("Daily maximum (basic)");

            var calculator = new TollCalculator();

            var dates = new[]
            {
                new DateTime(TestYear, TestMonth, TestDay, 7, 0, 0),   // 18
                new DateTime(TestYear, TestMonth, TestDay, 9, 0, 0),   // 8
                new DateTime(TestYear, TestMonth, TestDay, 11, 0, 0),  // 8
                new DateTime(TestYear, TestMonth, TestDay, 13, 0, 0),  // 8
                new DateTime(TestYear, TestMonth, TestDay, 15, 0, 0),  // 13
                new DateTime(TestYear, TestMonth, TestDay, 17, 0, 0)   // 13
            };

            // Sum would be 68, but maximum per day is 60
            AssertEqual(60, calculator.GetTollFee(new Car(), dates));
        }

        private static void TestEdgeCases()
        {
            Console.WriteLine("Edge cases (basic)");

            // No passages.
            AssertEqual(
                0,
                new TollCalculator().GetTollFee(
                    new Car(),
                    Array.Empty<DateTime>()));

            // Multiple passages within the same minute -> counted once
            AssertEqual(
                18,
                new TollCalculator().GetTollFee(
                    new Car(),
                    new[]
                    {
                        new DateTime(TestYear, TestMonth, TestDay, 7, 0, 0),
                        new DateTime(TestYear, TestMonth, TestDay, 7, 0, 0)
                    }));
        }

        // Helper funcs
        private static void AssertFee(
            int expected,
            DateTime date,
            Vehicle vehicle)
        {
            // Intentionally do not accept null vehicle here; tests must provide valid vehicles.
            var calculator = new TollCalculator();

            int actual = calculator.GetTollFee(
                vehicle,
                new[] { date });

            string message = $"{date:yyyy-MM-dd HH:mm} expected {expected}, got {actual}";
            AssertEqual(expected, actual, message);
        }

        private static void AssertEqual(
            int expected,
            int actual,
            string? message = null)
        {
            if (expected == actual)
            {
                _passed++;
                return;
            }

            _failed++;

            string failMessage = message != null ? message : $"Expected {expected}, got {actual}";
            Console.WriteLine($"FAIL: {failMessage}");
        }
    }
}