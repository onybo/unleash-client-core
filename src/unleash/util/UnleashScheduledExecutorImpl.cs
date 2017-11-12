using System;
using System.Threading;
using System.Threading.Tasks;

namespace Olav.Unleash.Util
{
    public class UnleashScheduledExecutorImpl : IUnleashScheduledExecutor
    {
        private Timer _timer;

        public AutoResetEvent SetInterval(Func<object, Task> commandFactory,
                                                          long initialDelaySec,
                                                          long periodSec)
        {
            var autoEvent = new AutoResetEvent(false);

            _timer = new Timer(async (e) => {await commandFactory(e);}, autoEvent, initialDelaySec * 1000, periodSec * 1000);
            return autoEvent;
        }
    }
}
