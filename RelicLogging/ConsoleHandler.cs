using System;
using System.Collections.Generic;
using System.Text;

namespace RelicLogging
{
    public class ConsoleHandler : ILogHandler
    {
        public void HandleLog(MessageType messageType, string message)
        {
            System.Console.Error.WriteLine(message);
        }

        public void BeginBatch() { }
        public void EndBatch() { }
    }
}
