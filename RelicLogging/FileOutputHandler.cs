using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RelicLogging
{
    public class FileOutputHandler : ILogHandler
    {
        public FileOutputHandler()
        {
            m_outputFile = File.CreateText("AttributeEditorXMLLog.txt");
            m_outputFile.AutoFlush = true;
        }

        #region ILogHandler Interface
        public void HandleLog(MessageType messageType, string message)
        {
            switch( messageType )
            {
                case MessageType.Error:
                    m_outputFile.WriteLine("{0}: ERROR {1}", DateTime.Now.ToString(), message);
                    break;
                case MessageType.Output:
                    m_outputFile.WriteLine("{0}: Output {1}", DateTime.Now.ToString(), message);
                    break;
                case MessageType.Warning:
                    m_outputFile.WriteLine("{0}: WARNING {1}", DateTime.Now.ToString(), message);
                    break;
            }
        }

        public void BeginBatch() { }
        public void EndBatch() { }
        #endregion

        StreamWriter m_outputFile;
    }
}
