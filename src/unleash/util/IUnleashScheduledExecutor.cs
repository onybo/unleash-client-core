using System;
using System.Threading;
using System.Threading.Tasks;

namespace Olav.Unleash.Util
{
        public interface IUnleashScheduledExecutor 
        {
                AutoResetEvent SetInterval(TimerCallback callback, long initialDelaySec, long periodSec);
        }    
}
