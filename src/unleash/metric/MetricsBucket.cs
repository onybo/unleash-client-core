using System;
using System.Collections.Generic;

namespace Olav.Unleash.Metric
{
    internal class MetricsBucket
    {
        internal MetricsBucket()
        {
            Start = DateTime.UtcNow;
            Toggles = new Dictionary<string, ToggleCount>();
        }

        internal void RegisterCount(string toggleName, bool active)
        {
            if (Toggles.ContainsKey(toggleName))
            {
                Toggles[toggleName].Register(active);
            }
            else
            {
                var counter = new ToggleCount();
                counter.Register(active);
                Toggles.Add(toggleName, counter);
            }
        }

        internal void End()
        {
            Stop = DateTime.UtcNow;
        }

        internal Dictionary<string, ToggleCount> Toggles { get; private set; }

        internal DateTime Start { get; private set; }

        internal DateTime Stop { get; private set; }
    }
}