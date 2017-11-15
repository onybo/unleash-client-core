using System;
using System.Threading;
using System.Threading.Tasks;

namespace Olav.Unleash.Util
{
    public class UnleashScheduledExecutorImpl : IUnleashScheduledExecutor
    {
        private Timer _timer;
        private AutoResetEvent _autoEvent;

        public AutoResetEvent SetInterval(TimerCallback callback,
                                                          long initialDelaySec,
                                                          long periodSec)
        {
            _autoEvent = new AutoResetEvent(false);

            _timer = new Timer(callback, _autoEvent, initialDelaySec * 1000, periodSec * 1000);
            return _autoEvent;
        }
    }
}
