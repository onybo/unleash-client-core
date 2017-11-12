using System;
using System.Threading;
using System.Threading.Tasks;

namespace Olav.Unleash.Util
{
        public interface IUnleashScheduledExecutor 
        {
                AutoResetEvent SetInterval(Func<object, Task> commandFactory, long initialDelaySec, long periodSec);
        }    
}
