
namespace Olav.Unleash.Repository
{
    public interface IToggleRepository 
    {
        FeatureToggle GetToggle(string name);
    }
}

