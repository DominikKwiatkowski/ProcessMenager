using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace ProcessManager.Core.Models
{
    public class Timer
    {
        public TimeSpan Time { get; private set; }
        private DateTime _beginTime;
        public event EventHandler<TimeSpan> TimeElapsed;

        public Timer(DateTime beginTime)
        {
            _beginTime = beginTime;
            Time = DateAndTime.Now - beginTime;
        }

        public async Task StartAsync(CancellationToken token = default(CancellationToken))
        {
            while (true)
            {
                // wait 1000 ms
                await Task.Delay(1000, token).ConfigureAwait(false);
                Time = DateAndTime.Now - _beginTime;
                TimeElapsed?.Invoke(this, Time);
            }
        }
    }
}
