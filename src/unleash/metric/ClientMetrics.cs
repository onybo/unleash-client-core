using Olav.Unleash.Util;

namespace Olav.Unleash.Metric
{
    internal class ClientMetrics 
    {
        internal ClientMetrics(UnleashConfig config, MetricsBucket bucket) 
        {
            AppName = config.AppName;
            InstanceId = config.InstanceId;
            Bucket = bucket;
        }

        internal string AppName { get; private set; }
        internal string InstanceId { get; private set; }
        internal MetricsBucket Bucket { get; private set; }
    }
}

