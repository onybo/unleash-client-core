using System.Collections.Generic;
using System.Linq;

namespace Olav.Unleash.Strategy
{
    public sealed class UserWithIdStrategy : Strategy 
    {
        private const string PARAM = "userIds";
        private const string STRATEGY_NAME = "userWithId";

        public override string Name => STRATEGY_NAME;

        public override bool IsEnabled(Dictionary<string, string> parameters) => false;

        public override bool IsEnabled(Dictionary<string, string> parameters, UnleashContext context)
        {
            return parameters.ContainsKey(PARAM) ?
                parameters[PARAM]
                    .Split(',')
                    .Select(n => n.Trim())
                    .Any(userId => userId == context.UserId)
                    :
                false;
        }
    }
}