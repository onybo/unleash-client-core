using System;
using System.Collections.Generic;
using Olav.Unleash.Util;

namespace Olav.Unleash.Metric
{
    public class ClientRegistration
    {
        internal ClientRegistration(UnleashConfig config, DateTime started, HashSet<string> strategies)
        {
            AppName = config.AppName;
            InstanceId = config.InstanceId;
            SdkVersion = config.SdkVersion;
            Started = started;
            Strategies = strategies;
            Interval = config.SendMetricsInterval;
        }

        public string AppName { get; private set; }

        public string InstanceId { get; private set; }

        public string SdkVersion { get; private set; }

        public HashSet<string> Strategies { get; private set; }

        public DateTime Started { get; private set; }

        public long Interval { get; private set; }
    }
}