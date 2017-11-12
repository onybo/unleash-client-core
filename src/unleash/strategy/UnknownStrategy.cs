using System.Collections.Generic;

namespace Olav.Unleash.Strategy
{
    public sealed class UnknownStrategy : Strategy 
    {
        private const string STRATEGY_NAME = "unknown";

        public override string Name => STRATEGY_NAME;

        public override bool IsEnabled(Dictionary<string, string> parameters) => false;
    }
}