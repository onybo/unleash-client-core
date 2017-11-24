using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Olav.Unleash.Logging;

namespace Olav.Unleash.Repository
{
    public static class JsonToggleParser
    {
        private static readonly ILog Logger = LogProvider.GetLogger("JsonToggleParser");

        public static ToggleCollection FromJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                throw new ArgumentException("some json is required", nameof(jsonString));
            var json = JsonConvert.DeserializeObject<JObject>(jsonString);
            if (!json.TryGetValue("version", out var version))
            {
                return DeserializeVersion0(json);
            }
            else if (((int)version) == 1)
            {
                return DeserializeVersion1(json);
            }

            throw new Exception("Could not extract toggles from json");
        }

        public static ToggleCollection DeserializeVersion0(JObject json)
        {

            if (!json.TryGetValue("features", out var featureTogglesToken))
            {
                Logger.Warn("Features not found in json");
                return ToggleCollection.EmptyCollection;
            }
            var featureToggles = new List<FeatureToggle>();
            foreach(var token in featureTogglesToken)
            {
                var strategyName = token["strategy"].ToObject<string>();
                var parameters = token["parameters"] != null ? token["parameters"].ToObject<Dictionary<string,string>>() : new Dictionary<string, string>();
                var strategy = new ActivationStrategy(strategyName, parameters );
                featureToggles.Add(
                    new FeatureToggle(
                        token["name"].ToObject<string>(),
                        token["enabled"].ToObject<bool>(),
                        new ActivationStrategy[] {strategy}));
            }

            return new ToggleCollection(featureToggles);
        }
        public static ToggleCollection DeserializeVersion1(JObject json)
        {
            if (!json.TryGetValue("features", out var featureTogglesToken))
            {
                Logger.Warn("Features not found in json");
                throw new Exception("Features in json were expected");
            }

            return new ToggleCollection(featureTogglesToken.ToObject<FeatureToggle[]>());
        }
    }
}