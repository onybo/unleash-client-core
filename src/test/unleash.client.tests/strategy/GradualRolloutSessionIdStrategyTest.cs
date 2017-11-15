using System;
using System.Collections.Generic;
using Olav.Unleash.Strategy;
using Xunit;

namespace Olav.Unleash.Client.Tests.Strategy
{

    public class GradualRolloutSessionIdStrategyTest
    {
        private const int SEED = 892350154;
        private const long MIN = 10000000L;
        private const long MAX = 9999999999L;

        private Random rand = new Random(SEED);
        private int[] percentages = {1, 2, 5, 10, 25, 50, 90, 99, 100};

        [Fact]
        public void should_have_a_name()
        {
            var gradualRolloutStrategy = new GradualRolloutSessionIdStrategy();
            Assert.Equal("gradualRolloutSessionId", gradualRolloutStrategy.Name);
        }

        [Fact]
        public void should_require_context()
        {
            var gradualRolloutStrategy = new GradualRolloutSessionIdStrategy();
            Assert.False(gradualRolloutStrategy.IsEnabled(new Dictionary<string, string>()));
        }

        [Fact]
        public void should_be_disabled_when_missing_user_id()
        {
            var context = UnleashContext.CreateBuilder().Build();
            var gradualRolloutStrategy = new GradualRolloutSessionIdStrategy();

            Assert.False(gradualRolloutStrategy.IsEnabled(new Dictionary<string, string>(), context));
        }

        [Fact]
        public void should_have_same_result_for_multiple_executions()
        {
            var context = UnleashContext.CreateBuilder().SessionId("1574576830").Build();
            var gradualRolloutStrategy = new GradualRolloutSessionIdStrategy();

            var parms = BuildParams(1, "innfinn");
            bool firstRunResult = gradualRolloutStrategy.IsEnabled(parms, context);

            for (int i = 0; i < 10; i++)
            {
                bool subsequentRunResult = gradualRolloutStrategy.IsEnabled(parms, context);
                Assert.Equal(
                        subsequentRunResult,
                        firstRunResult);
            }
        }

        [Fact]
        public void should_be_enabled_when_using_100percent_rollout()
        {
            var context = UnleashContext.CreateBuilder().SessionId("1574576830").Build();
            var gradualRolloutStrategy = new GradualRolloutSessionIdStrategy();

            var parms = BuildParams(100, "innfinn");
            var result = gradualRolloutStrategy.IsEnabled(parms, context);

            Assert.True(result);
        }

        [Fact]
        public void should_not_be_enabled_when_0percent_rollout()
        {
            var context = UnleashContext.CreateBuilder().SessionId("1574576830").Build();
            var gradualRolloutStrategy = new GradualRolloutSessionIdStrategy();

            var parms = BuildParams(0, "innfinn");
            var actual = gradualRolloutStrategy.IsEnabled(parms, context);

            Assert.False(actual); //"should not be enabled when 0% rollout",
        }

        [Fact]
        public void should_be_enabled_above_minimum_percentage()
        {
            const string sessionId = "1574576830";
            const string groupId = "";
            var minimumPercentage = StrategyUtils.GetNormalizedNumber(sessionId, groupId);

            var context = UnleashContext.CreateBuilder().SessionId(sessionId).Build();

            var gradualRolloutStrategy = new GradualRolloutSessionIdStrategy();

            for (int p = minimumPercentage; p <= 100; p++)
            {
                var parms = BuildParams(p, groupId);
                var actual = gradualRolloutStrategy.IsEnabled(parms, context);
                Assert.True(actual); //"should be enabled when " + p + "% rollout"
            }
        }

        [Fact(Skip = "Intended for manual execution")]
        public void generateReportForListOfLoginIDs()
        {
            const int numberOfIDs = 200000;

            foreach (var percentage in percentages)
            {
                var numberOfEnabledUsers = CheckRandomLoginIDs(numberOfIDs, percentage);
                var p = ((double)numberOfEnabledUsers / (double)numberOfIDs) * 100.0;
                //System.out.println("Testing " + percentage + "% --> " + numberOfEnabledUsers + " of " + numberOfIDs + " got new feature (" + p + "%)");
            }
        }

        private int CheckRandomLoginIDs(int numberOfIDs, int percentage)
        {
            int numberOfEnabledUsers = 0;
            for (int i = 0; i < numberOfIDs; i++)
            {
                var sessionId = GetRandomLoginId();
                var context = UnleashContext.CreateBuilder().SessionId(sessionId.ToString()).Build();

                var gradualRolloutStrategy = new GradualRolloutSessionIdStrategy();

                var parms = BuildParams(percentage, "");
                var enabled = gradualRolloutStrategy.IsEnabled(parms, context);
                if (enabled)
                {
                    numberOfEnabledUsers++;
                }
            }
            return numberOfEnabledUsers;
        }

        private Dictionary<string, string> BuildParams(int percentage, string groupId)
        {
            return new Dictionary<string, string>
            {
                {GradualRolloutSessionIdStrategy.PERCENTAGE, percentage.ToString()},
                {GradualRolloutSessionIdStrategy.GROUP_ID, groupId}
            };            
        }

        private long GetRandomLoginId()
        {
            var buf = new byte[8];
            rand.NextBytes(buf);
            var longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (MAX - MIN)) + MIN);
        }
    }
}