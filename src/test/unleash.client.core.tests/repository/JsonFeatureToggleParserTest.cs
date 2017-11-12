using System;
using System.IO;
using System.Linq;
using Olav.Unleash.Repository;
using Xunit;

namespace Olav.Unleash.Client.Core.Tests.Repository
{

    public class JsonFeatureToggleParserTest
    {

        [Fact]
        public void should_deserialize_correctly()
        {
            var content = File.ReadAllText("resources/features-v1.json");
            var toggleCollection = JsonToggleParser.FromJson(content);

            Assert.Equal(toggleCollection.Features.Count(), 3);
            Assert.True(toggleCollection.GetToggle("featureX").IsEnabled);
        }

        [Fact]
        public void should_deserialize_correctly_version0() 
        {
            var content = File.ReadAllText("resources/features-v0.json");
            var toggleCollection = JsonToggleParser.FromJson(content);

            Assert.Equal(toggleCollection.Features.Count(), 3);
            Assert.True(toggleCollection.GetToggle("featureX").IsEnabled);
        }

        [Fact]        
        public void should_deserialize_with_one_strategy()
        {
            var content = File.ReadAllText("resources/features-v1.json");
            var toggleCollection = JsonToggleParser.FromJson(content);
            var featureY = toggleCollection.GetToggle("featureY");

            Assert.Equal(featureY.Strategies.Count(), 1);
            Assert.Equal(featureY.Strategies.First().Name, "baz");
            Assert.Equal(featureY.Strategies.First().Parameters["foo"], "bar");
        }

        [Fact]
        public void should_deserialize_with_one_strategy_version0()
        {
            var content = File.ReadAllText("resources/features-v0.json");
            var toggleCollection = JsonToggleParser.FromJson(content);
            var featureY = toggleCollection.GetToggle("featureY");

            Assert.False(featureY.IsEnabled);
            Assert.Equal(featureY.Strategies.Count(), 1);
            Assert.Equal(featureY.Strategies.First().Name, "baz");
            Assert.Equal(featureY.Strategies.First().Parameters["foo"], "bar");
        }

        // @Test
        // public void should_deserialize_with_multiple_strategies() throws IOException {
        //     Reader content = getFileReader("/features-v1.json");
        //     ToggleCollection toggleCollection = JsonToggleParser.fromJson(content);
        //     FeatureToggle feature = toggleCollection.getToggle("featureZ");

        //     assertThat(feature.getStrategies().size(), is(2));
        //     assertThat(feature.getStrategies().get(1).getName(), is("hola"));
        //     assertThat(feature.getStrategies().get(1).getParameters().get("name"), is("val"));
        // }

        // @Test
        // public void should_throw() throws IOException {
        //     Reader content = getFileReader("/empty.json");
        //     try {
        //         JsonToggleParser.fromJson(content);
        //     } catch (IllegalStateException e) {
        //         assertTrue("Expected IllegalStateException", e instanceof IllegalStateException);
        //     }
        // }

        // @Test
        // public void should_throw_on_mission_features() throws IOException {
        //     Reader content = getFileReader("/empty-v1.json");
        //     try {
        //         JsonToggleParser.fromJson(content);
        //     } catch (IllegalStateException e) {
        //         assertTrue("Expected IllegalStateException", e instanceof IllegalStateException);
        //     }
        // }

        // @Test
        // public void should_deserialize_empty_litst_of_toggles() throws IOException {
        //     Reader content = getFileReader("/features-v1-empty.json");
        //     ToggleCollection toggleCollection = JsonToggleParser.fromJson(content);

        //     assertThat(toggleCollection.getFeatures().size(), is(0));
        // }

        // @Test
        // public void should_deserialize_old_format() throws IOException {
        //     Reader content = getFileReader("/features-v0.json");
        //     ToggleCollection toggleCollection = JsonToggleParser.fromJson(content);
        //     FeatureToggle featureY = toggleCollection.getToggle("featureY");

        //     assertThat(toggleCollection.getFeatures().size(), is(3));
        //     assertThat(featureY.getStrategies().size(), is(1));
        //     assertThat(featureY.getStrategies().get(0).getName(), is("baz"));
        //     assertThat(featureY.getStrategies().get(0).getParameters().get("foo"), is("bar"));
        // }

        // private Reader getFileReader(string filename) throws IOException {
        //     InputStream in = this.getClass().getResourceAsStream(filename);
        //     InputStreamReader reader = new InputStreamReader(in);
        //     return new BufferedReader(reader);
        // }

    }
}