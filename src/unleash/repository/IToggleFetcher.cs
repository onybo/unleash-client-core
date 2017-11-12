using System.Threading.Tasks;

namespace Olav.Unleash.Repository
{
    public interface IToggleFetcher 
    {
       Task<FeatureToggleResponse> FetchToggles();  
    }
}
