using System.Collections.Generic;

namespace Olav.Unleash.Strategy
{
    public sealed class DefaultStrategy : Strategy 
    {
        private const string STRATEGY_NAME = "default";
        
        public override string Name => STRATEGY_NAME;

        public override bool IsEnabled(Dictionary<string, string> parameters) => true;
    }
}