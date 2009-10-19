using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RelicLogging;

namespace Perforce
{
    public static class P4Shell
	{
		public enum Action
		{
			NoAction,
			Add,
			Edit,
			Delete,
			Branch,
			Integrate
		}
		public enum Operation
		{
			Normal,
			TestOnly
		}		

		/// <summary>
		/// Initialize P4Shell with the current system defaults.
		/// </summary>
		// Example 'p4 info' output:
		// User name: beau.brennen
		// Client name: R-BBRENNEN0372
		// Client host: R-BBRENNEN0372
		// Client root: d:\
		// Current directory: u:\
		// Client address: 10.5.3.166:4236
		// Server address: relp401.thqinc.com:1676
		// Server root: D:\P4DOW2_DB
		// Server date: 2007/06/12 14:14:13 -0700 Pacific Daylight Time
		// Server version: P4D/NTX64/2006.1/104454 (2006/08/02)
		// Server license: THQ (includes Locomotive Games, Helixe, Relic, Rainbow Studios, Volition, Concrete Games, Blue Tongue, Heavy Iron, Cranky Pants Games and Cobalt Studios) 201 users (support expired 2006/10/12)		
		public static void Initialize()
		{
			if( P4Shell.Online == false )
				return;

			try
			{
                System.Diagnostics.Process process = new System.Diagnostics.Process();

				process.StartInfo.FileName					= "p4";
				process.StartInfo.Arguments					= "info";
				process.StartInfo.CreateNoWindow			= true;
				process.StartInfo.RedirectStandardOutput	= true;
				process.StartInfo.RedirectStandardError		= true;				
				process.StartInfo.UseShellExecute			= false;
				process.Start();

				m_Output	= process.StandardOutput.ReadToEnd();
				m_Error		= process.StandardError.ReadToEnd();

				process.WaitForExit();
				process.Close();
			}
			catch
			{
				return;
			}

			if( m_Error.Length > 0 )
				return;

			string[] output = m_Output.Split( new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries );

			for( int lineIndex=0; lineIndex<output.Length; ++lineIndex )			
			{
				string line = output[lineIndex];
								
				if( line.StartsWith( "User name:" ) )
				{					
					m_User = line.Substring( "User name:".Length + 1 );
				}
				else if( line.StartsWith( "Client name:" ) )
				{					
					m_Client = line.Substring( "Client name:".Length + 1 );
				}
				else if( line.StartsWith( "Server address:" ) )
				{					
					m_Port = line.Substring( "Server address:".Length + 1 );
				}
			}
		}
		/// <summary>
		/// Ping's the Perforce server using the current P4Shell settings.
		/// </summary>
		/// <returns>Success or failure.</returns>
		public static bool PingServer()
		{
			if( P4Shell.Online == false )
				return true;

			Execute( "info", null );

			return m_Error.Length <= 0;
		}
		public static List<Changelist> GetPendingChangelists()
		{
			List<Changelist> changelists = new List<Changelist>();

			string arguments = "changes -s pending";

			if( m_Client != null )
				arguments += string.Format( " -c {0}", m_Client );

			if( m_User != null )
				arguments += string.Format( " -u {0}", m_User );

			Execute( arguments, null );

			if( m_Error.Length > 0 )
				return changelists;

			// Change 92703 on 2006/04/20 by beau.brennen@R-BBRENNEN0372 'Added the ability for modifiers'
			// Change 92697 on 2006/04/20 by beau.brennen@R-BBRENNEN0372 'Minefield extention is no longe'
			// Change 92618 on 2006/04/18 by beau.brennen@R-BBRENNEN0372 'Abilities can now spawn entitie'

			List<string> changelistDescList = new List<string>();
			changelistDescList.AddRange( m_Output.Split( new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries ) );

			foreach( string changelistDesc in changelistDescList )
			{				
				// Grab the index between the first two spaces.
				int		firstSpaceIndex		= changelistDesc.IndexOf( ' ' ) + 1;
				int		secondSpaceIndex	= changelistDesc.IndexOf( ' ', firstSpaceIndex + 1 );
				string	changelistIDString	= changelistDesc.Substring( firstSpaceIndex, secondSpaceIndex - firstSpaceIndex );
				int		changelistID		= Convert.ToInt32( changelistIDString );
				
				Changelist changelist = P4Shell.GetChangelist( changelistID );

				if( changelist != null )
				{
					changelists.Add( changelist );
				}				
			}

			// The default changelist is not include in that list, so we have to query for it manually.
			Changelist defaultChangelist = P4Shell.GetChangelist( P4Shell.DefaultChangelistID );

			if( defaultChangelist != null )
			{
				changelists.Add( defaultChangelist );
			}

			return changelists;
		}
		public static List<ClientSpec> GetClientSpecs()
		{
			List<ClientSpec> clientSpecList = new List<ClientSpec>();
						
			List<string> clientList = P4Shell.GetClients();
						
			foreach( string client in clientList )
			{
				ClientSpec clientSpec = new ClientSpec( client );

				if( GetClientSpec( client, ref clientSpec ) )
				{
					clientSpecList.Add( clientSpec );
				}
			}

			return clientSpecList;
		}
		public static List<string> GetClients()
		{
			List<string> clientSpecs = new List<string>();

			// Example 'clients' outputs:
			// Client clientname moddate root clientroot description
			// Client R-BBRENNEN0372 2007/08/14 root d:\ 'Created by beau.brennen. '
			
			Execute( "clients", null );

			if( m_Error.Length > 0 )
				return clientSpecs;

			// Each description is on a seperate line.
			List<string> clientSpecDescriptionList = new List<string>();
			clientSpecDescriptionList.AddRange( m_Output.Split( new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries ) );

			foreach( string clientSpecDescription in clientSpecDescriptionList )
			{				
				// Grab the client between the first two spaces.
				int		firstSpaceIndex		= clientSpecDescription.IndexOf( ' ' ) + 1;
				int		secondSpaceIndex	= clientSpecDescription.IndexOf( ' ', firstSpaceIndex + 1 );
				string	client				= clientSpecDescription.Substring( firstSpaceIndex, secondSpaceIndex - firstSpaceIndex );
				
				clientSpecs.Add( client );
			}
			
			return clientSpecs;
		}
		public static bool GetClientSpec( string client, ref ClientSpec clientSpec )
		{	
			Execute( string.Format( "client -o {0}", client ), null );

			if( m_Error.Length > 0 )
				return false;			

			string[] output = m_Output.Split( new string[]{"\r\n"}, StringSplitOptions.None );

			for( int lineIndex=0; lineIndex<output.Length; ++lineIndex )			
			{
				string line = output[lineIndex];

				if( line.Length == 0 )
					continue;

				if( line.StartsWith( "#" ) )
					continue;

				if( line.StartsWith( "Update:" ) )
				{
					clientSpec.UpdateDate = line.Substring( line.IndexOf( '\t' ) + 1 );
				}
				else if( line.StartsWith( "Access:" ) )
				{
					clientSpec.AccessDate = line.Substring( line.IndexOf( '\t' ) + 1 );
				}
				else if( line.StartsWith( "Owner:" ) )
				{
					clientSpec.Owner = line.Substring( line.IndexOf( '\t' ) + 1 );
				}
				else if( line.StartsWith( "Host:" ) )
				{
					clientSpec.Host = line.Substring( line.IndexOf( '\t' ) + 1 );
				}
				else if( line.StartsWith( "Description:" ) )
				{
					// The next line holds our description.
					// Currently we assume a 1 line description.
					++lineIndex;
					clientSpec.Description = output[lineIndex].Substring( output[lineIndex].IndexOf( '\t' ) + 1 );					
				}
				else if( line.StartsWith( "Root:" ) )
				{					
					clientSpec.Root = line.Substring( line.IndexOf( '\t' ) ).Trim();
				}
				else if( line.StartsWith( "Options:" ) )
				{	
					string[] options = line.Substring( line.IndexOf( '\t' ) + 1 ).Split( new char[]{ ' ' } );

					clientSpec.AllWrite		= !options[0].StartsWith( "no" );
					clientSpec.Clobber		= !options[1].StartsWith( "no" );
					clientSpec.Compress		= !options[2].StartsWith( "no" );
					clientSpec.Locked		= !options[3].StartsWith( "un" );
					clientSpec.ModTime		= !options[4].StartsWith( "no" );
					clientSpec.RmDir		= !options[5].StartsWith( "no" );
				}
				else if( line.StartsWith( "LineEnd:" ) )
				{					
					clientSpec.LineEnd = line.Substring( line.IndexOf( '\t' ) + 1 );
				}
				else if( line.StartsWith( "View:" ) )
				{
					// The following lines hold our views.
					int viewCount = 0;
					while( line.Length > 0 )
					{
						++lineIndex;
						line = output[lineIndex];

						clientSpec.View.Add( line.Trim() );
						++viewCount;
					}
				}
			}

			return true;
		}
		// Example 'p4 fstat' output:
		// ... depotFile //DOW2/Tech/ASSETS/Simulation/Attrib/Templates/position.xml
		// ... clientFile c:\DOW2\ASSETS\Simulation\Attrib\Templates\position.xml
		// ... isMapped
		// ... headAction branch
		// ... headType text
		// ... headTime 1178746388
		// ... headRev 1
		// ... headChange 8283
		// ... headModTime 1174929670
		// ... haveRev 1
		// ... action edit
		// ... change default
		// ... type text
		// ... actionOwner beau.brennen
		// ... ... otherOpen0 beau.brennen@R-BBRENNEN0372-2
		// ... ... otherAction0 edit
		// ... ... otherChange0 default
		// ... ... otherLock0 beau.brennen@R-BBRENNEN0372-2
		// ... ... otherOpen1 beau.brennen@R-BBRENNEN0372-3
		// ... ... otherAction1 edit
		// ... ... otherChange1 default
		// ... ... otherOpen 2
		// ... ... otherLock		
		public static List<FileStats> GetFileStats( string filePath )
		{
			List<FileStats> fileStats = new List<FileStats>();

			if( P4Shell.Online == false )
				return fileStats;

			Execute( string.Format( "fstat {0}", filePath ), null );			

			FileStats curFileStats = null;

            string[] errors = m_Error.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            for (int lineIndex = 0; lineIndex < errors.Length; ++lineIndex)
            {
                string line = errors[lineIndex];

                if (line.Contains(" - no such file(s)."))
                {
                    // This indicates the file isn't in p4, so make an unaddded file entry
                    curFileStats = new FileStats();
                    fileStats.Add(curFileStats);

                    int valueIndex = line.IndexOf(" - no such file(s).");
                    curFileStats.UnaddedFilename = line.Substring(0, valueIndex);
                    curFileStats = null;    // Don't accidentally populate this with more data, since this is the only line relating to this file
                }
            }

			string[] output = m_Output.Split( new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries );

			for( int lineIndex=0; lineIndex<output.Length; ++lineIndex )			
			{
				string line = output[lineIndex];
								
                if (line.Contains(" - no such file(s)."))
                {
                    // This indicates the file isn't in p4, so make an unaddded file entry
                    curFileStats = new FileStats();
                    fileStats.Add(curFileStats);

                    int valueIndex = line.IndexOf(" - no such file(s).");
                    curFileStats.UnaddedFilename = line.Substring(0, valueIndex);
                    curFileStats = null;    // Don't accidentally populate this with more data, since this is the only line relating to this file
                }
                else if (line.Contains("depotFile"))
				{
					// "depotFile" is the first line of every fstat returned.
					// Therefore create a new FileStats object and update our current index.
					curFileStats = new FileStats();
					fileStats.Add( curFileStats );					

					int valueIndex = line.IndexOf( "depotFile" ) + "depotFile".Length + 1;
					curFileStats.DepotFile = line.Substring( valueIndex );
				}
				else if( line.Contains( "clientFile" ) )
				{
					int valueIndex = line.IndexOf( "clientFile" ) + "clientFile".Length + 1;
					curFileStats.ClientFile = line.Substring( valueIndex );
				}
				else if( line.Contains( "headAction" ) )
				{
					curFileStats.HeadAction = StringToAction( line.Substring( line.LastIndexOf( ' ' ) + 1 ) );
				}
				else if( line.Contains( "headRev" ) )
				{
					curFileStats.HeadRev = int.Parse( line.Substring( line.LastIndexOf( ' ' ) + 1 ) );
				}
				else if( line.Contains( "headChange" ) )
				{
					curFileStats.HeadChange = int.Parse( line.Substring( line.LastIndexOf( ' ' ) + 1 ) );
				}
				else if( line.Contains( "haveRev" ) )
				{
					curFileStats.HaveRev = int.Parse( line.Substring( line.LastIndexOf( ' ' ) + 1 ) );
				}
				else if( line.Contains( "action " ) )
				{
					int valueIndex = line.IndexOf( "action" ) + "action".Length + 1;
					string actionName = line.Substring( valueIndex );
					curFileStats.OurAction = StringToAction( actionName );
				}
				else if( line.Contains( "change " ) )
				{
					string changelistIDString = line.Substring( line.LastIndexOf( ' ' ) + 1 );
					if( string.Compare( changelistIDString, "default", true ) == 0 )
						curFileStats.OurChangelistID = P4Shell.DefaultChangelistID;
					else
						curFileStats.OurChangelistID = int.Parse( changelistIDString );
				}
				else if( line.Contains( "ourLock" ) )
				{	
					curFileStats.OurLock = true;
				}
				else if( line.Contains( "otherOpen " ) )
				{
					// Note: This conditional must come before the 'otherOpen' (with no space) conditional.					
				}
				else if( line.Contains( "otherOpen" ) )
				{	
					int otherIndex = FileStats.Other.GetIndex( line, "otherOpen" );

					// Ensure we have enough 'others' allocated.
					while( curFileStats.Others.Count <= otherIndex )
						curFileStats.Others.Add( new FileStats.Other() );

					curFileStats.Others[otherIndex].User = line.Substring( line.LastIndexOf( ' ' ) + 1 );
				}
				else if( line.Contains( "otherAction" ) )
				{					
					int otherIndex = FileStats.Other.GetIndex( line, "otherAction" );
					string actionName = line.Substring( line.LastIndexOf( ' ' ) + 1 );
					curFileStats.Others[otherIndex].Action = StringToAction( actionName );
				}
				else if( line.Contains( "otherChange" ) )
				{					
					int		otherIndex	= FileStats.Other.GetIndex(line , "otherChange");
					string	changelist	= line.Substring( line.LastIndexOf( ' ' ) + 1 );

					curFileStats.Others[otherIndex].ChangelistID = ( changelist == "default" ? -1 : int.Parse( changelist ) );						
				}
				else if( line.Contains( "otherLock" ) && line.Contains( "otherLock " ) == false )
				{
					int otherIndex = FileStats.Other.GetIndex( line, "otherLock" );
					curFileStats.LockedByOtherIndex = otherIndex;
				}
				else if( line.Contains( "resolveFromFile0" ) )	// note: this index may change, I'm not entirely sure what it's for.
				{
					int valueIndex = line.IndexOf( "resolveFromFile0" ) + "resolveFromFile0".Length + 1;
					curFileStats.ResolveFromFile = line.Substring( valueIndex );
				}			
			}	
		
			return fileStats;
		}
		// Example 'p4 change' output:
		// # A Perforce Change Specification.
		// #
		// #  Change:      The change number. 'new' on a new changelist.  Read-only.
		// #  Date:        The date this specification was last modified.  Read-only.
		// #  Client:      The client on which the changelist was created.  Read-only.
		// #  User:        The user who created the changelist. Read-only.
		// #  Status:      Either 'pending' or 'submitted'. Read-only.
		// #  Description: Comments about the changelist.  Required.
		// #  Jobs:        What opened jobs are to be closed by this changelist.
		// #               You may delete jobs from this list.  (New changelists only.)
		// #  Files:       What opened files from the default changelist are to be added
		// #               to this changelist.  You may delete files from this list.
		// #               (New changelists only.)
		//
		// Change:	9212
		//
		// Date:	2007/05/25 11:06:09
		//
		// Client:	R-BBRENNEN0372-2
		//
		// User:	beau.brennen
		//
		// Status:	pending
		//
		// Description:
		//	Hello World.
		//	Is there anybody out there?
		//
		// Files:
		//	//DOW2/Tech/Assets/Simulation/Attrib/instances/artillery_shell.xml	# edit
		//	//DOW2/Tech/ASSETS/Simulation/Attrib/instances/assault_marine.xml	# edit
		//
		public static Changelist GetChangelist( int changelistID )
		{
			if( P4Shell.Online == false )
				return null;
                
			if( changelistID <= 0 )
			{
				Execute( "change -o", null );
			}
			else
			{
				Execute( string.Format( "change -o {0}", changelistID ), null );				
			}

			if( m_Error.Length != 0 )
				return null;
			
			Changelist changelist = new Changelist();

			string[] output = m_Output.Split( new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries );

			for( int lineIndex=0; lineIndex<output.Length; ++lineIndex )
			{
				string line = output[lineIndex];

				if( line.StartsWith( "#" ) )
					continue;

				if( line.StartsWith( "Change:" ) )
				{
					string changelistIDString = line.Substring( line.IndexOf( '\t' ) + 1 );

					if( changelistIDString == "new" )
					{
						changelist.ChangelistID = P4Shell.DefaultChangelistID;
					}
					else
					{
						changelist.ChangelistID = int.Parse( changelistIDString );
					}
				}
				else if( line.StartsWith( "Date:" ) )
				{
					changelist.Date = line.Substring( line.IndexOf( '\t' ) + 1 );
				}
				else if( line.StartsWith( "Client:" ) )
				{
					changelist.Client = line.Substring( line.IndexOf( '\t' ) + 1 );
				}
				else if( line.StartsWith( "User:" ) )
				{
					changelist.User = line.Substring( line.IndexOf( '\t' ) + 1 );
				}
				else if( line.StartsWith( "Status:" ) )
				{
					Changelist.Status status = Changelist.StringToStatus( line.Substring( line.IndexOf( '\t' ) + 1 ) );
					changelist.CurrentStatus = status;
				}
				else if( line.StartsWith( "Description:" ) )
				{
					++lineIndex;

                    StringBuilder description = new StringBuilder(1024);

					// Each line in the description starts with a tab. So read every line until the end of the file,
					// or until the line doesn't start with a tab.
					while( lineIndex < output.Length )
					{
						line = output[lineIndex];

						if( line[0] != '\t' )
						{
							// Reset the line index back one, so that when the surrouding
							// for loop increments it, we'll read this line again.
							--lineIndex;
							break;
						}

                        description.AppendLine(line.Substring( line.IndexOf( '\t' ) + 1 ));

						++lineIndex;
					}									
										
					changelist.Description = description.ToString();
				}
				else if( line.StartsWith( "Files:" ) )
				{
					++lineIndex;

					// Each line starts with a tab. So read every line until the end of the file,
					// or until the line doesn't start with a tab.
					while( lineIndex < output.Length )
					{
						line = output[lineIndex];

						if( line[0] != '\t' )
						{
							// Reset the line index back one, so that when the surrouding
							// for loop increments it, we'll read this line again.
							--lineIndex;
							break;
						}

						// Each line also ends with a tab, and the file operation, which we're not concerned with yet.
						int firstTabIndex	= line.IndexOf( '\t' );
						int lastTabIndex	= line.LastIndexOf( '\t' );
						string depotFilePath = line.Substring( firstTabIndex + 1, lastTabIndex - firstTabIndex - 1 );

						changelist.AddDepotFilePath( depotFilePath );

						++lineIndex;
					}
				}
			}

			return changelist;
		}
		/// <summary>
		/// Finds a list of changelists whose first 31 characters of the description match the given description, case insensitive.
		/// </summary>
		/// <param name="descriptionStartsWith">The given description.</param>
		/// <returns>A list of matching changelists.</returns>
		// Example 'p4 changes' output:
		// Change 10570 on 2007/06/08 by beau.brennen@R-BBRENNEN0372 *pending* 'Attribute Editor Changes '
		// Change 10283 on 2007/06/06 by beau.brennen@R-BBRENNEN0372 *pending* 'Adding gfxexport command-line t'
        public static List<Changelist> FindChangelist( string descriptionStartsWith )
		{	
			List<Changelist> changelists = new List<Changelist>();

			if( P4Shell.Online == false )
				return changelists;

            Execute(string.Format("changes -s pending -u {0} -c {1}", User, Client), null);

			if( m_Error.Length > 0 )
				return changelists;			

			string[] output = m_Output.Split( new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries );

			for( int lineIndex=0; lineIndex<output.Length; ++lineIndex )
			{
				string line = output[lineIndex];

				int index = line.IndexOf( "*pending* '" );

				if( index == -1 )
					continue;

				string description = line.Substring( index + "*pending* '".Length );

				if( description.StartsWith( descriptionStartsWith, StringComparison.CurrentCultureIgnoreCase ) )
				{
					// Grab the changelist ID between the first two spaces.
					int		firstSpaceIndex		= line.IndexOf( ' ' ) + 1;
					int		secondSpaceIndex	= line.IndexOf( ' ', firstSpaceIndex + 1 );
					string	changelistIDString	= line.Substring( firstSpaceIndex, secondSpaceIndex - firstSpaceIndex );

					try
					{
						int changelistID = int.Parse( changelistIDString );
						Changelist changelist = P4Shell.GetChangelist( changelistID );
						if( changelist != null )
							changelists.Add( changelist );
					}
					catch
					{
						continue;
					}
				}				
			}

			return changelists;
		}
		/// <summary>
		/// Creates a new changelist with the given description.
		/// </summary>
		/// <param name="description">The description of the changelist.</param>
		/// <returns>The changelist ID of the new changelist, or -1 on failure.</returns>
		public static bool CreateChangelist( string description, ref int changelistID )
		{
			if( P4Shell.Online == false )
				return true;

			if( description == null )
				description = "";

			// Populate our output with a 'new changelist' form.
			Execute( string.Format( "change -o" ), null );

			if( m_Error.Length != 0 )
				return false;

			// Replace the default description with the given description.			
			string input = m_Output.Replace( "<enter description here>", description );

			// Any files on the default changelist will be listed in this changelist.
			// We do not want to move them from the default changelist, so remove them
			// from the changelist description.
			int filesIndex = input.IndexOf( "Files:\r\n" );
			if( filesIndex != -1 )
			{
				int endOfFilesIndex = input.IndexOf( "\r\n\r\n", filesIndex );
				input = input.Substring( 0, filesIndex ) + input.Substring( endOfFilesIndex + 4 );
			}

			Execute( string.Format( "change -i" ), input );

			if( m_Error.Length > 0 )
				return false;
			
			// "Change 10330 created."
			// Get the integer between the two spaces.
			int firstSpaceIndex		= m_Output.IndexOf( ' ' );
			int secondSpaceIndex	= m_Output.LastIndexOf( ' ' );

			if( firstSpaceIndex == -1 || secondSpaceIndex == -1 || firstSpaceIndex == secondSpaceIndex )
			{
				m_Error = "Error reading Perforce changelist creation output.";
				return false;
			}

			string changelistIDString = m_Output.Substring( firstSpaceIndex + 1, secondSpaceIndex - firstSpaceIndex - 1 );

			changelistID = int.Parse( changelistIDString );

			if( changelistID <= 0 )
			{
				m_Error = "Error reading Perforce changelist ID.";
				return false;
			}

			return true;
		}

                // Pass -1 for all changelists, or specify a changelist number to only get files from that changelist
        public static List<FileStats> GetOpenedFileStats(int optionalChangelist)
        {
            if (P4Shell.Online == false)
                return new List<FileStats>();

            Execute("opened", null);

            if (m_Error.Length > 0)
                return new List<FileStats>();

            List<string> outputStrings = new List<string>();
            outputStrings.AddRange(m_Output.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

            string filesToGetStatsOn = "";

            foreach (string outputString in outputStrings)
            {
                int hashIndex = outputString.IndexOf('#');
                int firstSpaceIndex = outputString.IndexOf(' ', hashIndex);
                int secondSpaceIndex = outputString.IndexOf(' ', firstSpaceIndex + 1);
                int thirdSpaceIndex = outputString.IndexOf(' ', secondSpaceIndex + 1);
                int fourthSpaceIndex = outputString.IndexOf(' ', thirdSpaceIndex + 1);
                int fifthSpaceIndex = outputString.IndexOf(' ', fourthSpaceIndex + 1);

                int changelistID = -1;

                string possibleDefaultString = outputString.Substring(thirdSpaceIndex + 1, fourthSpaceIndex - thirdSpaceIndex - 1).ToLower();
                if (possibleDefaultString != "default")
                {
                    string changelistIDString = outputString.Substring(fourthSpaceIndex + 1, fifthSpaceIndex - fourthSpaceIndex - 1);
                    changelistID = Convert.ToInt32(changelistIDString);
                }

                if ((optionalChangelist != -1) && (changelistID != optionalChangelist))
                {
                    continue;
                }

                filesToGetStatsOn += outputString.Substring(0, hashIndex) + " ";
            }

            return GetFileStats(filesToGetStatsOn);
        }

        // Pass -1 for all changelists, or specify a changelist number to only get files from that changelist
        public static List<string> GetOpenedFiles( int optionalChangelist )
        {
            List<string> openedFiles = new List<string>();

            List<FileStats> filestatList = GetOpenedFileStats(optionalChangelist);

            foreach (FileStats filestat in filestatList)
            {
                if (filestat.ClientFile != "")
                {
                    openedFiles.Add(filestat.ClientFile);
                }
            }

            return openedFiles;
        }

		public static bool Add( string filePath, int changelistID )
		{
			if( P4Shell.Online == false )
				return true;

			string command = "add";

			if( changelistID > P4Shell.DefaultChangelistID )
				command += string.Format( " -c {0}", changelistID.ToString() );

			command += " " + filePath;

			Execute( command, null );

			if( m_Error.Length != 0 )
				return false;

			return true;
		}
		public static bool Edit( string filePath, int changelistID )
		{
			if( P4Shell.Online == false )
				return true;

			string command = "edit";

			if( changelistID > P4Shell.DefaultChangelistID )
				command += string.Format( " -c {0}", changelistID.ToString() );

			command += " " + filePath;

			Execute( command, null );

			if( m_Error.Length != 0 )
				return false;

			return true;
		}
		public static bool Integrate( string fromFile, string toFile, Operation operation )
		{			
			if( P4Shell.Online == false )
				return true;

			string command = "integrate";

			if( m_WorkingChangelistID > 0 )
				command += string.Format( " -c {0}", m_WorkingChangelistID );

			if( operation == Operation.TestOnly )
				command += " -n";

			command += string.Format( " {0} {1}", fromFile, toFile );

			Execute( command, null );

			if( m_Error.Length != 0 )
				return false;

			return true;
		}
		public static bool Revert( string filePath )
		{
			if( P4Shell.Online == false )
				return true;

			string command = string.Format( "revert {0}", filePath );

			Execute( command, null );

			if( m_Error.Length != 0 )
				return false;

			return true;
		}		
		public static bool Reopen( string filePath, int newChangelistID )
		{
			if( P4Shell.Online == false )
				return true;

			string command = "reopen -c ";

			if( newChangelistID <= P4Shell.DefaultChangelistID )
				command += "default ";
			else
				command += newChangelistID.ToString();

			command += " " + filePath;

			Execute( command, null );

			return ( m_Error.Length == 0 );
		}
		/// <summary>
		/// Submits the given changelist, and updates it's description if provided.
		/// Note that when submitting the default changelist, you must always provide
		/// a description.
		/// </summary>
		/// <param name="changelistID">The changelistID to submit.</param>
		/// <param name="description">The changelist description. For changelists other than the default changelist,
		/// this can be null, in which case the existing changelist description will be used.</param>
		/// <returns>Success/Failure. See the Error property for more information.</returns>
		public static bool Submit( int changelistID, string description )
		{
			if( P4Shell.Online == false )
				return true;

			if( changelistID <= P4Shell.DefaultChangelistID && ( description == null || description.Length <= 0 ) )
			{
				m_Error = "A valid non-zero length description must be provided when submitting the default changelist.";
				return false;
			}

			if( description == null )
			{				
				Execute( string.Format( "submit -c {0}", changelistID ), null );
				return ( m_Error.Length == 0 );
			}
			else
			{
				// Populate our output with a changelist form.
				string command = "change -o";

				// Get the current information if we're submitting a previously created changelist.
				if( changelistID > P4Shell.DefaultChangelistID )
					command += " " + changelistID.ToString();

				Execute( command, null );

				if( m_Error.Length != 0 )
					return false;

				// Replace the default description with the given description.			
				string input = m_Output.Replace( "<enter description here>", description );

				// We have to specify the description through the standard input.
				command = "submit -i";

				if( changelistID > P4Shell.DefaultChangelistID )
					command += " " + changelistID.ToString();

				Execute( command, input );

				return ( m_Error.Length == 0 );
			}				
		}
		public static bool Delete( string filePath, int changelistID )
		{
			if( P4Shell.Online == false )
				return true;

			string command = "delete";

			if( changelistID > P4Shell.DefaultChangelistID )				
				command += " -c " + changelistID.ToString();

			command += " " + filePath;

			Execute( command, null );

			if( m_Error.Length != 0 )
				return false;

			return true;
		}
		public static bool Sync( string filePath )
		{
			if( P4Shell.Online == false )
				return true;

			string command = string.Format( "sync {0}", filePath );

			Execute( command, null );

			if( m_Error.Length != 0 )
				return false;

			return true;
		}
		/// <summary>
		/// This utility method will attempt whatever is necessary to get the given
		/// file path into an editable state as far as Perforce is concerned.
		/// </summary>
        public static void EasyEdit(List<FileStats> fileStats, int changelistID, bool reopenDeleted)
        {
            foreach (FileStats fileStat in fileStats)
		{
			if( P4Shell.Online )
			{				
                    if (fileStat.UnaddedFilename != null && fileStat.UnaddedFilename.Length > 0)
                    {
                        if (P4Shell.Add(fileStat.UnaddedFilename, changelistID) == false)
				{
                            Log.Error("P4 error adding file {0}. {1}", fileStat.UnaddedFilename, P4Shell.Error);
				}			
				else
				{
                            fileStat.ClientFile = fileStat.UnaddedFilename;
                        }
                    }
                    else
                    {
                        switch (fileStat.OurAction)
					{						
						case Action.NoAction:
						{
                                    if (fileStat.HeadAction == Action.Delete)
							{
								if (reopenDeleted)
								{
									// We need to sync to the previous version before opening for add again, 
									// so that Perforce will know have a file to work with in order to 
									// figure out the file type (text vs binary). This is what 
									// P4Win does when you "Recover Deleted File".
									// Note that the file will still be read-only, but that will be handled below.
                                            string versionFilePath = fileStat.ClientFile + "#" + (fileStat.HeadRev - 1).ToString();
                                            if (P4Shell.Sync(versionFilePath) == false || P4Shell.Add(fileStat.ClientFile, changelistID) == false)
									{
                                                Log.Error("P4 error reopening file {0}. {1}", fileStat.ClientFile, P4Shell.Error);
									}
								}
							}
							else
							{
                                        if (P4Shell.Edit(fileStat.ClientFile, changelistID) == false)
								{
                                            Log.Error("P4 error editing file {0}. {1}", fileStat.ClientFile, P4Shell.Error);
								}
							}
						} break;					

						case Action.Integrate:
						case Action.Branch:
						{
							// If we try to open a file for edit that is already open on a different changelist,
							// the operation will fail. Therefore, reopen the file on the target changelist.
                                    if (fileStat.OurChangelistID != changelistID)
							{
                                        if (P4Shell.Reopen(fileStat.ClientFile, changelistID) == false)
								{
                                            Log.Error("P4 error editing file {0}. {1}", fileStat.ClientFile, P4Shell.Error);
								}
							}
							
							// The file is already open on the target changelist, so simply execute the edit command.
                                    if (P4Shell.Edit(fileStat.ClientFile, changelistID) == false)
							{
                                        Log.Error("P4 error editing file {0}. {1}", fileStat.ClientFile, P4Shell.Error);
							}
						} break;

						case Action.Delete:
						{
                                    if (P4Shell.Revert(fileStat.ClientFile) == false || P4Shell.Edit(fileStat.ClientFile, changelistID) == false)
							{
                                        Log.Error("P4 error editing file {0}. {1}", fileStat.ClientFile, P4Shell.Error);
							}
						} break;
						
						case Action.Add:
						case Action.Edit:
						{
							// Files open for add cannot be opened for edit, but we want to make sure they're on the target
							// changelist. Note that if the file is read-only, it will be flagged read-write below.
                                    if (fileStat.OurChangelistID != changelistID)
							{
                                        if (P4Shell.Reopen(fileStat.ClientFile, changelistID) == false)
								{
                                            Log.Error("P4 error editing file {0}. {1}", fileStat.ClientFile, P4Shell.Error);
								}
							}
						} break;

						default:
                                throw new ApplicationException(string.Format("Unknown P4 Action for file {0}", fileStat.ClientFile));
                        }
                    }
                }

                // Make sure the path exists. If we try to create a file in a folder
                // that doesn't exist, the create will throw an exception.
                string makeWriteableFilename = "";

                if (fileStat.ClientFile != null && fileStat.ClientFile.Length > 0)
                {
                    makeWriteableFilename = fileStat.ClientFile;
                }
                else if (fileStat.UnaddedFilename != null && fileStat.UnaddedFilename.Length > 0)
                {
                    makeWriteableFilename = fileStat.UnaddedFilename;
                }

                if(makeWriteableFilename.Length > 0)
                {
                    int filenameIndex = makeWriteableFilename.LastIndexOf('\\');
                    if (filenameIndex != -1)
                    {
                        string pathToFile = makeWriteableFilename.Substring(0, filenameIndex);
                        if (Directory.Exists(pathToFile) == false)
                            Directory.CreateDirectory(pathToFile);
                    }

                    // Make sure it's writable.
                    SetFileWritable(makeWriteableFilename);
					}					
				}
			}			

		/// <summary>
		/// This utility method will attempt whatever is necessary to get the given
		/// file path into an editable state as far as Perforce is concerned.
		/// </summary>
		/// <param name="filePath">The full file path to edit.</param>
		/// <param name="changelistID">The changelist ID to use if it is necessary to open the file.</param>
		/// <returns>True on success, false on failure.</returns>
		public static void EasyEdit( string filePaths, int changelistID, bool reopenDeleted )
		{
			if( filePaths == null )
				throw new ArgumentNullException( "filePath" );

			if( filePaths.Length <= 0 )
				throw new ArgumentOutOfRangeException( "filePath", "The filePath length must be greater than zero." );

            if (P4Shell.Online)
            {
                List<FileStats> fileStats = P4Shell.GetFileStats(filePaths);

                EasyEdit(fileStats, changelistID, reopenDeleted);
            }
            else
            {
                string[] files = filePaths.Split(new string[] { "\t " }, StringSplitOptions.RemoveEmptyEntries);

                foreach(string filePath in files)
                {
			        // Make sure the path exists. If we try to create a file in a folder
			        // that doesn't exist, the create will throw an exception.
			        int filenameIndex = filePath.LastIndexOf( '\\' );
			        if( filenameIndex != -1 )
			        {
				        string pathToFile = filePath.Substring( 0, filenameIndex );
				        if( Directory.Exists( pathToFile ) == false )
					        Directory.CreateDirectory( pathToFile );
			        }
        
			        // Make sure it's writable.
			        SetFileWritable(filePath);
		        }	
            }
		}	


		public static void EasyDelete( string filePath, int changelistID )
		{
			if( filePath == null )
				throw new ArgumentNullException( "filePath" );

			if( filePath.Length <= 0 )
				throw new ArgumentOutOfRangeException( "filePath", "The filePath length must be greater than zero." );

			if( P4Shell.Online )
			{				
				List<FileStats> fileStats = P4Shell.GetFileStats( filePath );
								
				if( fileStats.Count <= 0 || fileStats[0].OurAction == Action.NoAction )
				{
					// If we don't know about the file, or we haven't touched it yet, just delete it.
					P4Shell.Delete( filePath, changelistID );
				}
				else
				{					
					// Otherwise, if we've currently done anything but delete it, revert that change and delete it.
					if( fileStats[0].OurAction != Action.Delete )
					{
						P4Shell.Revert( filePath );
						P4Shell.Delete( filePath, changelistID );
					}
				}
			}

			// Make sure it's writable, otherwise delete will throw an exception.
			SetFileWritable(filePath);

			// Make sure it's actually gone. This does not throw an exception if the file does not exist.
			try
			{
			File.Delete( filePath );
		}
			catch (Exception)
			{
				// It will actually throw an exception if that folder doesn't exist.
			}
		}

        public static bool IsHeadRevisionDelete(FileStats fileStat)
        {
            return (fileStat.HeadAction == Action.Delete);
        }

		public static bool IsHeadRevisionDelete(string filePath, int changelistID)
		{
			if( filePath == null )
				throw new ArgumentNullException( "filePath" );

			if( filePath.Length <= 0 )
				throw new ArgumentOutOfRangeException( "filePath", "The filePath length must be greater than zero." );

			if (P4Shell.Online)
			{
				List<FileStats> fileStats = P4Shell.GetFileStats(filePath);

                if (fileStats == null || fileStats.Count == 0)
				{
                    return false;
				}

                return IsHeadRevisionDelete(fileStats[0]);
			}
			return false;
		}
		public static Action StringToAction( string actionName )
		{	
			switch( actionName )
			{
				case "add":
					return Action.Add;
					
				case "edit":
					return Action.Edit;

				case "delete":
					return Action.Delete;

				case "branch":
					return Action.Branch;

				case "integrate":
					return Action.Integrate;
			}

			return Action.NoAction;
		}
		public static string ActionToString( Action action )
		{	
			switch( action )
			{
				case Action.Add:
					return "add";
					
				case Action.Edit:
					return "edit";

				case Action.Delete:
					return "delete";

				case Action.Branch:
					return "branch";

				case Action.Integrate:
					return "integrate";
			}

			return "no action";
		}
		private static void Execute( string arguments, string input )
		{
			if( P4Shell.Online == false )
				throw new ApplicationException( "Execute cannot be performed while P4Shell is offline." );

			try
			{
				string globalArguments = "";

				if( m_Port != null )
					globalArguments += string.Format( "-p {0} ", m_Port );

				if( m_Client != null )
					globalArguments += string.Format( "-c {0} ", m_Client );

				if( m_User != null )
					globalArguments += string.Format( "-u {0} ", m_User );

                System.Diagnostics.Process process = new System.Diagnostics.Process();

				process.StartInfo.FileName					= "p4";
				process.StartInfo.Arguments					= string.Format( "{0}{1}", globalArguments, arguments );
				process.StartInfo.CreateNoWindow			= true;
				process.StartInfo.RedirectStandardOutput	= true;
				process.StartInfo.RedirectStandardError		= true;
				process.StartInfo.RedirectStandardInput		= true;
				process.StartInfo.UseShellExecute			= false;
				process.Start();

				if( input != null )				
				{
					process.StandardInput.Write( input );
					process.StandardInput.Close();
				}

				m_Output	= process.StandardOutput.ReadToEnd();
				m_Error		= process.StandardError.ReadToEnd();

				//process.WaitForExit();
				process.Close();
			}
			catch
			{
			}
		}
		public static string Client
		{
			get { return m_Client; }
			set 
			{ 
				if( m_Client != value )
				{
					m_Client = value; 

					if( EventClientChanged != null )
						EventClientChanged( null, EventArgs.Empty );
				}
			}
		}
		public static string User
		{
			get { return m_User; }
			set { m_User = value; }
		}
		public static string Port
		{
			get { return m_Port; }
			set { m_Port = value; }
		}
		public static string Error
		{
			get { return m_Error; }
		}
        public static string Output
        {
            get { return m_Output; }
        }
		public static int WorkingChangelistID
		{
			get { return m_WorkingChangelistID; }
			set { m_WorkingChangelistID = value; }
		}
		public static int DefaultChangelistID
		{
			get { return 0; }
		}
		public static bool Online
		{
			get { return m_Online; }
			set { m_Online = value; }
		}
		public static void SetFileWritable(string filePath)
		{
			// Make sure it exists.
			if (File.Exists(filePath))
			{
				FileAttributes fileAttributes = File.GetAttributes(filePath);
				if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					fileAttributes = fileAttributes &= ~FileAttributes.ReadOnly;
					File.SetAttributes(filePath, fileAttributes);
				}
			}
		}

        public static void SetFilesWriteable(List<string> files)
        {
            foreach(String filename in files)
            {
                SetFileWritable(filename);
            }
        }

		public static void SetFileReadOnly(string filePath)
		{
			// Make sure it exists.
			if (File.Exists(filePath))
			{
				FileAttributes fileAttributes = File.GetAttributes(filePath);
				if ((fileAttributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
				{
					fileAttributes = fileAttributes |= FileAttributes.ReadOnly;
					File.SetAttributes(filePath, fileAttributes);
				}
			}
		}

        public static void SetFilesReadOnly(List<string> files)
        {
            foreach (String filename in files)
            {
                SetFileReadOnly(filename);
            }
        }


		public static event EventHandler	EventClientChanged;
		private static string		m_Client;
		private static string		m_User;
		private static string		m_Port;	
		private static int			m_WorkingChangelistID	= 0;		// If this is set to a valid changelist (i.e. > 0), all operations involving a changelist will attempt to use it.
		private static string		m_Output				= "";
		private static string		m_Error					= "";
		private static bool			m_Online				= true;
	}
}