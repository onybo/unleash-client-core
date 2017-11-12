using System.Collections.Generic;

namespace Olav.Unleash.Strategy
{
// import java.util.Map;
// import java.util.Optional;

// import no.finn.unleash.UnleashContext;

/**
 * Implements a gradual roll-out strategy based on session id.
 *
 * Using this strategy you can target only users bound to a session and gradually expose your
 * feature to higher percentage of the logged in user.
 *
 * This strategy takes two parameters:
 *  - percentage :  a number between 0 and 100. The percentage you want to enable the feature for.
 *  - groupId :     a groupId used for rolling out the feature. By using the same groupId for different
 *                  toggles you can correlate the user experience across toggles.
 *
 */
    public sealed class GradualRolloutSessionIdStrategy : Strategy
    {
        private const string PERCENTAGE = "percentage";
        private const string GROUP_ID = "groupId";

        private const string NAME = "gradualRolloutSessionId";

        public override string Name => NAME;

        public override bool IsEnabled(Dictionary<string, string> parameters) => false;

        public override bool IsEnabled(Dictionary<string, string> parameters, UnleashContext context)
        {
            var sessionId = context.SessionId;

            if(string.IsNullOrWhiteSpace(sessionId)) 
                return false;

            var percentage = StrategyUtils.GetPercentage(parameters[PERCENTAGE]);
            var groupId = parameters.ContainsKey(GROUP_ID) ? parameters[GROUP_ID] : "";

            var normalizedSessionId = StrategyUtils.GetNormalizedNumber(sessionId, groupId);

            return percentage > 0 && normalizedSessionId <= percentage;
        }
    }
}
