using System;
using System.Collections.Generic;
using System.Text;
using Perforce;

namespace P4Backup
{
    class Program
    {
		private static string k_DeletesFilename = "__deletes.txt";

        static void Main(string[] argsOrig)
        {
            List<string> argsCopy = new List<string>();
            bool bDoBackup = false;
            bool bDoRestore = false;
            bool bHelp = false;
            string port = "";
            string client = "";
            string user = "";

            try
            {
                string argsEnv = System.Environment.GetEnvironmentVariable("P4BACKUPOPTS");
                char[] delim = { ' ', '\t' };
                string[] argsEnvSplit = argsEnv.Split(delim);
                argsCopy.AddRange(argsEnvSplit);
            }
            catch (System.Exception)
            {            	
            }

            argsCopy.AddRange(argsOrig);

            string[] args = new string[argsCopy.Count];
            argsCopy.CopyTo(args);

            for (int i = 0; i < args.Length; ++i)
            {
                args[i] = args[i].ToLower();

                if (args[i] == "-backup" || args[i] == "-b")
                {
                    bDoBackup = true;
                }
                else if (args[i] == "-restore" || args[i] == "-r")
                {
                    bDoRestore = true;
                }
                else if (args[i] == "-port" || args[i] == "-p")
                {
                    if (args.Length > i + 1)
                    {
                        port = args[i + 1];
                        ++i;
                    }
                }
                else if (args[i] == "-user" || args[i] == "-u")
                {
                    if (args.Length > i + 1)
                    {
                        user = args[i + 1];
                        ++i;
                    }
                }
                else if (args[i] == "-client" || args[i] == "-c")
                {
                    if (args.Length > i + 1)
                    {
                        client = args[i + 1];
                        ++i;
                    }
                }
                else if ( (args[i] == "-help") || (args[i] == "-?") || (args[i] == "-h") )
                {
                    bHelp = true;
                }
            }

            if (!bDoBackup && !bDoRestore)
            {
                Console.WriteLine("ERROR: Neither Restore nor Backup command specified.");
                Console.WriteLine("");
                bHelp = true;
            }

            if(bDoBackup && bDoRestore)
            {
                Console.WriteLine("ERROR: Restore AND Backup command specified.  Pick one!");
                Console.WriteLine("");
                bHelp = true;
            }

            if (bHelp)
            {
                PrintHelp();
                return;
            }

            P4Shell.Initialize();

            if(port != "")
            {
                P4Shell.Port = port;
            }
            if(user != "")
            {
                P4Shell.User = user;
            }
            if(client != "")
            {
                P4Shell.Client = client;
            }

            if(bDoBackup)
            {
                BackupCommand(args);
            }
            else
            {
                RestoreCommand(args);
            }

            Console.WriteLine("Done.");
        }

        static void PrintHelp()
        {
            Console.WriteLine("Command Line Help");
            Console.WriteLine("-----------------");
            Console.WriteLine("");
            Console.WriteLine("P4Backup is a tool to take your currently open Perforce files and back");
            Console.WriteLine("them up to a network drive.  It can also restore backed up files to the");
            Console.WriteLine("same or another computer, while reopening the files for add/edit.");
            Console.WriteLine("The primary uses for this tool are to transfer work from one computer");
            Console.WriteLine("to another, keeping it Perforce kosher, or saving a work checkpoint");
            Console.WriteLine("without having to create a new personal P4 branch.  This tool handles");
            Console.WriteLine("deletes by storing them in __deletes.txt in the root backup folder.");
            Console.WriteLine("This means you can't have a file named __deletes.txt as part of a");
            Console.WriteLine("backup or restore operation, or it will ruin your day.  This tool");
            Console.WriteLine("will restore integrate/branch operations as simple edits.");
            Console.WriteLine("");
            Console.WriteLine("There are 2 commands for P4Backup:  -backup and -restore.  If neither is");
            Console.WriteLine("specified, it will default to running -backup.  Each has some options...");
            Console.WriteLine("");
            Console.WriteLine("-backup or -b:");
            Console.WriteLine("");
            Console.WriteLine("  By default, this will take all open files in the default repository, and");
            Console.WriteLine("  back them up to a dated folder under U:\\backup\\.  The backup uses the");
            Console.WriteLine("  full folder structure starting from the root of the drive backed up from.");
            Console.WriteLine("    * -backupdir or -bd <folder> : This lets you specify an alternative");
            Console.WriteLine("          backup directory, rather than U:\\Backup.  If you use this option,");
            Console.WriteLine("          a dated subdirectory will not be created unless you also use...");
            Console.WriteLine("    * -datedsubdir : When used in combination with -backupdir, this creates");
            Console.WriteLine("          a dated subdirectory under then directory specified.");
            Console.WriteLine("    * -changelist  or -cl <changelist number> : By default, -backup backs up");
            Console.WriteLine("          all open files on the default repository.  If you use this option,");
            Console.WriteLine("          it will only backup files in the specified changelist.");
            Console.WriteLine("");
            Console.WriteLine("-restore or -r:");
            Console.WriteLine("");
            Console.WriteLine("  This command takes a previously backed up folder and restores it to the");
            Console.WriteLine("  active P4 folder, while opening for edit/add any files restored.");
            Console.WriteLine("  If any of the files are already open for edit/add in your P4 directory,");
            Console.WriteLine("  errors will occur.");
            Console.WriteLine("    * -backupdir or -bd <folder> : Required.  This specifies the folder to");
            Console.WriteLine("           restore from.  Use the root of the previous backup including the");
            Console.WriteLine("           dated subdirectory if any.");
            Console.WriteLine("    * -targetdrive or -t <drive:> : Required.  This specifies the drive to");
            Console.WriteLine("           restore to in the form \"D:\".  You can specify a sub folder if you");
            Console.WriteLine("           want, but the backup takes the full directory structure from the");
            Console.WriteLine("           root of the drive, so if you want the restore to match the backup,");
            Console.WriteLine("           just pass the drive letter.");
            Console.WriteLine("");
            Console.WriteLine("Options for both:");
            Console.WriteLine("");
            Console.WriteLine("    * -port or -p <p4 port> : Lets you specify a non-default p4 port n the form");
            Console.WriteLine("           \"p4relic:1667\".");
            Console.WriteLine("    * -user or -u <p4 user> : Lets you specify a non-default p4 user.");
            Console.WriteLine("    * -client or -c <p4 client> : Lets you specify a non-default p4 clientspec.");
            Console.WriteLine("");
            Console.WriteLine("Environment variable:");
            Console.WriteLine("");
            Console.WriteLine("  Anything in an environment variable named P4BACKUPOPTS will be prepended to");
            Console.WriteLine("  any options passed via the command line.  If you specify an option in both");
            Console.WriteLine("  the environment variable and the command line, the command line takes");
            Console.WriteLine("  precedence.");
        }

        static void BackupCommand(string[] args)
        {
            Console.WriteLine("Performing backup...");

            int changelist = -1;
            string backupDir = "U:\\backup";
            bool bGotDirectory = false;
            bool bDatedSubdir = false;

            for (int i = 0; i < args.Length; ++i)
            {
                args[i] = args[i].ToLower();

                if (args[i] == "-datedsubdir")
                {
                    bDatedSubdir = true;
                }
                else if (args[i] == "-backupdir" || args[i] == "-bd")
                {
                    bGotDirectory = true;

                    if (args.Length > i + 1)
                    {
                        backupDir = args[i + 1];
                        // Trim trailing \s
                        backupDir = backupDir.TrimEnd(new char[] { '\\' });
                        ++i;
                    }
                }
                else if (args[i] == "-changelist" || args[i] == "-cl")
                {
                    if (args.Length > i + 1)
                    {
                        changelist = Convert.ToInt32(args[i + 1]);
                        ++i;
                    }
                }
            }

            // Append a dated subdir if requested
            if ((!bGotDirectory) || bDatedSubdir)
            {
                backupDir += "\\" + System.DateTime.Now.ToString("s").Replace(':', '_');
            }

            List<FileStats> openedFiles = P4Shell.GetOpenedFileStats(changelist);

            DoBackup(openedFiles, backupDir);
        }

        static void RestoreCommand(string[] args)
        {
            string backupDir = "";
            string targetDrive = "";

            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "-backupdir" || args[i] == "-bd")
                {
                    if (args.Length > i + 1)
                    {
                        backupDir = args[i + 1];
                        // Trim trailing \s
                        backupDir = backupDir.TrimEnd(new char[] { '\\' });
                        ++i;
                    }
                }
                else if (args[i] == "-targetdrive" || args[i] == "-t")
                {
                    if (args.Length > i + 1)
                    {
                        targetDrive = args[i + 1];
                        // Trim trailing \s
                        targetDrive = targetDrive.TrimEnd(new char[] { '\\' });
                        ++i;
                    }
                }
            }

            if(backupDir == "")
            {
                Console.WriteLine("ERROR: Missing -backupdir parameter.");
                return;
            }

            if(targetDrive == "")
            {
                Console.WriteLine("ERROR: Missing -targetdrive parameter.");
                return;
            }

            DoRestore(backupDir, targetDrive);
        }

        static void DoRestore(string backupDir, string targetDrive)
        {
            System.IO.DirectoryInfo dirInfo;

            List<System.IO.FileInfo> fileInfos = new List<System.IO.FileInfo>();

            string backupDirFull = System.IO.Path.GetFullPath(backupDir);

            // Recursively get a list of FileInfos for the backed up files
            try
            {
                dirInfo = new System.IO.DirectoryInfo(backupDirFull);
                if (!dirInfo.Exists)
                {
                    throw new System.IO.DirectoryNotFoundException();
                }

                GetFileSystemInfosRecurse(dirInfo.GetFileSystemInfos(), fileInfos);
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("ERROR: Failed to list files in backup directory.");
                return;
            }

            if(fileInfos.Count == 0)
            {
                Console.WriteLine("ERROR: No files in backup directory to restore.");
                return;
            }

            // Make the new changelist to hold the restored files in P4.
            int changeListID = -1;

            if (!P4Shell.CreateChangelist("P4Backup restored files", ref changeListID))
            {
                Console.WriteLine("ERROR: Failed to create new changelist.");
                Console.WriteLine(P4Shell.Error);
                return;
            }

            string filesForEdit = "";
            string filesForAdd = "";

            System.IO.FileInfo deletedFileInfo = null;

            // Find any existing files that need to be opened for edit, the rest are for add.
            foreach (System.IO.FileInfo fileInfo in fileInfos)
            {
                if(fileInfo.Name == k_DeletesFilename)
                {
                    deletedFileInfo = fileInfo;
                }
                else
                {
                    string localFilename = targetDrive + fileInfo.FullName.Substring(backupDirFull.Length);

                    if (System.IO.File.Exists(localFilename))
                    {
                        filesForEdit += localFilename + " ";
                    }
                    else
                    {
                        filesForAdd += localFilename + " ";
                    }
                }
            }

            // Remove __deletes.txt from the the list of fileinfos, since it's not a real p4 file to be restored
            if(deletedFileInfo != null)
            {
                fileInfos.Remove(deletedFileInfo);

                StringBuilder deletedFilenamesMerged = new StringBuilder();

                Console.WriteLine("Processing deletes from __deletes.txt...");
                try
                {
                    System.IO.StreamReader SR = deletedFileInfo.OpenText();

                    deletedFilenamesMerged.EnsureCapacity((int)(deletedFileInfo.Length * 1.2f));

                    string deletedFilename = SR.ReadLine();
                    while(deletedFilename != null)
                    {
                        deletedFilenamesMerged.Append(targetDrive);
                        deletedFilenamesMerged.Append('\\');
                        deletedFilenamesMerged.Append(deletedFilename);
                        deletedFilenamesMerged.Append(' ');
                        deletedFilename = SR.ReadLine();
                    }
                    SR.Close();
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("ERROR: Failed to open \"__deletes.txt\". Aborting.  Error: " + e.Message);
                    return;
                }

                if(!P4Shell.Delete(deletedFilenamesMerged.ToString(), changeListID))
                {
                    Console.WriteLine("ERROR: Failed to open files for delete. Aborting.");
                    Console.WriteLine(P4Shell.Output);
                    Console.WriteLine(P4Shell.Error);
                    return;
                }

                Console.WriteLine(P4Shell.Output);
            }

            // Open existing files on the hard drive for edit
            if ((filesForEdit != "") && !P4Shell.Edit(filesForEdit, changeListID))
            {
                Console.WriteLine("ERROR: Failed to open existing files for edit. Aborting.");
                Console.WriteLine(P4Shell.Output);
                Console.WriteLine(P4Shell.Error);
                return;
            }

            Console.WriteLine(P4Shell.Output);

            try
            {
                foreach (System.IO.FileInfo fileInfo in fileInfos)
                {
                    string localFilename = targetDrive + fileInfo.FullName.Substring(backupDirFull.Length);
                    string localDir = localFilename.Substring(0, localFilename.LastIndexOf('\\'));

                    System.IO.Directory.CreateDirectory(localDir);
                    System.IO.File.Copy(fileInfo.FullName, localFilename, true);
                }
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("ERROR: Exception during file copy. Aborting. Error: " + e.Message);
                return;
            }

            if((filesForAdd != "") && !P4Shell.Add(filesForAdd, changeListID))
            {
                Console.WriteLine("ERROR: After files copied locally, error adding some to perforce: \n" + P4Shell.Error);
                Console.WriteLine(P4Shell.Output);
                Console.WriteLine(P4Shell.Error);
                return;
            }

            Console.WriteLine(P4Shell.Output);

            Console.WriteLine("\nRestore successful.");
        }

        static void DoBackup(List<FileStats> files, string backupDir)
        {
            List<string> deletes = new List<string>();

            foreach(FileStats filestat in files)
            {
                string filename = filestat.ClientFile;

                string rawFile = filename;

                int driveIndex = filename.IndexOf(':');

                if(driveIndex != -1)
                {
                    rawFile = filename.Substring(driveIndex+1);
                    rawFile = rawFile.TrimStart(new char[]{'\\'});
                }

                if(filestat.OurAction == P4Shell.Action.Delete)
                {
                    deletes.Add(rawFile);
                }
                else
                {
                    string backupFile = backupDir + "\\" + rawFile;

                    Console.WriteLine("Backing up \"" + filename + "\" to \"" + backupFile + "\"...");

                    try
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(backupFile));
                        System.IO.File.Copy(filename, backupFile);
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine("    Copy failed.  Error: " + e.Message);
                    }
                }
            }

            if(deletes.Count > 0)
            {
                Console.WriteLine("Deletes found... writing to __deletes.txt in \"" + backupDir + "\"...");
                string deleteFilename = backupDir + "\\" + k_DeletesFilename;

                try
                {
                    System.IO.Directory.CreateDirectory(backupDir);
                    System.IO.StreamWriter SW = System.IO.File.CreateText(deleteFilename);

                    foreach (string deletedFile in deletes)
                    {
                        Console.WriteLine("Marking \"" + deletedFile + "\" as deleted...");
                        SW.WriteLine(deletedFile);
                    }
                    SW.Close();
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("    Failed to create __deletes.txt.  Error: " + e.Message);
                }
            }
        }

        static void GetFileSystemInfosRecurse(System.IO.FileSystemInfo[] dirAndFileInfos, List<System.IO.FileInfo> fileInfos)
        {
            // Iterate through each item.
            foreach (System.IO.FileSystemInfo i in dirAndFileInfos)
            {
                // Check to see if this is a DirectoryInfo object.
                if (i is System.IO.DirectoryInfo)
                {
                    // Cast the object to a DirectoryInfo object.
                    System.IO.DirectoryInfo dInfo = (System.IO.DirectoryInfo)i;

                    // Iterate through all sub-directories.
                    GetFileSystemInfosRecurse(dInfo.GetFileSystemInfos(), fileInfos);
                }
                // Check to see if this is a FileInfo object.
                else if (i is System.IO.FileInfo)
                {
                    fileInfos.Add((System.IO.FileInfo)i);
                }
            }
        }
    }
}
