using System;
using System.Collections.Generic;

namespace Olav.Unleash.Strategy
{
    public sealed class GradualRolloutRandomStrategy : Strategy 
    {
        private const string PERCENTAGE = "percentage";
        private const string STRATEGY_NAME = "gradualRolloutRandom";

        private readonly Random _random;

        public GradualRolloutRandomStrategy() => _random = new Random();
        
        public override string Name => STRATEGY_NAME;

        public override bool IsEnabled(Dictionary<string, string> parameters) 
        {
            if (!parameters.ContainsKey(PERCENTAGE)) return false;
            int percentage = StrategyUtils.GetPercentage(parameters[PERCENTAGE]);
            int randomNumber = _random.Next(100) + 1;
            return percentage >= randomNumber;
        }
    }
}