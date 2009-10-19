using System;
using System.Collections.Generic;
using System.Text;


namespace Perforce
{
    public class ClientSpec
    {
        public ClientSpec( string client )
        {
            m_Client	= client;
            m_View		= new List<string>();
        }


        public string Client
        {
            get { return m_Client; }
            set { m_Client = value; }
        }


        public string UpdateDate
        {
            get { return m_UpdateDate; }
            set { m_UpdateDate = value; }
        }


        public string AccessDate
        {
            get { return m_AccessDate; }
            set { m_AccessDate = value; }
        }


        public string Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }


        public string Host
        {
            get { return m_Host; }
            set { m_Host = value; }
        }


        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }


        public string Root
        {
            get { return m_Root; }
            set { m_Root = value; }
        }


        public bool AllWrite
        {
            get { return m_AllWrite; }
            set { m_AllWrite = value; }
        }


        public bool Clobber
        {
            get { return m_Clobber; }
            set { m_Clobber = value; }
        }


        public bool Compress
        {
            get { return m_Compress; }
            set { m_Compress = value; }
        }


        public bool Locked
        {
            get { return m_Locked; }
            set { m_Locked = value; }
        }


        public bool ModTime
        {
            get { return m_ModTime; }
            set { m_ModTime = value; }
        }


        public bool RmDir
        {
            get { return m_RmDir; }
            set { m_RmDir = value; }
        }


        public string LineEnd
        {
            get { return m_LineEnd; }
            set { m_LineEnd = value; }
        }


        public List<string> View
        {
            get { return m_View; }
            set { m_View = value; }
        }


        private string 			m_Client;
        private string 			m_UpdateDate;
        private string 			m_AccessDate;
        private string 			m_Owner;
        private string 			m_Host;
        private string 			m_Description;
        private string 			m_Root;
        private bool			m_AllWrite;		// If set, unopened files on the client are left writable.
        private bool			m_Clobber;		// If set, a p4 sync overwrites ("clobbers") writable-but-unopened files in the client that have the same name as the newly-synced files
        private bool			m_Compress;		// If set, the data stream between the client and the server is compressed. (Both client and server must be version 99.1 or higher, or this setting is ignored.)
        private bool			m_Locked;		
        private bool			m_ModTime;
        private bool			m_RmDir;		// If set, p4 sync deletes empty directories in a client if all files in the directory have been removed.
        private string			m_LineEnd;
        private List<string>	m_View;
    }
}
