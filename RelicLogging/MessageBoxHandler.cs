using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RelicLogging
{
    public class MessageBoxHandler : ILogHandler
    {
        public MessageBoxHandler()
        {
            m_ShowWarnings = false;
            m_ShowErrors = true;
        }

        #region ILogHandler Members
        public void HandleLog(MessageType messageType, string message)
        {
            switch (messageType)
            {
                case MessageType.Error:
                    {
                        if (m_ShowErrors)
                        {
                            if (m_BatchErrors != null)
                            {
                                m_BatchErrors.Add(message);
                            }
                            else
                            {
                                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    break;
                case MessageType.Warning:
                    {
                        if (m_ShowWarnings)
                        {
                            if (m_BatchWarnings != null)
                            {
                                m_BatchWarnings.Add(message);
                            }
                            else
                            {
                                MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                    break;
            }
        }

        public void BeginBatch()
        {
            if (m_BatchErrors != null || m_BatchWarnings != null)
            {
                throw new ApplicationException("BeginBatch() called multiple times without a matching EndBatch().");
            }

            m_BatchWarnings = new List<string>();
            m_BatchErrors = new List<string>();
        }
        public void EndBatch()
        {
            string message = "";
            MessageBoxIcon button = MessageBoxIcon.Exclamation;

            if (m_ShowWarnings && m_BatchWarnings.Count > 0)
            {
                message += string.Format("{0} Warnings found.\n", m_BatchWarnings.Count);
            }

            if (m_ShowErrors && m_BatchErrors.Count > 0)
            {
                message += string.Format("{0} Errors found.\n", m_BatchErrors.Count);
            }

            if(message != "")
            {
                message += "Please check the output window for details.";
                MessageBox.Show(message, message, MessageBoxButtons.OK, button);
            }

            m_BatchWarnings = null;
            m_BatchErrors = null;
        }
        #endregion

        public bool ShowWarnings
        {
            get { return m_ShowWarnings; }
            set { m_ShowWarnings = value; }
        }
        public bool ShowErrors
        {
            get { return m_ShowErrors; }
            set { m_ShowErrors = value; }
        }

        private bool m_ShowWarnings;
        private bool m_ShowErrors;
        private List<string> m_BatchWarnings;
        private List<string> m_BatchErrors;
    }
}
