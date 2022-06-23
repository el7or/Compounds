using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Common.Helpers
{
    public class NumberHelpers
    {
        public static string GenerateRandomNumber()
        {
            Random generator = new Random();
            return generator.Next(1001, 10000).ToString("D4");
        }

        public static string GenerateUniqueRandomNumber(List<string> list)
        {
            var rndNo = GenerateRandomNumber();
            while(list.Contains(rndNo))
                rndNo = GenerateRandomNumber();
            return rndNo;
        }
    }
}
