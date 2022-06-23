using System;

namespace Puzzle.Compound.Common.Extensions
{
    public static class TimeExtensions
    {
        public static int ToUnixTime(this DateTime input)
        {
            return (int)input.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
