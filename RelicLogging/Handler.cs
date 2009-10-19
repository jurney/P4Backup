using System;
using System.Collections.Generic;
using System.Text;

namespace RelicLogging
{
    public enum MessageType
    {
        Error,
        Warning,
        Output
    }

    public interface ILogHandler
    {
        void HandleLog(MessageType messageType, string message);

        // Begin a batch, meaning do not immediately take action, but instead queue logging
        // if until EndBatch() is called, then handle all queued messages together.
        void BeginBatch();
        void EndBatch();
    }
}
