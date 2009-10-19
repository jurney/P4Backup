using System;
using System.Collections.Generic;
using System.Text;

namespace RelicLogging
{
    /// <summary>
    /// Threading:
    /// 
    /// Only the actual logging functions are thread-safe. That is, only Error, Output and Warning
    /// are thread safe, everythign else is single-thread only.
    /// </summary>
    public static class Log
    {
        public static void AddHandler(ILogHandler handler)
        {
            m_Handlers.Add(handler);
        }
        public static List<ILogHandler> GetHandlers(Type handlerType)
        {
            List<ILogHandler> handlerList = new List<ILogHandler>();

            foreach (ILogHandler handler in m_Handlers)
            {
                if (handler.GetType() == handlerType)
                    handlerList.Add(handler);
            }

            return handlerList;
        }

        /// <summary>
        /// Threadsafe
        /// </summary>
        public static void Error(string format, params object[] args)
        {
            lock (m_Handlers)
            {
                ++m_BatchErrorCount;
                ++m_TotalErrorCount;

                string message = string.Format(format, args);

                foreach (ILogHandler handler in m_Handlers)
                    handler.HandleLog(MessageType.Error, message);
            }
        }
        /// <summary>
        /// Threadsafe
        /// </summary>
        public static void Output(string format, params object[] args)
        {
            lock (m_Handlers)
            {
                ++m_BatchOutputCount;
                ++m_TotalOutputCount;

                string message = string.Format(format, args);

                foreach (ILogHandler handler in m_Handlers)
                    handler.HandleLog(MessageType.Output, message);
            }
        }
        /// <summary>
        /// Threadsafe
        /// </summary>
        public static void Warning(string format, params object[] args)
        {
            lock (m_Handlers)
            {
                ++m_BatchWarningCount;
                ++m_TotalWarningCount;

                string message = string.Format(format, args);

                foreach (ILogHandler handler in m_Handlers)
                    handler.HandleLog(MessageType.Warning, message);
            }
        }

        public static void BeginBatch()
        {
            m_BatchOutputCount = 0;
            m_BatchWarningCount = 0;
            m_BatchErrorCount = 0;

            foreach (ILogHandler handler in m_Handlers)
                handler.BeginBatch();
        }
        public static void EndBatch()
        {
            foreach (ILogHandler handler in m_Handlers)
                handler.EndBatch();
        }
        public static void EndBatch(out int outputCount, out int warningCount, out int errorCount)
        {
            outputCount = m_BatchOutputCount;
            warningCount = m_BatchWarningCount;
            errorCount = m_BatchErrorCount;

            EndBatch();
        }

        public static int TotalOutputCount
        {
            get { return m_TotalOutputCount; }
        }
        public static int TotalWarningCount
        {
            get { return m_TotalWarningCount; }
        }
        public static int TotalErrorCount
        {
            get { return m_TotalErrorCount; }
        }

        private static List<ILogHandler> m_Handlers = new List<ILogHandler>();
        private static int m_BatchOutputCount = 0;
        private static int m_BatchWarningCount = 0;
        private static int m_BatchErrorCount = 0;
        private static int m_TotalOutputCount = 0;
        private static int m_TotalWarningCount = 0;
        private static int m_TotalErrorCount = 0;
    }
}
