using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWE.VideoLister.BusinessLogic.Providers
{
    internal class LoggingProvider : ILoggingProvider
    {
        private readonly ConcurrentQueue<string> queue;

        public LoggingProvider()
        {
            queue = new ConcurrentQueue<string>();
        }

        public void LogError(string message)
        {
            queue.Enqueue($"ERROR: {message}");
        }

        public void LogInfo(string message)
        {
            queue.Enqueue($"INFO: {message}");
        }

        public bool TryGetLastMessage(out string message)
        {
            return queue.TryDequeue(out message);
        }
    }
}
