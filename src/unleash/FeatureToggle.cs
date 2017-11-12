using System.Collections.Generic;

namespace Olav.Unleash
{
    public sealed class FeatureToggle 
    {
        public FeatureToggle(string name, bool enabled, IEnumerable<ActivationStrategy> strategies) 
        {
            Name = name;
            IsEnabled = enabled;
            Strategies = strategies;
        }

        public string Name { get; private set; }

        public bool IsEnabled { get; private set; }

        public IEnumerable<ActivationStrategy> Strategies { get; private set; }

        public override string ToString() =>
                    $"FeatureToggle{{ name='{Name}' , enabled={IsEnabled}, strategies='{Strategies}'}}"; 
    }
}

