using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Perforce
{
    public class Changelist
	{
		public enum Status
		{
			Unknown,
			New,
			Pending,
			Submitted
		}


		public Changelist()
		{
			m_DepotFilePaths = new List<string>();
		}


		public static Status StringToStatus( string statusName )
		{
			switch( statusName )
			{
				case "new":
					return Status.New;
					
				case "pending":
					return Status.Pending;

				case "submitted":
					return Status.Submitted;
			}

			return Status.Unknown;
		}


		public static string StatusToString( Status status )
		{
			switch( status )
			{
				case Status.New:
					return "new";
					
				case Status.Pending:
					return "pending";

				case Status.Submitted:
					return "submitted";
			}

			return "unknown";
		}


		public void AddDepotFilePath( string depotFilePath )
		{
			m_DepotFilePaths.Add( depotFilePath );
		}


		#region Properties
		public int ChangelistID
		{
			get { return m_ChangelistID; }
			set { m_ChangelistID = value; }
		}


		public string Date
		{
			get { return m_Date; }
			set { m_Date = value; }
		}
		

		public string Client
		{
			get { return m_Client; }
			set { m_Client = value; }
		}
		

		public string User
		{
			get { return m_User; }
			set { m_User = value; }
		}
		
		public Status CurrentStatus
		{
			get { return m_Status; }
			set { m_Status = value; }
		}		


		public string Description
		{
			get { return m_Description; }
			set { m_Description = value; }
		}

		public ReadOnlyCollection<string> DepotFilePaths
		{
			get { return m_DepotFilePaths.AsReadOnly(); }
		}
		#endregion


		private int				m_ChangelistID;		// zero means a newly created changelist, or the default changelist.		
		private string			m_Date;
		private string			m_Client;
		private string			m_User;
		private Status			m_Status;
		private string			m_Description;
		private List<string>	m_DepotFilePaths;
	}
}
