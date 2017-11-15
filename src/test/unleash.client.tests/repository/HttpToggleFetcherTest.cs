using System;
using System.Linq;
using System.Threading.Tasks;
using Olav.Unleash.Repository;
using Olav.Unleash.Util;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace Olav.Unleash.Client.Tests.Repository
{
    public class HttpToggleFetcherTest
    {
        private readonly FluentMockServer _server;
        public HttpToggleFetcherTest()
        {
            _server = FluentMockServer.Start();
        }

        [Fact(Skip="runs in vscode, but not from commandline")]
        public async Task happy_path_test_version0()
        {
            _server.Given(
                Request
                    .Create()
                    .WithPath("/features")
                    .WithHeader("Accept", "application/json")
                    .UsingGet())
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyFromFile("resources/features-v0.json")
                );

            var uri = new Uri("http://localhost:" + _server.Ports.First());
            var config = UnleashConfig.TheBuilder().AppName("test").UnleashAPI(uri).Build();
            var httpToggleFetcher = new HttpToggleFetcher(config);
            var response = await httpToggleFetcher.FetchToggles();
            var featureX = response.ToggleCollection.GetToggle("featureX");
            Assert.True(featureX.IsEnabled);
        }

        [Fact(Skip="runs in vscode, but not from commandline")]
        public async Task happy_path_test_version1() 
        {
            _server.Given(
                Request
                    .Create()
                    .WithPath("/features")
                    .WithHeader("Accept", "application/json")
                    .UsingGet())
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyFromFile("resources/features-v1.json")
                );

            var uri = new Uri("http://localhost:" + _server.Ports.First());
            var config = UnleashConfig.TheBuilder().AppName("test").UnleashAPI(uri).Build();
            var httpToggleFetcher = new HttpToggleFetcher(config);
            var response = await httpToggleFetcher.FetchToggles();
            var featureX = response.ToggleCollection.GetToggle("featureX");
            Assert.True(featureX.IsEnabled);
        }


        [Fact(Skip="runs in vscode, but not from commandline")]
        public void given_empty_body()
        {
            _server.Given(
                Request
                    .Create()
                    .WithPath("/features")
                    .WithHeader("Accept", "application/json")
                    .UsingGet())
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")                        
                );


            var uri = new Uri("http://localhost:" + _server.Ports.First());
            var config = UnleashConfig.TheBuilder().AppName("test").UnleashAPI(uri).Build();
            var httpToggleFetcher = new HttpToggleFetcher(config);
            
            Assert.ThrowsAnyAsync<Exception>(async () => await httpToggleFetcher.FetchToggles());
        }

        [Fact(Skip="runs in vscode, but not from commandline")]
        public void given_json_without_feature_field()
        {
            _server.Given(
                Request
                    .Create()
                    .WithPath("/features")
                    .WithHeader("Accept", "application/json")
                    .UsingGet())
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody("{}")                        
                );


            var uri = new Uri("http://localhost:" + _server.Ports.First());
            var config = UnleashConfig.TheBuilder().AppName("test").UnleashAPI(uri).Build();
            var httpToggleFetcher = new HttpToggleFetcher(config);

            Assert.ThrowsAnyAsync<Exception>(async () => await httpToggleFetcher.FetchToggles());
        }

        [Fact(Skip="runs in vscode, but not from commandline")]
        public async Task should_handle_not_changed()
        {
            _server.Given(
                Request
                    .Create()
                    .WithPath("/features")
                    .WithHeader("Accept", "application/json")
                    .UsingGet())
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(302)
                        .WithHeader("Content-Type", "application/json")
                );


            var uri = new Uri("http://localhost:" + _server.Ports.First());
            var config = UnleashConfig.TheBuilder().AppName("test").UnleashAPI(uri).Build();
            var httpToggleFetcher = new HttpToggleFetcher(config);
            var response = await httpToggleFetcher.FetchToggles();
            Assert.False(response.IsChanged);
        }

        [Fact(Skip="runs in vscode, but not from commandline")]
        public async Task should_handle_errors()
        {
            var httpCodes = new[] {400,401,403,404,500,503};
            foreach(var httpCode in httpCodes) 
            {
                _server.Given(
                    Request
                        .Create()
                        .WithPath("/features")
                        .WithHeader("Accept", "application/json")
                        .UsingGet())
                    .RespondWith(
                        Response
                            .Create()
                            .WithStatusCode(httpCode)
                            .WithHeader("Content-Type", "application/json")
                    );


                var uri = new Uri("http://localhost:" + _server.Ports.First());
                var config = UnleashConfig.TheBuilder().AppName("test").UnleashAPI(uri).Build();
                var httpToggleFetcher = new HttpToggleFetcher(config);
                var response = await httpToggleFetcher.FetchToggles();
                Assert.False(response.IsChanged);
            }
        }
    }
}