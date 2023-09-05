using System;
using System.Diagnostics;

namespace Animator
{
    public class DisposableStopwatch : IDisposable
    {
        private static readonly object writeLock = new();

        private readonly Stopwatch stopwatch;
        private readonly Action writeMessage;

        public DisposableStopwatch(Action writeMessage)
        {
            stopwatch = Stopwatch.StartNew();
            this.writeMessage = writeMessage;
        }

        public void Dispose()
        {
            lock (writeLock)
            {
                stopwatch.Stop();
                writeMessage();
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($" ({stopwatch.ElapsedMilliseconds}ms)");
                Console.ForegroundColor = color;
            }
        }
    }
}
