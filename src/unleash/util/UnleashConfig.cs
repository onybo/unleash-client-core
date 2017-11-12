using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using Serilog;

namespace Olav.Unleash.Util
{
    public class UnleashConfig
    {
        private const string UNLEASH_APP_NAME_HEADER = "UNLEASH-APPNAME";
        private const string UNLEASH_INSTANCE_ID_HEADER = "UNLEASH-INSTANCEID";

        public UnleashConfig(
                Uri unleashAPI,
                Dictionary<string, string> customHttpHeaders,
                string appName,
                string instanceId,
                string sdkVersion,
                string backupFile,
                long fetchTogglesInterval,
                long sendMetricsInterval,
                bool disableMetrics)
        {


            if (appName == null)
            {
                throw new ArgumentNullException(nameof(appName), "You are required to specify the unleash appName");
            }

            if (unleashAPI == null)
            {
                throw new ArgumentNullException(nameof(unleashAPI), "You are required to specify the unleashAPI url");
            }

            UnleashAPI = unleashAPI;
            CustomHttpHeaders = customHttpHeaders;
            UnleashURLs = new UnleashURLs(unleashAPI);
            AppName = appName;
            InstanceId = instanceId;
            SdkVersion = sdkVersion;
            BackupFile = backupFile;
            FetchTogglesInterval = fetchTogglesInterval;
            SendMetricsInterval = sendMetricsInterval;
            IsDisableMetrics = disableMetrics;
        }

        public Uri UnleashAPI { get; private set; }

        public Dictionary<string, string> CustomHttpHeaders { get; private set; }

        public string AppName { get; private set; }

        public string InstanceId { get; private set; }

        public string SdkVersion { get; private set; }

        public long FetchTogglesInterval { get; private set; }

        public long SendMetricsInterval { get; private set; }

        public bool IsDisableMetrics {get; private set; }

        public UnleashURLs UnleashURLs { get; private set; }

        public static Builder TheBuilder()
        {
            return new Builder();
        }

        public string BackupFile { get; set; }

        public static void SetRequestProperties(HttpRequestHeaders headers, UnleashConfig config)
        {
            headers.Add(UNLEASH_APP_NAME_HEADER, config.AppName);
            headers.Add(UNLEASH_INSTANCE_ID_HEADER, config.InstanceId);
            headers.Add("User-Agent", config.AppName);

            config
                .CustomHttpHeaders
                .Keys
                .ToList()                
                .ForEach(key => headers.Add(key, config.CustomHttpHeaders[key]));
        }

        public class Builder
        {
            private Uri _unleashAPI;
            private Dictionary<string, string> _customHttpHeaders = new Dictionary<string, string>();
            private string _appName;
            private string _instanceId = GetDefaultInstanceId();
            private string _sdkVersion = GetDefaultSdkVersion();
            private string _backupFile;
            private long _fetchTogglesInterval = 10;
            private long _sendMetricsInterval = 60;
            private bool _disableMetrics = false;

            static string GetDefaultInstanceId()
            {
                string hostName = "";
                try
                {
                    hostName = System.Net.Dns.GetHostName() + "-";
                }
                catch (SocketException e)
                {
                    Log.Warning(e, "Failed to resolve hostname");
                }
                return hostName + "generated-" + (new Random().Next(1000000));
            }

            public Builder UnleashAPI(Uri unleashAPI)
            {
                _unleashAPI = unleashAPI;
                return this;
            }

            public Builder UnleashAPI(string unleashAPI)
            {
                _unleashAPI = new Uri(unleashAPI);
                return this;
            }

            public Builder CustomHttpHeader(string name, string value)
            {
                _customHttpHeaders.Add(name, value);
                return this;
            }

            public Builder AppName(string appName)
            {
                _appName = appName;
                return this;
            }

            public Builder InstanceId(string instanceId)
            {
                _instanceId = instanceId;
                return this;
            }

            public Builder FetchTogglesInterval(long fetchTogglesInterval)
            {
                _fetchTogglesInterval = fetchTogglesInterval;
                return this;
            }

            public Builder SendMetricsInterval(long sendMetricsInterval)
            {
                _sendMetricsInterval = sendMetricsInterval;
                return this;
            }

            public Builder DisableMetrics()
            {
                _disableMetrics = true;
                return this;
            }

            public Builder BackupFile(string backupFile)
            {
                _backupFile = backupFile;
                return this;
            }

            private string GetBackupFile()
            {
                if (_backupFile != null)
                {
                    return _backupFile;
                }
                else
                {
                    var fileName = $"unleash-{_appName}-repo.json";
                    return Path.GetTempPath() + Path.PathSeparator + fileName;
                }
            }

            public UnleashConfig Build()
            {
                return new UnleashConfig(
                        _unleashAPI,
                        _customHttpHeaders,
                        _appName,
                        _instanceId,
                        _sdkVersion,
                        GetBackupFile(),
                        _fetchTogglesInterval,
                        _sendMetricsInterval,
                        _disableMetrics);
            }

            private static string GetDefaultSdkVersion()
            {
                // string version = Optional.ofNullable(getClass().getPackage().getImplementationVersion())
                //         .orElse("development");
                var version = "development";
                return "unleash-client-core:" + version;
            }
        }
    }
}