#region << Revision History>>
/**
 * Author: Narayan
 *
 * Creation Date: 23rd May 2013
 *
 * Description: This application will monitor specfied location , once file is completely copied it will take back up and initiate SDM application.
 *
 *
 * Revision History:
 *-----------------------------------------------------------------------------------------------------
 * Version    	Date        	Developer         		Description
 *-----------------------------------------------------------------------------------------------------
 * 1.0          23/05/2013      Narayan                 Initial version.
 * 
 *-----------------------------------------------------------------------------------------------------
 */

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;
using Unisys.UDTS.Utility;

namespace FileWatcher
{
    class Program
    {
        /// <summary>
        /// Variable to hold  file landing  area path.
        /// </summary>
        static string FileLandArea = System.Configuration.ConfigurationManager.AppSettings["FileLandingArea"];
        /// <summary>
        /// Variable to hold save path of the files or back up path.
        /// </summary>
        static string Save = System.Configuration.ConfigurationManager.AppSettings["Save"];
        /// <summary>
        /// Variable to hold path of the LVMMYY or files processing path.
        /// </summary>
        static string LVMMYY = System.Configuration.ConfigurationManager.AppSettings["Processed"];

        /// <summary>
        /// Variable to hold SDM application path.
        /// </summary>
        static string SDMPath = System.Configuration.ConfigurationManager.AppSettings["SDMEXEPath"];

        /// <summary>
        /// Variable to hold to enable/disable log
        /// </summary>
        static string strEnbleLog = System.Configuration.ConfigurationManager.AppSettings["EnableLog"];

        /// <summary>
        /// Variable to hold connection string to log audit message.
        /// </summary>
        static string strConString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];

        /// <summary>
        /// Declaring the Variable for log
        /// </summary>
        static bool bEnableLog = false;


        /// <summary>
        /// Declaring the constant Variable for component name
        /// </summary>
        const string STRFILEWATCHER = "FILEWATCHER";

        static void Main(string[] args)
        {
            if (strEnbleLog.ToUpper().Trim() == "TRUE")
                bEnableLog = true;
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            if (Directory.Exists(FileLandArea))
            {
                watcher.Path = FileLandArea;
                // Watch all files.
                watcher.Filter = "*.*";
                // Add event handlers.
                watcher.Changed += new FileSystemEventHandler(OnCreated);
                // Begin watching.
                watcher.EnableRaisingEvents = true;
                // Wait for the user to quit the program.
                if (bEnableLog)
                    Utility.Audit(STRFILEWATCHER, "Filewatcher started and monitoring FLA", strConString);
                while (Console.ReadLine() != "exit") ;
            }
            else
            {
                if (bEnableLog)
                    Utility.Audit(STRFILEWATCHER, "Source directory " + FileLandArea + " does not exist.", strConString);
                Environment.Exit(0);
            }
        }

        // Define the event handlers. 
        /// <summary>
        /// Event handler is triggered when the file is copied to a file watcher directory.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            try
            {
                bool filestatus = FileIsDone(e.FullPath);
                if (filestatus)
                {
                    try
                    {
                        {
                            Copyfile(Save, e.Name, e.FullPath);
                            Copyfile(LVMMYY, e.Name, e.FullPath);
                            File.Delete(e.FullPath);
                            if (bEnableLog)
                                Utility.Audit(STRFILEWATCHER, "File " + e.Name + " deleted from the path " + e.FullPath.Replace(e.Name, ""), strConString);

                            //Call SDM application with recevied file name as parameter.
                            CallSDMApp(e.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        //log an exception.. if copy fails.
                        //Console.WriteLine(ex.ToString() + "  :" + DateTime.Now);
                        Utility.Exception_System("", "", STRFILEWATCHER, "OnCreated()", "FW", ex.Message, strConString);
                    }

                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine("failed for file" + e.Name);
                //Ignore the exception as file is used by another process or still copy is in progress.
            }

        }

        /// <summary>
        /// methode to call SDM application.
        /// </summary>
        /// <param name="filename"></param>
        private static void CallSDMApp(string filename)
        {
            Process objpro = new Process();
            // Geting the file name from  config file
            objpro.StartInfo.FileName = SDMPath;
            objpro.StartInfo.Arguments = filename; // Added space
            //Start the Process
            objpro.Start();
            objpro.Dispose();
            if (bEnableLog)
                Utility.Audit(STRFILEWATCHER, "Started SDM application from filewatcher for the filename: " + filename, strConString);
            //Console.WriteLine("Started SDM from filewatcher for the filename " + filename + "   :" + DateTime.Now);
        }

        /// <summary>
        /// Method to copy file from source to destination.
        /// </summary>
        /// <param name="BackupPath"></param>
        /// <param name="Name"></param>
        /// <param name="FullPath"></param>
        private static void Copyfile(string BackupPath, string Name, string FullPath)
        {
            if (Directory.Exists(BackupPath))
            {
                System.Threading.Thread.Sleep(50);
                File.Copy(FullPath, BackupPath + "\\" + Name, true);

                if (BackupPath.Contains("Save"))
                {
                    if (bEnableLog)
                        Utility.Audit(STRFILEWATCHER, "File " + Name + " copied to the backup folder " + BackupPath, strConString);
                }
                else
                {
                    if (bEnableLog)
                        Utility.Audit(STRFILEWATCHER, "File " + Name + " copied to the file processing folder " + BackupPath, strConString);
                }
            }
            else
            {
                if (bEnableLog)
                    Utility.Audit(STRFILEWATCHER, "Directory  " + BackupPath + " does not exist, check config file for Save/LVMMYY variables.", strConString);
            }

        }

        /// <summary>
        /// Method to open the file, if file is open means file is copied completely other wise copying is in progress.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FileIsDone(string path)
        {
            try
            {
                System.Threading.Thread.Sleep(500);
                using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {

                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }

            return true;
        }
    }
}
