using Olav.Unleash.Strategy;
using Xunit;

namespace Olav.Unleash.Client.Tests.Util
{
    public class StrategyUtilsTest 
    {
        [Fact]
        public void normalized_values_are_the_same_across_node_java_go_and_dotnet_clients() 
        {
            Assert.Equal(73, StrategyUtils.GetNormalizedNumber("123", "gr1"));
            Assert.Equal(25, StrategyUtils.GetNormalizedNumber("999", "groupX"));
        }
    } 
}