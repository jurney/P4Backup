using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RelicLogging
{
    public partial class OutputForm : Form, ILogHandler
    {
        public OutputForm()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void OutputForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void buttonCopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.textBoxOutput.Text, TextDataFormat.UnicodeText);
        }

        internal class LogMessage
        {
            public LogMessage(MessageType mt, string m)
            {
                messageType = mt;
                message = m;
            }
            public readonly MessageType messageType;
            public readonly string message;
        }

        private System.Text.StringBuilder m_OutputTextStringBuilder = new StringBuilder();

        private void AddItem(LogMessage message)
        {
			m_OutputTextStringBuilder.Append(message.message);
			m_OutputTextStringBuilder.Append("\r\n");
        }

        #region ILogHandler Members
        public void HandleLog(MessageType messageType, string message)
        {
            LogMessage msg = new LogMessage(messageType, message);

            if (InvokeRequired)
            {
                lock (m_OutputTextStringBuilder)
                {
                    AddItem(msg);
                }
            }
            else
            {
                AddItem(msg);
            }
        }

        public void BeginBatch()
        {
        }
        public void EndBatch()
        {
            UpdateOutput();
        }

        #endregion

        public void UpdateOutput()
        {
            lock (m_OutputTextStringBuilder)
            {
                textBoxOutput.Text = m_OutputTextStringBuilder.ToString();
            }
        }

        private void OutputForm_Shown(object sender, EventArgs e)
        {
            UpdateOutput();
        }
    }
}