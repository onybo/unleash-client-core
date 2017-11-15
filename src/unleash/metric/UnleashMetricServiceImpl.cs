using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Olav.Unleash.Util;

namespace Olav.Unleash.Metric
{
    internal class UnleashMetricServiceImpl : IUnleashMetricService
    {
        private readonly DateTime _started;
        private readonly UnleashConfig _unleashConfig;
        private readonly long _metricsInterval;
        private readonly UnleashMetricsSender _unleashMetricsSender;

        //mutable
        private MetricsBucket _currentMetricsBucket;

        private readonly IUnleashScheduledExecutor _unleashExecutor;

        public UnleashMetricServiceImpl(UnleashConfig unleashConfig,
            IUnleashScheduledExecutor executor)
            : this(unleashConfig, new UnleashMetricsSender(unleashConfig), executor)
        {
        }

        internal UnleashMetricServiceImpl(UnleashConfig unleashConfig,
                                        UnleashMetricsSender unleashMetricsSender,
                                        IUnleashScheduledExecutor executor)
        {
            _currentMetricsBucket = new MetricsBucket();
            _started = DateTime.UtcNow;
            _unleashConfig = unleashConfig;
            _metricsInterval = unleashConfig.SendMetricsInterval;
            _unleashMetricsSender = unleashMetricsSender;
            _unleashExecutor = executor;            

            executor.SetInterval(async s => await SendMetrics(s), _metricsInterval, _metricsInterval);
        }

        public async Task Register(HashSet<string> strategies)
        {
            var registration = new ClientRegistration(_unleashConfig, _started, strategies);
            await _unleashMetricsSender.RegisterClient(registration);
        }

        public void Count(string toggleName, bool active)
        {
            _currentMetricsBucket.RegisterCount(toggleName, active);
        }

        public async Task SendMetrics(object e)
        {
            var metricsBucket = _currentMetricsBucket;
            _currentMetricsBucket = new MetricsBucket();
            metricsBucket.End();
            ClientMetrics metrics = new ClientMetrics(_unleashConfig, metricsBucket);
            await _unleashMetricsSender.SendMetrics(metrics);
        }
    }
}