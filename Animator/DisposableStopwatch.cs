using System;
using System.Diagnostics;

namespace Animator
{
    public class DisposableStopwatch : IDisposable
    {
        private readonly string comment;
        private readonly Stopwatch stopwatch;

        public DisposableStopwatch(string comment)
        {
            this.comment = comment;
            stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            stopwatch.Stop();
            Console.WriteLine($"{comment} ({stopwatch.ElapsedMilliseconds}ms)");
        }
    }
}
