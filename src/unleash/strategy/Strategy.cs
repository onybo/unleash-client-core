using System.Collections.Generic;

namespace Olav.Unleash.Strategy
{
    public abstract class Strategy 
    {
        public virtual string Name { get; }
        public abstract bool IsEnabled(Dictionary<string, string> parameters);
        
        public virtual bool IsEnabled(Dictionary<string, string> parameters, UnleashContext context) =>
            IsEnabled(parameters);
    }
}