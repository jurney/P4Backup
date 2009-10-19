using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RelicLogging
{
    class StatusBarHandler : ILogHandler
    {
        public StatusBarHandler(ToolStripStatusLabel warningsLabel, ToolStripStatusLabel errorsLabel, ToolStripStatusLabel messageLabel)
        {
            m_WarningsLabel = warningsLabel;
            m_ErrorsLabel = errorsLabel;
            m_MessageLabel = messageLabel;
        }

        #region ILogHandler Members
        public void HandleLog(MessageType messageType, string message)
        {
            switch (messageType)
            {
                case MessageType.Error:
                    {
                        if (m_ErrorsLabel != null)
                            m_ErrorsLabel.Text = Log.TotalErrorCount.ToString();

                        if (m_MessageLabel != null)
                            m_MessageLabel.Text = "Error: " + message;
                    }
                    break;
                case MessageType.Output:
                    {
                        if (m_MessageLabel != null)
                            m_MessageLabel.Text = message;
                    }
                    break;
                case MessageType.Warning:
                    {
                        if (m_WarningsLabel != null)
                            m_WarningsLabel.Text = Log.TotalWarningCount.ToString();

                        if (m_MessageLabel != null)
                            m_MessageLabel.Text = "Warning: " + message;
                    }
                    break;
            }
        }

        public void BeginBatch() { }
        public void EndBatch() { }
        #endregion

        private ToolStripStatusLabel m_WarningsLabel;
        private ToolStripStatusLabel m_ErrorsLabel;
        private ToolStripStatusLabel m_MessageLabel;
    }
}
