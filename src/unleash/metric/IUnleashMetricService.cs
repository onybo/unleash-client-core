using System.Collections.Generic;
using System.Threading.Tasks;

namespace Olav.Unleash.Metric
{
    public interface IUnleashMetricService 
    {
        Task Register(HashSet<string> strategies);
        void Count(string toggleName, bool active);
    }
}
