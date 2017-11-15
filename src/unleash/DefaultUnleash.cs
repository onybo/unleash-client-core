using System.Collections.Generic;
using System.Linq;
using Olav.Unleash.Metric;
using Olav.Unleash.Repository;
using Olav.Unleash.Strategy;
using Olav.Unleash.Util;
using AStrategy = Olav.Unleash.Strategy.Strategy;

namespace Olav.Unleash
{
    public sealed class DefaultUnleash : Unleash
    {
        private static readonly IEnumerable<AStrategy> BUILTIN_STRATEGIES = new AStrategy[] 
                { 
                    new DefaultStrategy(),
                    new ApplicationHostnameStrategy(),
                    new GradualRolloutRandomStrategy(),
                    new GradualRolloutSessionIdStrategy(),
                    new GradualRolloutUserIdStrategy(),
                    new RemoteAddressStrategy(),
                    new UserWithIdStrategy()
                };

        private static readonly UnknownStrategy UNKNOWN_STRATEGY = new UnknownStrategy();

        private readonly IUnleashMetricService _metricService;
        private readonly IToggleRepository _toggleRepository;
        private readonly Dictionary<string, AStrategy> _strategyMap;

        private static FeatureToggleRepository DefaultToggleRepository(UnleashConfig unleashConfig)
        {
            return new FeatureToggleRepository(
                    unleashConfig,
                    new UnleashScheduledExecutorImpl(),
                    new HttpToggleFetcher(unleashConfig),
                    new ToggleBackupHandlerFile(unleashConfig));
        }

        public DefaultUnleash(UnleashConfig unleashConfig, params AStrategy[] strategies) : this(unleashConfig, DefaultToggleRepository(unleashConfig), strategies)
        {
        }

        public DefaultUnleash(UnleashConfig unleashConfig, IToggleRepository toggleRepository,
                params AStrategy[] strategies)
        {
            _toggleRepository = toggleRepository;
            _strategyMap = BuildStrategyMap(strategies);
            _metricService = new UnleashMetricServiceImpl(unleashConfig, new UnleashScheduledExecutorImpl());
            _metricService.Register(new HashSet<string>(_strategyMap.Keys.ToList()));
        }

        public override bool IsEnabled(string toggleName, UnleashContext context)
        {
            return IsEnabled(toggleName, context, false);
        }

        // public override bool IsEnabled(string toggleName, bool defaultSetting)
        // {
        //     return IsEnabled(toggleName, _contextProvider.Context, defaultSetting);
        // }

        public override bool IsEnabled(string toggleName, UnleashContext context, bool defaultSetting)
        {
            bool enabled;
            FeatureToggle featureToggle = _toggleRepository.GetToggle(toggleName);

            if (featureToggle == null)
            {
                enabled = defaultSetting;
            }
            else if (!featureToggle.IsEnabled)
            {
                enabled = false;
            }
            else
            {
                enabled = featureToggle
                            .Strategies
                            .Any(s => GetStrategy(s.Name).IsEnabled(s.Parameters, context));
            }

            Count(toggleName, enabled);
            return enabled;
        }

        public FeatureToggle GetFeatureToggleDefinition(string toggleName)
        {
            return _toggleRepository.GetToggle(toggleName);
        }

        public void Count(string toggleName, bool enabled)
        {
            _metricService.Count(toggleName, enabled);
        }

        private Dictionary<string, AStrategy> BuildStrategyMap(AStrategy[] strategies)
        {
            var map = new Dictionary<string, AStrategy>();

            BUILTIN_STRATEGIES
                .Concat(strategies ?? new AStrategy[0])
                .ToList()
                .ForEach(strategy => map.Add(strategy.Name, strategy));

            return map;
        }

        private AStrategy GetStrategy(string strategy)
        {
            return _strategyMap.ContainsKey(strategy) ? _strategyMap[strategy] : UNKNOWN_STRATEGY;
        }
    }
}
