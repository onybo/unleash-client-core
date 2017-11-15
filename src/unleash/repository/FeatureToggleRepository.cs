using System;
using System.Threading;
using System.Threading.Tasks;
using Olav.Unleash.Util;
using Serilog;

namespace Olav.Unleash.Repository
{
    public sealed class FeatureToggleRepository : IToggleRepository
    {

        private readonly IToggleBackupHandler _toggleBackupHandler;
        private readonly IToggleFetcher _toggleFetcher;

        private ToggleCollection _toggleCollection;

        private IUnleashScheduledExecutor _executor;

        public FeatureToggleRepository(
                UnleashConfig unleashConfig,
                IUnleashScheduledExecutor executor,
                IToggleFetcher toggleFetcher,
                IToggleBackupHandler toggleBackupHandler)
        {

            _toggleBackupHandler = toggleBackupHandler;
            _toggleFetcher = toggleFetcher;

            _toggleCollection = toggleBackupHandler.Read();

            _executor = executor;
            _executor.SetInterval(s => UpdateToggles(s).Wait(), 0, unleashConfig.FetchTogglesInterval);
        }

        public async Task UpdateToggles(object state)
        {            
            try
            {
                var response = await _toggleFetcher.FetchToggles();
                if (response.IsChanged)
                {
                    _toggleCollection = response.ToggleCollection;
                    _toggleBackupHandler.Write(response.ToggleCollection);
                }
            }
            catch (Exception e)
            {
                Log.Warning("Could not refresh feature toggles", e);
            }
        }

        public FeatureToggle GetToggle(string name)
        {
            return _toggleCollection.GetToggle(name);
        }
    }
}