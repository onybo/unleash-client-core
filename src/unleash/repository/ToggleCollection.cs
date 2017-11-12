using System.Collections.Generic;
using Newtonsoft.Json;

namespace Olav.Unleash.Repository
{
    public sealed class ToggleCollection
    {
        private readonly IEnumerable<FeatureToggle> _features;
        private const int version = 1;

        [JsonIgnore]
        private readonly Dictionary<string, FeatureToggle> _cache;

        public ToggleCollection()
        {            
        }
        
        internal ToggleCollection(IEnumerable<FeatureToggle> features)
        {
            _features = EnsureNotNull(features);
            _cache = new Dictionary<string, FeatureToggle>();
            foreach (var featureToggle in _features)
            {
                _cache.Add(featureToggle.Name, featureToggle);
            }
        }

        public static ToggleCollection EmptyCollection => new ToggleCollection(new List<FeatureToggle>());

        private IEnumerable<FeatureToggle> EnsureNotNull(IEnumerable<FeatureToggle> features)
        {
            if (features == null) return new List<FeatureToggle>();
            return features;
        }

        internal IEnumerable<FeatureToggle> Features => _features;

        internal FeatureToggle GetToggle(string name) => _cache.ContainsKey(name) ?  _cache[name] : null;
    }
}