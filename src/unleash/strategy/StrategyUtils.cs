using System;

namespace Olav.Unleash.Strategy
{
    public sealed class StrategyUtils 
    {
        private const int ONE_HUNDRED = 100;

        /**
        * Takes to string inputs concat them, produce a hashCode and return a normalized value between 0 and 100;
        *
        * @param identifier
        * @param groupId
        * @return
        */
        public static int GetNormalizedNumber(string identifier, string groupId) 
        {
            int hashCode = Math.Abs((groupId + ':' + identifier).GetHashCode());
            return hashCode % ONE_HUNDRED + 1;
        }

        /**
        * Takes a numeric string value and converts it to a integer between 0 and 100.
        *
        * returns 0 if the string is not numeric.
        *
        * @param percentage - A numeric string value
        * @return a integer between 0 and 100
        */
        public static int GetPercentage(string percentage) 
        {
            int value;
            var result = int.TryParse(percentage, out value);
            if (value > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage), "value should not exceed 100");
            return value;
        }
    }
}