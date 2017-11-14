using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasyCaptures.Helpers;
using Olav.Unleash.Metric;
using Olav.Unleash.Util;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace Olav.Unleash.Client.Core.Tests.Repository
{
    public class UnleashMetricsSenderTest
    {
        private readonly FluentMockServer _server;
        public UnleashMetricsSenderTest()
        {
            _server = FluentMockServer.Start();
        }

        [Fact(Skip="runs in vscode, but not from commandline")]
        public async Task should_send_client_registration()
        {
            _server.Given(
                Request
                    .Create()
                    .WithPath("/client/register")
                    .WithHeader("UNLEASH-APPNAME", "test-app")
                    .UsingGet())
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(200)
                );

            var uri = new Uri("http://localhost:" + _server.Ports.First());
            var config = UnleashConfig.TheBuilder().AppName("test-app").UnleashAPI(uri).Build();

            var sender = new UnleashMetricsSender(config);
            await sender.RegisterClient(new ClientRegistration(config, DateTime.Now, new HashSet<string>()));
        }

        [Fact(Skip="runs in vscode, but not from commandline")]
        public async Task should_send_client_metrics()
        {
            _server.Given(
                Request
                    .Create()
                    .WithPath("/client/metrics")
                    .WithHeader("UNLEASH-APPNAME", "test-app")
                    .UsingGet())
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(200)
                );

            var uri = new Uri("http://localhost:" + _server.Ports.First());
            var config = UnleashConfig.TheBuilder().AppName("test-app").UnleashAPI(uri).Build();

            var sender = new UnleashMetricsSender(config);
            var bucket = new MetricsBucket();
            var metrics = new ClientMetrics(config, bucket);
            await sender.SendMetrics(metrics);
        }

        [Fact(Skip="runs in vscode, but not from commandline")]
        public void should_handle_service_failure_when_sending_metrics()
        {
            _server.Given(
                Request
                    .Create()
                    .WithPath("/client/metrics")
                    .WithHeader("UNLEASH-APPNAME", "test-app")
                    .UsingGet())
                .RespondWith(
                    Response
                        .Create()
                        .WithStatusCode(500)
                );

            var uri = new Uri("http://localhost:" + _server.Ports.First());
            var config = UnleashConfig.TheBuilder().AppName("test-app").UnleashAPI(uri).Build();

            var sender = new UnleashMetricsSender(config);
            var bucket = new MetricsBucket();
            var metrics = new ClientMetrics(config, bucket);
            sender.SendMetrics(metrics);
        }
    }
}