using System.Collections.Generic;
using System.Linq;

namespace Olav.Unleash.Strategy
{
    public sealed class RemoteAddressStrategy : Strategy 
    {
        private const string PARAM = "IPs";
        private const string STRATEGY_NAME = "remoteAddress";

        public override string Name => STRATEGY_NAME;

        public override bool IsEnabled(Dictionary<string, string> parameters) => false;

        public override bool IsEnabled(Dictionary<string, string> parameters, UnleashContext context) =>
            parameters.ContainsKey(PARAM) ?
                parameters[PARAM]
                    .Split(',')
                    .Select(n => n.Trim())
                    .Contains(context.RemoteAddress) :
                false;
    }
}
