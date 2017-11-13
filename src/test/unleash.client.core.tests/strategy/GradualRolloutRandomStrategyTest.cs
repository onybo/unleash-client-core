using System;
using System.Collections.Generic;
using Olav.Unleash.Strategy;
using Xunit;

namespace Olav.Unleash.Client.Core.Tests.Repository
{

    public class GradualRolloutRandomStrategyTest
    {

        private static GradualRolloutRandomStrategy gradualRolloutRandomStrategy;

        public GradualRolloutRandomStrategyTest()
        {
            gradualRolloutRandomStrategy = new GradualRolloutRandomStrategy();
        }

        [Fact]
        public void should_not_be_enabled_when_percentage_not_set()
        {
            var parameters = new Dictionary<string, string>();

            var enabled = gradualRolloutRandomStrategy.IsEnabled(parameters);

            Assert.False(enabled);
        }

        [Fact]
        public void should_not_be_enabled_when_percentage_is_not_a_not_a_number()
        {
            var parameters = new Dictionary<string, string>() {{
                "percentage", "foo"
            }};

            var enabled = gradualRolloutRandomStrategy.IsEnabled(parameters);

            Assert.False(enabled);
        }

        [Fact]
        public void should_not_be_enabled_when_percentage_is_not_a_not_a_valid_percentage_value()
        {
            var parameters = new Dictionary<string, string>() {{
                "percentage", "ab"
            }};

            var enabled = gradualRolloutRandomStrategy.IsEnabled(parameters);

            Assert.False(enabled);
        }

        [Fact]
        public void should_never_be_enabled_when_0_percent()
        {
            var parameters = new Dictionary<string, string>() {{
                "percentage", "0"
            }};

            for (int i = 0; i < 1000; i++) 
            {
                var enabled = gradualRolloutRandomStrategy.IsEnabled(parameters);
                Assert.False(enabled);
            }
        }

        [Fact]
        public void should_always_be_enabled_when_100_percent()
        {
            var parameters = new Dictionary<string, string>() {{
                "percentage", "100"
            }};

            for (int i = 0; i <= 100; i++) {
                var enabled = gradualRolloutRandomStrategy.IsEnabled(parameters);
                Assert.True(enabled);
            }
        }

        [Fact]
        public void should_diverage_at_most_with_one_percent_point()
        {
            int percentage = 55;
            var min= percentage - 1;
            var max = percentage + 1;

            var parameters = new Dictionary<string, string>() {{
                "percentage", ""+percentage
            }};

            var rounds = 20000;
            var countEnabled = 0;

            for (int i = 0; i < rounds; i++) 
            {
                var enabled = gradualRolloutRandomStrategy.IsEnabled(parameters);
                if(enabled) 
                {
                    countEnabled = countEnabled + 1;
                }
            }

            var measuredPercentage = Math.Round(((double) countEnabled / rounds * 100));

            Assert.True(measuredPercentage >= min);
            Assert.True(measuredPercentage <= max);
        }
    }
}