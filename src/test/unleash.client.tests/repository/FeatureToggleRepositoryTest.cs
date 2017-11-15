using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasyCaptures.Helpers;
using Olav.Unleash.Repository;
using Olav.Unleash.Util;
using Xunit;

namespace Olav.Unleash.Client.Tests.Repository
{
    internal class MockExecutor : IUnleashScheduledExecutor
    {
        public AutoResetEvent SetInterval(Func<object, Task> commandFactory, long initialDelaySec, long periodSec)
        {
            return null;
        }
    }

    public class FeatureToggleRepositoryTest
    {

        [Fact]
        public void no_backup_file_and_no_repository_available_should_give_empty_repo()
        {
            var config = UnleashConfig.TheBuilder()
                    .AppName("test")
                    .UnleashAPI("http://localhost:4242/api/")
                    .Build();
            var toggleFetcher = new HttpToggleFetcher(config);
            var toggleBackupHandler = new ToggleBackupHandlerFile(config);
            var toggleRepository = new FeatureToggleRepository(config, new MockExecutor(), toggleFetcher, toggleBackupHandler);
            Assert.Null(toggleRepository.GetToggle("unknownFeature"));
        }

        private Task Timer(object state)
        {
            return Task.FromResult(1);
        }

        [Fact]
        public void backup_toggles_should_be_loaded_at_startup()
        {
            UnleashConfig config = UnleashConfig.TheBuilder()
                    .AppName("test")
                    .UnleashAPI("http://localhost:4242/api/")
                    .FetchTogglesInterval(long.MaxValue)
                    .Build();

            var toggleBackupHandler = A.Fake<IToggleBackupHandler>();
            var toggleFetcher = A.Fake<IToggleFetcher>();
            var executor = A.Fake<IUnleashScheduledExecutor>();
            new FeatureToggleRepository(config, executor, toggleFetcher, toggleBackupHandler);

            A.CallTo(() => toggleBackupHandler.Read()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void feature_toggles_should_be_updated()
        {
            var toggleFetcher = A.Fake<IToggleFetcher>();
            var toggleBackupHandler = A.Fake<IToggleBackupHandler>();

            var toggleCollection = PopulatedToggleCollection(
                    new FeatureToggle("toggleFetcherCalled", false, new[] { new ActivationStrategy("custom", null) }));
            A.CallTo(() => toggleBackupHandler.Read()).Returns(toggleCollection);

            //setup fetcher
            toggleCollection = PopulatedToggleCollection(
                    new FeatureToggle("toggleFetcherCalled", true, new[] { new ActivationStrategy("custom", null) }));
            var response = FeatureToggleResponse.Changed(toggleCollection);
            A.CallTo(() => toggleFetcher.FetchToggles()).Returns(response);

            //init
            var executor = A.Fake<IUnleashScheduledExecutor>();
            //ArgumentCaptor<Runnable> runnableArgumentCaptor = ArgumentCaptor.forClass(Runnable.class);
            var cmdFactory = new Capture<Func<object, Task>>();


            UnleashConfig config = new UnleashConfig.Builder()
                    .AppName("test")
                    .UnleashAPI("http://localhost:4242/api/")
                    .FetchTogglesInterval(200L)
                    .Build();

            //run the toggle fetcher callback
            A.CallTo(() => executor.SetInterval(cmdFactory, A<long>.Ignored, A<long>.Ignored)).Returns(new AutoResetEvent(false));

            var toggleRepository = new FeatureToggleRepository(config, executor, toggleFetcher, toggleBackupHandler);


            A.CallTo(() => toggleFetcher.FetchToggles()).MustNotHaveHappened();
            cmdFactory.Value(null);
            A.CallTo(() => toggleBackupHandler.Read()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => toggleFetcher.FetchToggles()).MustHaveHappened(Repeated.Exactly.Once);
            Assert.True(toggleRepository.GetToggle("toggleFetcherCalled").IsEnabled);
        }

        private ToggleCollection PopulatedToggleCollection(FeatureToggle featureToggle)
        {
            return new ToggleCollection(new List<FeatureToggle> {featureToggle});
        }
    }
}