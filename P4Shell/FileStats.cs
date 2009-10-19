using System;
using System.Collections.Generic;
using System.Text;

namespace Perforce
{
    public class FileStats
	{		
		public class Other
		{
			/// <summary>
			/// Given "... ... otherOpen42 beau.brennen@R-BBRENNEN0372" and "otherOpen", return 42.
			/// </summary>
			/// <param name="line">The single line of text to strip the index from.</param>
			/// <param name="action">The action that prepends the index.</param>
			/// <returns>-1 on failure.</returns>
			public static int GetIndex( string line , string action )
			{				
				int otherIndexStart = line.LastIndexOf( action );

				if( otherIndexStart == -1 )
					return -1;

				otherIndexStart += action.Length;

				int otherIndexLength	= line.LastIndexOf( ' ' ) - otherIndexStart;
				int otherIndex			= int.Parse( line.Substring( otherIndexStart, otherIndexLength ) );

				return otherIndex;
			}

			public string User
			{
				get { return m_User; }
				set { m_User = value; }
			}


			public P4Shell.Action Action
			{
				get { return m_Action; }
				set { m_Action = value; }
			}

			
			public int ChangelistID
			{
				get { return m_ChangelistID; }
				set { m_ChangelistID = value; }
			}

			
			private string			m_User;					// "username@workspace"
			private P4Shell.Action	m_Action;
			private int				m_ChangelistID;			// 0 == default changelist.			
		}		


		public FileStats()
		{
			m_Others				= new List<Other>();
			m_HeadRev				= -1;
			m_HeadChange			= -1;
			m_HaveRev				= -1;
			m_OurChangelistID		= -1;
			m_LockedByOtherIndex	= -1;
		}


		#region Properties
		public string DepotFile
		{
			get { return m_DepotFile; }
			set { m_DepotFile = value; }
		}
		

		public string ClientFile
		{
			get { return m_ClientFile; }
			set { m_ClientFile = value; }
		}


		public P4Shell.Action HeadAction
		{
			get { return m_HeadAction; }
			set { m_HeadAction = value; }
		}


		public int HeadRev
		{
			get { return m_HeadRev; }
			set { m_HeadRev = value; }
		}


		public int HeadChange
		{
			get { return m_HeadChange; }
			set { m_HeadChange = value; }
		}


		public int HaveRev
		{
			get { return m_HaveRev; }
			set { m_HaveRev = value; }
		}


		public P4Shell.Action OurAction
		{
			get { return m_OurAction; }
			set { m_OurAction = value; }
		}


		public int OurChangelistID
		{
			get { return m_OurChangelistID; }
			set { m_OurChangelistID = value; }
		}


		public bool OurLock
		{
			get { return m_OurLock; }
			set { m_OurLock = value; }
		}


		public List<Other> Others
		{
			get { return m_Others; }			
		}


		public int LockedByOtherIndex
		{
			get { return m_LockedByOtherIndex; }
			set { m_LockedByOtherIndex = value; }
		}


		public string ResolveFromFile
		{
			get { return m_ResolveFromFile; }
			set { m_ResolveFromFile = value; }
		}

        
        public string UnaddedFilename
        {
            get { return m_UnaddedFilename; }
            set { m_UnaddedFilename = value; }
        }
        #endregion


		private string			m_DepotFile;
		private string			m_ClientFile;
		private P4Shell.Action	m_HeadAction;
		private int				m_HeadRev;
		private int				m_HeadChange;
		private int				m_HaveRev;
		private P4Shell.Action	m_OurAction;				// open action, if opened in your workspace.
		private int				m_OurChangelistID;
		private bool			m_OurLock;					// do we have the file locked.
		private List<Other>		m_Others;					// a list of other people who are currently interacting with this file.
		private int				m_LockedByOtherIndex;		// the index into Others who has this file locked.
		private string			m_ResolveFromFile;			// if this file is in the process of being integrated, this is the file it came from.
        private string          m_UnaddedFilename;          // Filename as passed into tool of unadded file.  If not empty, this file is unadded to p4.
    }
}
