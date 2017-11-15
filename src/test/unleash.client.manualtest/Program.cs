using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Olav.Unleash.Repository;
using Olav.Unleash.Util;

namespace Olav.Unleash.Client.ManualTest
{
    public class ActiveForUserWithIdStrategy : Olav.Unleash.Strategy.Strategy
    {
        public override string Name => "ActiveForUserWithId";

        public override bool IsEnabled(Dictionary<string, string> parameters)
        {
            Console.WriteLine("parameters = " + parameters);
            return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var unleashConfig = new UnleashConfig.Builder()
                .AppName("c-sharp-test")
                .InstanceId("instance y")
                .UnleashAPI("https://unleash.herokuapp.com/api/")
                .FetchTogglesInterval(1)
                .SendMetricsInterval(2)
                .Build();

            var context = UnleashContext.CreateBuilder()
                            .SessionId(new Random().Next(10000) + "")
                            .UserId(new Random().Next(10000) + "")
                            .RemoteAddress("192.168.1.1")
                            .Build();

            // var executor = new UnleashScheduledExecutorImpl();
            // var repository = new FeatureToggleRepository(
            //         unleashConfig,
            //         executor,
            //         new HttpToggleFetcher(unleashConfig),
            //         new ToggleBackupHandlerFile(unleashConfig));
            var unleash = new DefaultUnleash(unleashConfig,                 
                    //repository,
                    new ActiveForUserWithIdStrategy()
                );

            
            //var test =  executor.SetInterval(s => repository.UpdateToggles(s).Wait(), 0, unleashConfig.FetchTogglesInterval);

            Run(unleash, context, "test", 200);
        }

        private static void Run(DefaultUnleash unleash, UnleashContext context, string name, int maxRounds)
        {
            int currentRound = 0;
            var sw = new Stopwatch();
            while (currentRound < maxRounds)
            {
                currentRound++;
                
                sw.Start();
                bool enabled = unleash.IsEnabled("Demo", context);
                sw.Stop();

                Console.WriteLine(name + "\t" + "Demo" + ":" + enabled + "\t " + sw.ElapsedMilliseconds + "ms");
                sw.Reset();

                Thread.Sleep(new Random().Next(10000));
            }
        }
    }
}

