using System;

namespace NuPattern.Extensibility.Serialization.Json
{
    internal class MathUtils
    {
        public static int IntLength(int i)
        {
            if (i < 0)
                throw new ArgumentOutOfRangeException();

            if (i == 0)
                return 1;

            return (int)Math.Floor(Math.Log10(i)) + 1;
        }

        public static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 48);
            }
            return (char)((n - 10) + 97);
        }

        public static int? Min(int? val1, int? val2)
        {
            if (val1 == null)
                return val2;
            if (val2 == null)
                return val1;

            return Math.Min(val1.Value, val2.Value);
        }

        public static int? Max(int? val1, int? val2)
        {
            if (val1 == null)
                return val2;
            if (val2 == null)
                return val1;

            return Math.Max(val1.Value, val2.Value);
        }

        public static double? Max(double? val1, double? val2)
        {
            if (val1 == null)
                return val2;
            if (val2 == null)
                return val1;

            return Math.Max(val1.Value, val2.Value);
        }

        public static bool ApproxEquals(double d1, double d2)
        {
            // are values equal to within 6 (or so) digits of precision?
            return Math.Abs(d1 - d2) < (Math.Abs(d1) * 1e-6);
        }
    }
}