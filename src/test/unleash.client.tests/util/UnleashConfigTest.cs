using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Olav.Unleash.Util;
using Xunit;

namespace Olav.Unleash.Client.Tests.Util
{



    public class UnleashConfigTest
    {

        [Fact]
        public void should_require_unleasAPI_url()
        {
            Assert.Throws<ArgumentNullException>(() => UnleashConfig.TheBuilder().AppName("test").Build());
        }

        [Fact]
        public void should_require_app_name()
        {
            Assert.Throws<ArgumentNullException>(() => UnleashConfig.TheBuilder().UnleashAPI("http://unleash.com").Build());
        }

        [Fact]
        public void should_require_valid_uri()
        {
            Assert.Throws<UriFormatException>(() => UnleashConfig.TheBuilder().UnleashAPI("this is not a uri").Build());
        }

        [Fact]
        public void should_build_config()
        {
            UnleashConfig config = UnleashConfig.TheBuilder()
                    .AppName("my-app")
                    .InstanceId("my-instance-1")
                    .UnleashAPI("http://unleash.org")
                    .Build();

            Assert.Equal("my-app", config.AppName);
            Assert.Equal("my-instance-1", config.InstanceId);
            Assert.Equal(new Uri("http://unleash.org"), config.UnleashAPI);
        }

        [Fact]
        public void should_generate_backupfile()
        {
            UnleashConfig config = UnleashConfig.TheBuilder()
                    .AppName("my-app")
                    .UnleashAPI("http://unleash.org")
                    .Build();

            Assert.Equal("my-app", config.AppName);
            Assert.Equal(Path.GetTempPath() + Path.PathSeparator + "unleash-my-app-repo.json", config.BackupFile);
        }

        [Fact]
        public void should_use_provided_backupfile()
        {
            UnleashConfig config = UnleashConfig.TheBuilder()
                    .AppName("my-app")
                    .BackupFile("/test/unleash-backup.json")
                    .UnleashAPI("http://unleash.org")
                    .Build();

            Assert.Equal("my-app", config.AppName);
            Assert.Equal("/test/unleash-backup.json", config.BackupFile);
        }

        [Fact]
        public void should_set_sdk_version()
        {
            UnleashConfig config = UnleashConfig.TheBuilder()
                    .AppName("my-app")
                    .UnleashAPI("http://unleash.org")
                    .Build();

            Assert.Equal("unleash-client-core:development", config.SdkVersion);
        }

        [Fact]
        public void should_add_app_name_and_instance_id_and_user_agent_to_connection() //throws IOException 
        {
            string appName = "my-app";
            string instanceId = "my-instance-1";
            string unleashAPI = "http://unleash.org";

            var unleashConfig = UnleashConfig.TheBuilder()
                    .AppName(appName)
                    .InstanceId(instanceId)
                    .UnleashAPI(unleashAPI)
                    .Build();

            var someUrl = new Uri(unleashAPI + "/some/arbitrary/path");
            var request = new HttpRequestMessage(HttpMethod.Get, someUrl);

            UnleashConfig.SetRequestProperties(request.Headers, unleashConfig);

            Assert.Single(request.Headers.GetValues(UnleashConfig.UNLEASH_APP_NAME_HEADER));
            Assert.Equal(appName, request.Headers.GetValues(UnleashConfig.UNLEASH_APP_NAME_HEADER).First());
            Assert.Single(request.Headers.GetValues(UnleashConfig.UNLEASH_INSTANCE_ID_HEADER));
            Assert.Equal(instanceId, request.Headers.GetValues(UnleashConfig.UNLEASH_INSTANCE_ID_HEADER).First());
            Assert.Equal(appName, request.Headers.GetValues("User-Agent").First());
        }

        [Fact]
        public void should_add_custom_headers_to_connection_if_present()
        {
            string unleashAPI = "http://unleash.org";
            string headerName = "UNLEASH-CUSTOM-TEST-HEADER";
            string headerValue = "Some value";

            UnleashConfig unleashConfig = UnleashConfig.TheBuilder()
                    .AppName("my-app")
                    .InstanceId("my-instance-1")
                    .UnleashAPI(unleashAPI)
                    .CustomHttpHeader(headerName, headerValue)
                    .Build();

            var someUrl = new Uri(unleashAPI + "/some/arbitrary/path");
            var request = new HttpRequestMessage(HttpMethod.Get, someUrl);

            UnleashConfig.SetRequestProperties(request.Headers, unleashConfig);

            Assert.Single(request.Headers.GetValues(headerName));
            Assert.Equal(headerValue, request.Headers.GetValues(headerName).First());
        }
    }
}