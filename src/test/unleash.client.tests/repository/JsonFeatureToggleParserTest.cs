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

            Assert.Equal(3, toggleCollection.Features.Count());
            Assert.True(toggleCollection.GetToggle("featureX").IsEnabled);
        }

        [Fact]
        public void should_deserialize_correctly_version0() 
        {
            var content = File.ReadAllText("resources/features-v0.json");
            var toggleCollection = JsonToggleParser.FromJson(content);

            Assert.Equal(3, toggleCollection.Features.Count());
            Assert.True(toggleCollection.GetToggle("featureX").IsEnabled);
        }

        [Fact]        
        public void should_deserialize_with_one_strategy()
        {
            var content = File.ReadAllText("resources/features-v1.json");
            var toggleCollection = JsonToggleParser.FromJson(content);
            var featureY = toggleCollection.GetToggle("featureY");

            Assert.Single(featureY.Strategies);
            Assert.Equal("baz", featureY.Strategies.First().Name);
            Assert.Equal("bar", featureY.Strategies.First().Parameters["foo"]);
        }

        [Fact]
        public void should_deserialize_with_one_strategy_version0()
        {
            var content = File.ReadAllText("resources/features-v0.json");
            var toggleCollection = JsonToggleParser.FromJson(content);
            var featureY = toggleCollection.GetToggle("featureY");

            Assert.False(featureY.IsEnabled);
            Assert.Single(featureY.Strategies);
            Assert.Equal("baz", featureY.Strategies.First().Name);
            Assert.Equal("bar", featureY.Strategies.First().Parameters["foo"]);
        }

        [Fact]
        public void should_deserialize_with_multiple_strategies() 
        {
            var content = File.ReadAllText("resources/features-v1.json");

            var toggleCollection = JsonToggleParser.FromJson(content);
            var featureZ = toggleCollection.GetToggle("featureZ");

            Assert.True(featureZ.IsEnabled);
            Assert.Equal(2, featureZ.Strategies.Count());
            Assert.Equal("hola", featureZ.Strategies.ToArray()[1].Name);
            Assert.Equal("val", featureZ.Strategies.ToArray()[1].Parameters["name"]);
        }

        [Fact]
        public void should_throw()
        {
            var content = File.ReadAllText("resources/empty.json");
            Assert.Throws<ArgumentException>(() => JsonToggleParser.FromJson(content));
        }

        [Fact]
        public void should_throw_on_mission_features()
        {
            var content = File.ReadAllText("resources/empty-v1.json");
            Assert.Throws<Exception>(() => JsonToggleParser.FromJson(content));
        }

        [Fact]
        public void should_deserialize_empty_litst_of_toggles() 
        {
            var content = File.ReadAllText("resources/features-v1-empty.json");
            var toggleCollection = JsonToggleParser.FromJson(content);

            Assert.Empty(toggleCollection.Features);
        }

        [Fact]
        public void should_deserialize_old_format()
        {
            var content = File.ReadAllText("resources/features-v0.json");
            var toggleCollection = JsonToggleParser.FromJson(content);
            var featureY = toggleCollection.GetToggle("featureY");

            Assert.Equal(3, toggleCollection.Features.Count());
            Assert.Single(featureY.Strategies);
            Assert.Equal("baz", featureY.Strategies.First().Name);
            Assert.Equal("bar", featureY.Strategies.First().Parameters["foo"]);
        }
    }
}