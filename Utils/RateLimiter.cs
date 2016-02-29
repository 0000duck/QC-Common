using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace QuantumConcepts.Common.Utils
{
    /// <summary>This class is useful when a certain amount of time must pass when running specific code. At disposal, this class will sleep until the specified time has elapsed.</summary>
    public class RateLimiter : IDisposable
    {
        public long Milliseconds { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public RateLimiter(long milliseconds)
        {
            this.Milliseconds = milliseconds;
            this.Start = DateTime.Now;
            this.End = this.Start.AddMilliseconds(this.Milliseconds);
        }
        
        public void Dispose()
        {
            //Sleep until the end time is reached.
            while (this.End > DateTime.Now)
                Thread.Sleep(1);
        }
    }
}
