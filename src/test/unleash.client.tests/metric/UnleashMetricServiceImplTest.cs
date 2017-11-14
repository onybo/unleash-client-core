using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasyCaptures.Helpers;
using Olav.Unleash.Metric;
using Olav.Unleash.Util;
using Xunit;

namespace Olav.Unleash.Client.Core.Tests.Repository
{
    public class UnleashMetricServiceImplTest
    {

        [Fact]
        public void should_register_future_for_sending_interval_regualry()
        {
            long interval = 10;
            UnleashConfig config = UnleashConfig
                    .TheBuilder()
                    .AppName("test")
                    .SendMetricsInterval(interval)
                    .UnleashAPI("http://unleash.com")
                    .Build();
            var executor = A.Fake<IUnleashScheduledExecutor>();
            var unleashMetricService = new UnleashMetricServiceImpl(config, executor);

            A.CallTo(() => executor.SetInterval(
                                    A<Func<object, Task>>.Ignored,
                                    A<long>.That.IsEqualTo(interval),
                                    A<long>.That.IsEqualTo(interval)))
                        .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task should_register_client() 
        {
            long interval = 10;
            UnleashConfig config = UnleashConfig
                    .TheBuilder()
                    .AppName("test")
                    .SendMetricsInterval(interval)
                    .UnleashAPI("http://unleash.com")
                    .Build();

            var executor = A.Fake<IUnleashScheduledExecutor>();
            var sender = A.Fake<UnleashMetricsSender>();

            var argument = new Capture<ClientRegistration>();
            A.CallTo(() => sender.RegisterClient(argument)).Returns(Task.FromResult(0));

            var unleashMetricService = new UnleashMetricServiceImpl(config, sender, executor);
            var strategies = new HashSet<string>();
            strategies.Add("default");
            strategies.Add("custom");
            await unleashMetricService.Register(strategies);

            Assert.Equal(config.AppName, argument.Value.AppName);
            Assert.Equal(config.InstanceId, argument.Value.InstanceId);
            Assert.True(argument.Value.Started > DateTime.MinValue);
            Assert.Equal(2, argument.Value.Strategies.Count);
            Assert.Contains("default", argument.Value.Strategies);
            Assert.Contains("custom", argument.Value.Strategies);
        }


        [Fact]
        public async Task should_send_metrics() 
        {
            UnleashConfig config = UnleashConfig
                    .TheBuilder()
                    .AppName("test")
                    .SendMetricsInterval(10)
                    .UnleashAPI("http://unleash.com")
                    .Build();

            var executor = A.Fake<IUnleashScheduledExecutor>();
            var sender = A.Fake<UnleashMetricsSender>();

            var sendMetricsCallback = new Capture<Func<object, Task>>();

            A.CallTo(() => executor.SetInterval(sendMetricsCallback, A<long>.Ignored, A<long>.Ignored)).Returns(new AutoResetEvent(false));

            var unleashMetricService = new UnleashMetricServiceImpl(config, sender, executor);

            await sendMetricsCallback.Value.Invoke(null);
            
            A.CallTo(() => sender.SendMetrics(
                                    A<ClientMetrics>.Ignored))
                        .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void should_record_and_send_metrics() 
        {
            UnleashConfig config = UnleashConfig
                    .TheBuilder()
                    .AppName("test")
                    .SendMetricsInterval(10)
                    .UnleashAPI("http://unleash.com")
                    .Build();

            var executor = A.Fake<IUnleashScheduledExecutor>();
            var sender = A.Fake<UnleashMetricsSender>();

            var sendMetricsCallback = new Capture<Func<object, Task>>();
            A.CallTo(() => executor.SetInterval(sendMetricsCallback, A<long>.Ignored, A<long>.Ignored)).Returns(new AutoResetEvent(false));

            var clientMetricsArgumentCaptor = new Capture<ClientMetrics>();
            A.CallTo(() => sender.SendMetrics(clientMetricsArgumentCaptor)).Returns(Task.FromResult(0));

            var unleashMetricService = new UnleashMetricServiceImpl(config, sender, executor);
            unleashMetricService.Count("someToggle", true);
            unleashMetricService.Count("someToggle", false);
            unleashMetricService.Count("someToggle", true);
            unleashMetricService.Count("otherToggle", true);

            //Call the sendMetricsCallback
            // var sendMetricsCallback = ArgumentCaptor.forClass(Runnable.class);
            // verify(executor).setInterval(sendMetricsCallback.capture(), anyLong(), anyLong());
            // sendMetricsCallback.getValue().run();

            //verify(sender).sendMetrics(clientMetricsArgumentCaptor.capture());
            sendMetricsCallback.Value.Invoke(null);


            var clientMetrics = clientMetricsArgumentCaptor.Value;
            var bucket = clientMetricsArgumentCaptor.Value.Bucket;

            Assert.Equal(config.AppName, clientMetrics.AppName);
            Assert.Equal(config.InstanceId, clientMetrics.InstanceId);
            Assert.True(bucket.Start > DateTime.MinValue);
            Assert.True(bucket.Stop > DateTime.MinValue);
            Assert.Equal(2, bucket.Toggles.Count);
            Assert.Equal(2, bucket.Toggles["someToggle"].Yes);
            Assert.Equal(1, bucket.Toggles["someToggle"].No);
        }
    }
}