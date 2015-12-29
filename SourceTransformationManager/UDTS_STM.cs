#region << Revision History>>
/**
 * Author: Narayan
 *
 * Creation Date: 4th September 2013
 *
 * Description: This application read the load queue table and invoke the respective packages.
 *
 *
 * Revision History:
 *-----------------------------------------------------------------------------------------------------
 * Version    	Date        	Developer         		Description
 *-----------------------------------------------------------------------------------------------------
 * 1.0.0.0     	07/12/2011 	    Narayan		            Initial Version
 * 1.0.0.1      05/06/2015      Amit                    PHOENIX Version
 * 
 *-----------------------------------------------------------------------------------------------------
 */

#endregion


#region << Namespaces >>
using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using System.IO;
using Unisys.UDTS.Utility;
using UNISYS.UDTS.SourceTransformationManager.LoadQueue;
using UNISYS;
using System.Management;


#endregion


namespace UNISYS.UDTS.SourceTransformationManager
{
    /// <summary>
    /// This code takes a source file name as an input parameter from conect direct and initiates the associated package.
    /// </summary>
    public class UDTH_SFM
    {

        #region<<Code to get configuration Valve >>
        /// <summary>
        ///  code to get connection String from config file
        /// </summary>
        static string StrConString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString(); 

        /// <summary>
        ///  code to get SSIS connection String from config file
        /// </summary>
        static string StrSSISConString = ConfigurationManager.ConnectionStrings["SSISConnectionString"].ToString();

        /// <summary>
        /// Code to get Package path  from config file.
        /// </summary>
        static string StrPkgPath = System.Configuration.ConfigurationManager.ConnectionStrings["PkgPath"].ConnectionString;

        /// <summary>
        /// Code to get dtexec path from config file.
        /// </summary>
        static string StrDtexecPath = System.Configuration.ConfigurationManager.ConnectionStrings["DtexecPath"].ConnectionString;

        /// <summary>
        /// Code to get SourceFilePath path from config file.
        /// </summary>
        static string SrcFilePath = System.Configuration.ConfigurationManager.ConnectionStrings["SourceFilePath"].ConnectionString;

        /// <summary>
        /// Code to get isScrambled status (true/false) from config file.
        /// </summary>
        static string strIsScrambled = System.Configuration.ConfigurationManager.ConnectionStrings["isScrambled"].ConnectionString;

     /// <summary>
        /// Code to get log enabler flag from config file.
        /// </summary>
        static string strEnableLog = System.Configuration.ConfigurationManager.ConnectionStrings["EnableLog"].ConnectionString;

        /// <summary>
        /// Code to get Enable SFM Background Run flag  from config file.
        /// </summary>
        static string strEnableSFMBackgroundRun = System.Configuration.ConfigurationManager.ConnectionStrings["EnableSFMBackgroundRun"].ConnectionString;

        /// <summary>
        /// Code to get Enable Use Shell Execute flag from config file.
        /// </summary>
        static string strEnableUseShellExecute = System.Configuration.ConfigurationManager.ConnectionStrings["EnableUseShellExecute"].ConnectionString;

        /// <summary>
        /// Code to get the flag to process header and footer
        /// </summary>
        static string strProcessHeaderFooter = System.Configuration.ConfigurationManager.ConnectionStrings["ProcessHeaderFooter"].ConnectionString;

        /// <summary>
        /// Code to get file name list which are not process through header and process
        /// </summary>
        static string strSourceFileIndicators = System.Configuration.ConfigurationManager.ConnectionStrings["SourceFileIndicators"].ConnectionString;

        /// <summary>
        /// Code to get X02 file path
        /// </summary>
        static string strX02FilePath = System.Configuration.ConfigurationManager.ConnectionStrings["X02FilePath"].ConnectionString;

        /// <summary>
        /// Code to get Exception file path
        /// </summary>
        static string strExceptionFilePath = System.Configuration.ConfigurationManager.ConnectionStrings["ExceptionFilePath"].ConnectionString;  
        

        /// <summary>
        /// code to get SASWorking file path
        /// </summary>
        static string strSASWorkingFilePath = "D:\\Ouptut";//System.Configuration.ConfigurationManager.ConnectionStrings["SASWorkingFilePath"].ConnectionString;

        /// <summary>
        /// code to get X02CountVerifyFilePath file path
        /// </summary>
        static string strX02CountVerifyFilePath = System.Configuration.ConfigurationManager.ConnectionStrings["X02CountVerifyFilePath"].ConnectionString;

        /// <summary>
        /// code to get stop file path
        /// </summary>
        static string strStopFilePath = System.Configuration.ConfigurationManager.ConnectionStrings["StopFilePath"].ConnectionString;

        /// <summary>
        /// code to get Cascade ALS file path
        /// </summary>
       static string strALSFilePath = "D:\\Ouptut"; //System.Configuration.ConfigurationManager.ConnectionStrings["CascadeALSFilePath"].ConnectionString;

        /// <summary>
        /// code to get Cascade Mort file path
        /// </summary>
        static string strMortFilePath = "D:\\Ouptut"; //System.Configuration.ConfigurationManager.ConnectionStrings["CascadeMortFilePath"].ConnectionString;

        /// <summary>
        /// code to get Cascade Exe file path
        /// </summary>
        static string strExpFilePath = "D:\\Ouptut"; //System.Configuration.ConfigurationManager.ConnectionStrings["CascadeExeFilePath"].ConnectionString;

        /// <summary>
        /// code to get Cascade AKA file path
        /// </summary>
        static string strAKAFilePath =  "D:\\Ouptut"; //System.Configuration.ConfigurationManager.ConnectionStrings["CascadeAKAFilePath"].ConnectionString;

        /// <summary>
        /// code to get Cascade Mort Chunk level  file path
        /// </summary>
        static string strMortChunkFilePath =  "D:\\Ouptut"; //System.Configuration.ConfigurationManager.ConnectionStrings["CascadeMortChunkFilePath"].ConnectionString;

        /// <summary>
        /// code to get Error package name
        /// </summary>
        static string strErrorPackageName = System.Configuration.ConfigurationManager.ConnectionStrings["X02ErrorPackageName"].ConnectionString;

        /// <summary>
        /// code to get Exception package name
        /// </summary>
        static string strExceptionPackageName = System.Configuration.ConfigurationManager.ConnectionStrings["ExceptionPackageName"].ConnectionString;

        /// <summary>
        /// code to get Reconciliation package name
        /// </summary>
        static string strReconciliationPackageName = "PKG_UFSS"; //System.Configuration.ConfigurationManager.ConnectionStrings["ReconciliationPackageName"].ConnectionString;

        /// <summary>
        /// code to get X02 count verify package name
        /// </summary>
        static string strX02countverifyPackageName = System.Configuration.ConfigurationManager.ConnectionStrings["X02countverifyPackageName"].ConnectionString;

        /// <summary>
        /// code to get CascMortChunkPackageName package name
        /// </summary>
        static string strCascMortChunkPackageName = "PKG_UFSS";// System.Configuration.ConfigurationManager.ConnectionStrings["CascMortChunkPackageName"].ConnectionString;

        /// <summary>
        /// Code to get Interval 
        /// </summary>
        static Int32 nInterval = Int32.Parse(System.Configuration.ConfigurationManager.ConnectionStrings["Interval"].ConnectionString);

        /// <summary>
        /// Code to get Check sum exe path from config file.
        /// </summary>
        static string StrCheckSumPath = System.Configuration.ConfigurationManager.ConnectionStrings["CheckSumPath"].ConnectionString;

        /// <summary>
        /// Code to get check sum  enabler flag from config file.
        /// </summary>
        static string strCheckSum = System.Configuration.ConfigurationManager.ConnectionStrings["CheckSum"].ConnectionString;

        /// <summary>
        /// code to get Trinity_Transform & Load report  file path.
        /// </summary>
        static string strRptFilePath = "D:\\Output"; //System.Configuration.ConfigurationManager.ConnectionStrings["TrinityLoadRptFilePath"].ConnectionString;


        /// <summary>
        /// Code to get X03 file path
        /// </summary>
        static string strX03FilePath = System.Configuration.ConfigurationManager.ConnectionStrings["X03FilePath"].ConnectionString;
        /// <summary>
        /// Code to get Release name from config file
        /// </summary>
        static string strReleaseName = System.Configuration.ConfigurationManager.ConnectionStrings["ReleaseName"].ConnectionString;

        /// <summary>
        /// Code to get X02 Transform split count from config file
        /// </summary>
        static string strX02TransformOutCount = System.Configuration.ConfigurationManager.ConnectionStrings["X02TransformOutCount"].ConnectionString;

        /// <summary>
        /// Code to get X03 Transform split count from config file
        /// </summary>
        static string strX03TransformOutCount = System.Configuration.ConfigurationManager.ConnectionStrings["X03TransformOutCount"].ConnectionString;

        /// <summary>
        /// Code to get Release name from config file
        /// </summary>
        static string strRawfilePath = System.Configuration.ConfigurationManager.ConnectionStrings["RawfilePath"].ConnectionString;

        /// <summary>
        /// Code to get X02Status from config file
        /// </summary>
        static string strX02StatusAccountType = System.Configuration.ConfigurationManager.ConnectionStrings["X02Status"].ConnectionString;

        /// <summary>
        /// Code to get X03Status from config file
        /// </summary>
        static string strX03StatusAccountType = System.Configuration.ConfigurationManager.ConnectionStrings["X03Status"].ConnectionString;

        /// <summary>
        /// Code to get X02ClosureDate from config file
        /// </summary>
        static string strX02ClosureDate = System.Configuration.ConfigurationManager.ConnectionStrings["X02ClosureDate"].ConnectionString;

        /// <summary>
        /// Code to get X03ClosureDate from config file
        /// </summary>
        static string strX03ClosureDate = System.Configuration.ConfigurationManager.ConnectionStrings["X03ClosureDate"].ConnectionString;

        /// <summary>
        /// Code to get PackageOutputPathFolder from config file
        /// </summary>
        static string strPackageOutputPathFolder = System.Configuration.ConfigurationManager.ConnectionStrings["PackageOutputPathFolder"].ConnectionString;

        /// <summary>
        /// Code to get EnablePackageOutput from config file
        /// </summary>
        static string strEnablePackageOutput = System.Configuration.ConfigurationManager.ConnectionStrings["EnablePackageOutput"].ConnectionString;

        /// <summary>
        /// Code to get RunCSVPreprocessor from config file
        /// </summary>
        static string IsPreprocessed = System.Configuration.ConfigurationManager.ConnectionStrings["IsPreprocessed"].ConnectionString;

        #endregion

        #region <<Variable and Constants used in STM Application>>
        /// <summary>
        /// Constant to store the application name.
        /// </summary>
        const string STRSFMLOGCOMP = "STM application";
        /// <summary>
        /// Constant to store the log for SFM start.
        /// </summary>
        const string STRSFMSTARTLOG = "STM applications started";
        /// <summary>
        /// Constant to store the log for SFM completion.
        /// </summary>
        const string STRSFMCOMPLOG = "STM application completed";

        //Variable to get number of packages in progress
        static int nInprogressPackage = 0;

        // variable to hold the log enabler
        static bool bEnableLog = true;

        // variable to hold the Load queue status update
        static bool bEnableLQStatus = false;

        // variable to hold the MultiNode status
        static bool bIsMultiNode = true;

        // variable to hold the PrimaryNode status
        static bool bIsPrimaryNode = true;

        // variable to hold the Load queue status update
        static string strEnableLQStatus = string.Empty;

        // Variable to hold the name of the source file
        static string strSourceFileName = string.Empty;

        // Variable to hold the name of the package
        static string strPackageName = string.Empty;

        //Variable to hold the chunk number.
        static string strChunkNumber = string.Empty;

        //Variable to hold the tranch number.
        static string strTrancheNumber = string.Empty;

        //Variable to hold the table name.
        static string strTableName = string.Empty;

        //Variable to hold the priority.
        static string strPriority = string.Empty;

        //Variable to hold the status.
        static string strStatus = string.Empty;

        //Variable to hold the type.
        static string strType = string.Empty;

        //Variable to hold the type.
        static string strSourceTableName = string.Empty;

        static string strAckFile = string.Empty;

        static string batfile = string.Empty;

       // static string strFileType = string.Empty;

        //Declaration of Timers 
        private static System.Timers.Timer timerSTM = new System.Timers.Timer();

        // variable to hold the package output enabler
        static bool bEnablePackageOutput = true;

        static FileInfo F;

        #endregion

        static void Main(string[] args)
        {
            
            //Initailzation of Event_Handler for the timer control, this Handler is triggered once the timer control is elapsed with the Preset interval.
            timerSTM.Elapsed += new System.Timers.ElapsedEventHandler(OnElapsedTime);
            timerSTM.Interval = nInterval * 1000;
            // Call the function time directly to start process and later it  will fire using the elapse functin event
            //Method to check weather stop file arives, if not get count of inprogress packages and max no of packages to run from config if  in progress packages is
            //less , than get the data from loadqueue based on status and priority  and execute the package
            GetInprogressLoadQueueMain();
            timerSTM.Enabled = true;
            timerSTM.Start();
            Console.ReadLine();

        }


        #region <<Method : OnElapsedTime >>
        /// <summary>
        /// Event Handler for timer control, this event is triggered once the timer is elapsed with its preset interval. Also the time can be enabled and disable using this function.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            timerSTM.Enabled = false;
            // Call the function time directly to start process and later it  will fire using the elapse function event
            GetInprogressLoadQueueMain();
            timerSTM.Enabled = true;
        }
        #endregion

        #region<< Code to get the package which are ready to execute from loadqueue table>>
        /// <summary>
        /// Code to get the package which are ready to execute from loadqueue table
        /// </summary>
        public static void GetInprogressLoadQueueMain()
        {
            strEnableLQStatus = System.Configuration.ConfigurationManager.ConnectionStrings["EnableLQStatus"].ConnectionString;
            Int32 nMaxNoOfPackageToLoad = Int32.Parse(System.Configuration.ConfigurationManager.ConnectionStrings["MaxNoOfPackageToLoad"].ConnectionString);
            string strChkPhyMemUtilization = System.Configuration.ConfigurationManager.ConnectionStrings["CheckPhysicalMemoryUtilization"].ConnectionString;
            Int64 strMaxPhyMemUtilization = Int64.Parse(System.Configuration.ConfigurationManager.ConnectionStrings["MaxPerPhysicalMemoryUtilization"].ConnectionString);
            Int32 nPkgCountToInvoke = 0;
            int countCascade = 0;
            Int64 iPhyMemory = 0;
            if (strEnableLog.Trim().ToLower() == "false")
                bEnableLog = false;
            IList<UDTS_STM_LoadQueue> obj_LoadQueue = new List<UDTS_STM_LoadQueue>(); 
            try
            {
                if (File.Exists(strStopFilePath))
                {
                    if (bEnableLog)
                        Utility.Audit(STRSFMLOGCOMP, "Recevied stop file '" + strStopFilePath + "' and stoped running STM application as requested.", StrConString);

                    //Exiting the application
                    Environment.Exit(0);
                }

                // Variable to get number of packages in Progess from NoOfPackageInProgress Method
                nInprogressPackage = NoOfPackageInProgress();

                if (bEnableLog)
                    Utility.Audit(STRSFMCOMPLOG, "Number of inprogress packages: " + nInprogressPackage + " MaxNoOfpackagesToload " + nMaxNoOfPackageToLoad, StrConString);

                // Check if in progress packages  is less than 
                if (nInprogressPackage < nMaxNoOfPackageToLoad)
                {
                    // calling method to get all the target type and package name  which ready to run
                    obj_LoadQueue = GetDataFromLoadQueue();
                    if (bEnableLog)
                        Utility.Audit(STRSFMCOMPLOG, "Number of inprogress packages: " + nInprogressPackage + " MaxNoOfpackagesToload " + nMaxNoOfPackageToLoad + " No. of Package in LoadQueue:" + obj_LoadQueue.Count, StrConString);
                    //Checking for any package is ready to execute
                    if (obj_LoadQueue.Count > 0)
                    {
                        nPkgCountToInvoke = nMaxNoOfPackageToLoad - nInprogressPackage;
                        if (nPkgCountToInvoke > obj_LoadQueue.Count)
                        {
                            nPkgCountToInvoke = obj_LoadQueue.Count; // get the final no fo package to invoke based on current in progress, max. no. of pkg to load and package in loadqueue
                        }
                        if (bEnableLog)
                            Utility.Audit(STRSFMCOMPLOG, "Number of in progress packages:" + nInprogressPackage + " Max. no. of packages to load:" + nMaxNoOfPackageToLoad + " No. of Package in LoadQueue:" + obj_LoadQueue.Count + " No. of Package to Load:" + nPkgCountToInvoke, StrConString);
                        for (int queueCount = 0; queueCount < nPkgCountToInvoke; queueCount++)
                        {
                            //getting data from load queue table  for each  iteration
                            strPackageName = obj_LoadQueue[queueCount].packageName.ToString() + ".dtsx";
                            strChunkNumber = obj_LoadQueue[queueCount].chunkNumber.ToString();
                            strTrancheNumber = obj_LoadQueue[queueCount].tranchNumber.ToString();
                            strType = obj_LoadQueue[queueCount].Type.ToString();
                            strStatus = obj_LoadQueue[queueCount].Status.ToString();
                            strSourceFileName = obj_LoadQueue[queueCount].SourceFileName.ToString();
                            strSourceTableName = obj_LoadQueue[queueCount].SourceTableName.ToString();
                           // strFileType = obj_LoadQueue[queueCount].filetype.ToString(); // Added for TRG for X03/X02
                            //To check wheather package name  exist in package path
  
                            if (File.Exists(StrPkgPath.TrimEnd('\\') + "\\" + strPackageName))
                            {
                                if (strChkPhyMemUtilization.Trim().ToLower() == "true")// && iPhyMemory < strMaxPhyMemUtilization)
                                {
                                    iPhyMemory = GetPhysicalMemoryUsageInPer();
                                    //http://stackoverflow.com/questions/3360555/how-to-pass-parameters-to-threadstart-method-in-thread
                                    //http://stackoverflow.com/questions/9418554/starting-a-new-thread-in-a-foreach-loop
                                    if (iPhyMemory < strMaxPhyMemUtilization)
                                    {
                                        if (bEnablePackageOutput)
                                         {
                                             string tmpstrPackageName, tmpstrTrancheNumber, tmpstrChunkNumber, tmpstrType, tmpstrSourceFileName, tmpstrSourceTableName;
                                             tmpstrPackageName = strPackageName; tmpstrTrancheNumber = strTrancheNumber; tmpstrChunkNumber = strChunkNumber; tmpstrType = strType;
                                             tmpstrSourceFileName = strSourceFileName; tmpstrSourceTableName = strSourceTableName;

                                             Thread thread = new Thread(() => ExceutePackage(tmpstrPackageName, tmpstrTrancheNumber, tmpstrChunkNumber, tmpstrType, tmpstrSourceFileName, tmpstrSourceTableName));
                                             thread.Start(); 
                                         }

                                         else
                                         {
                                             //calling method to execute the package
                                             ExceutePackage(strPackageName, strTrancheNumber, strChunkNumber, strType, strSourceFileName, strSourceTableName);
                                         }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    if (bEnablePackageOutput)
                                    {
                                        string tmpstrPackageName, tmpstrTrancheNumber, tmpstrChunkNumber, tmpstrType, tmpstrSourceFileName, tmpstrSourceTableName;
                                        tmpstrPackageName = strPackageName; tmpstrTrancheNumber = strTrancheNumber; tmpstrChunkNumber = strChunkNumber; tmpstrType = strType;
                                        tmpstrSourceFileName = strSourceFileName; tmpstrSourceTableName = strSourceTableName;
                                        
                                        //ExceutePackage(tmpstrPackageName, tmpstrTrancheNumber, tmpstrChunkNumber, tmpstrType, tmpstrSourceFileName, tmpstrSourceTableName);
                                        Thread thread = new Thread(() => ExceutePackage(tmpstrPackageName, tmpstrTrancheNumber, tmpstrChunkNumber, tmpstrType, tmpstrSourceFileName, tmpstrSourceTableName));
                                        thread.Start();
                                    }
                                    else
                                    {
                                        //calling method to execute the package
                                        ExceutePackage(strPackageName, strTrancheNumber, strChunkNumber, strType, strSourceFileName, strSourceTableName);
                                    }
                                }
                            }
                            else
                            {
                                //Loging package is missing to log table
                                if (bEnableLog)
                                    Utility.Audit(STRSFMCOMPLOG, "Package " + StrPkgPath.TrimEnd('\\') + "\\" + strPackageName + " does not exists in the package path ", StrConString);
                            }
                        }//end of for loop
                    }

                }

                // code here  to check wheather all the Casecade  package are completed  at tranch level and close STM Application 
                countCascade = CheckCascadePackage(); // Ckeck on this in integration testing tranch level
                if (countCascade == 0)
                {
                    if (bEnableLog)
                        Utility.Audit(STRSFMCOMPLOG, "STM application is closed as all tranche level cascade packages are started.", StrConString);
                    // Exiting the application
                    Environment.Exit(1);

                }


            }
            catch (Exception ex)
            {
                // Logging exception
                Utility.Exception_System(strTrancheNumber, strChunkNumber, STRSFMLOGCOMP, "NoOfPackageInProgress()", "STM", ex.Message, StrConString);
                // Exiting the application
                Environment.Exit(1);
            }


        }
        #endregion

        #region <<Code to check the Physical memory Utilization>>

        /// <summary>
        /// Code to get physical memory in usage percentage
        /// </summary>
        /// <returns></returns>
        static Int64 GetPhysicalMemoryUsageInPer()
        {
            Int64 perUsage = 0;
            try
            {
                PerformanceCounter pc = new PerformanceCounter("Memory", "Available Bytes");
                Int64 freeMemory = Convert.ToInt64(pc.NextValue());
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                Int64 totalMemory = 0;
                foreach (ManagementObject item in moc)
                {
                    totalMemory = Convert.ToInt64(item.Properties["TotalPhysicalMemory"].Value);// / 1073741824, 2)) + " GB";                
                }
                perUsage = (Int64)((totalMemory - freeMemory) * 100 / totalMemory);
            }
            catch (Exception Ex)
            {
                perUsage = 0;
                Utility.Exception_System(strTrancheNumber, strChunkNumber, STRSFMLOGCOMP, "GetPhysicalMemoryUsageInPer()", "STM", Ex.Message, StrConString);
                // Exiting the application
                Environment.Exit(2);
            }
            return perUsage;
        }
        #endregion

        #region<<check all the cascade package are loaded into load queue table>>
        /// <summary>
        /// Code to check all the cascade package are loaded into load queue table
        /// </summary>
        /// <returns></returns>
        private static int CheckCascadePackage()
        {
            int cascadeCount = 0;
            // create an instance of sql connection
            SqlConnection objCon = new SqlConnection(StrConString);

            // Pass the Stored Procedure name
            SqlCommand objCmd = new SqlCommand("PR_UDMH_STM_CHK_Cascade_Pkg_CMP", objCon);

            // selecting the command Type as SSP
            objCmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (objCon.State == ConnectionState.Closed)
                    objCon.Open();
                cascadeCount = (int)objCmd.ExecuteScalar();
            }

            catch (Exception ex)
            {
                // Logging exception
                Utility.Exception_System(strTrancheNumber, strChunkNumber, STRSFMLOGCOMP, "CheckCascadePackage()", "STM", ex.Message, StrConString);
                // Exiting the application
                Environment.Exit(2);
            }
            finally
            {
                //Closing connection
                if (objCon.State == ConnectionState.Open)
                    objCon.Close();
                //Disposing Connection Object
                objCon.Dispose();
            }

            return cascadeCount;


        }

        #endregion

        #region<<Check file is locked by another process>>
        /// <summary>
        /// Check file is locked by another process
        /// </summary>
        /// <returns></returns>
        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        #endregion

        #region<< Code to execute  packages >>
        /// <summary>
        /// Code to execute  packages 
        /// </summary>
        /// <param name="strPackageName"></param>
        /// <param name="strTranchNumber"></param>
        /// <param name="strChunkNumber"></param>
        /// <param name="strType"></param>
        /// <param name="strSourceFileName"></param>
        /// <param name="strSourceTableName"></param>
        private static void ExceutePackage(string strPackageName, string strTrancheNum, string strChunkNumber, string strType, string strSourceFileName, string strSourceTableName)
        {
            string strpac = strPackageName;

            if (strChunkNumber.Trim() == "") //Default chunk number in case of blank
            {
                strChunkNumber = "01";
            }
            if (strTrancheNum.Trim() == "") //Default tranche number in case of blank
            {
                strTrancheNum = "01";
            }
            // Variable to hold recordcount
            int RecCount = 0;
            //variable to hold header code
            int errHeader = 0;
            // variable to hold trailer code
            int errFooter = 0;
            //variable to hold header information
            string strHeader = string.Empty;
            // variable to hold trailer information
            string strFooter = string.Empty;

            if (strEnableLog.Trim().ToLower() == "false") 
                bEnableLog = false;
            // variable to hold the background run flag
            bool bEnableSFMBackgroundRun = true;
            if (strEnableSFMBackgroundRun.Trim().ToLower() == "false")
                bEnableSFMBackgroundRun = false;
            // variable to hold to use shell execute flag
            bool bEnableUseShellExecute = false;
            if (strEnableUseShellExecute.Trim().ToLower() == "true")
                bEnableUseShellExecute = true;
            bEnablePackageOutput = false;
            if (strEnablePackageOutput.Trim().ToLower() == "true")
                bEnablePackageOutput = true;
            // Create an instance of Process
            Process proStartPkg = new Process();

            // path to Dtsexe.exe utility
            proStartPkg.StartInfo.FileName = StrDtexecPath;
            // Add the connection string parameter to use by the package to load the source data
            //passing parameters
            //Start: Paramenter to run the SFM in background                    
            if (bEnableSFMBackgroundRun)
            {
                proStartPkg.StartInfo.UseShellExecute = bEnableUseShellExecute;//false;
                proStartPkg.StartInfo.CreateNoWindow = true;
                proStartPkg.StartInfo.RedirectStandardInput = true;
                proStartPkg.StartInfo.RedirectStandardOutput = true;
                proStartPkg.StartInfo.RedirectStandardError = true;
                proStartPkg.StartInfo.ErrorDialog = false;
                proStartPkg.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            try
            {

                proStartPkg.StartInfo.Arguments = " /File \"" + StrPkgPath + strPackageName + "\""
                        + " /SET \"\\Package.Variables[connTransformation].Value\";" + "\"\\\"" + StrSSISConString.Trim() + "\\\"\"";
                // + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber;

                if (strType.ToUpper() == "SOURCE")
                {
                    if (strPackageName.Contains(strErrorPackageName))
                    {
                        proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[srcFilePath].Value\";" + "\"" + SrcFilePath + "\\\"";
                    }
                    else
                    {
                        
                        //AKS>> Getting file name without file extension from loadqueue table
                         string strSourceFileNameWithoutExtension = Path.GetFileNameWithoutExtension(strSourceFileName);
                         string strRawFile = string.Empty;
                         string RawFileNameWithoutExtension = string.Empty;
                         string status = string.Empty;
                        //Reading file name from raw file path
                         string[] files = Directory.GetFiles(strRawfilePath);

                         foreach (string Filename in files)
                         {
                             //Splitting the string based on dot(.)
                             string[] splitted = Filename.Split('.');

                             RawFileNameWithoutExtension = Path.GetFileName(splitted[0]);

                             //Compairing the filename from loadqueue table and raw file path 
                             if (strSourceFileNameWithoutExtension == RawFileNameWithoutExtension)
                             {
                                 strRawFile = Path.GetFileName(Filename); //Storing filename with extension to pass as command line argument in csvPreProcessor method.
                                 status = "true";
                             }
                          
                         }

                         if (status == "true")
                            {
                                if (IsPreprocessed == "false") //Checking the flag to run CSV_Preprocessor if file is not processed then processing the raw files
                                {
                                    //method to process the raw files  
                                    csvPreprocessor(strRawfilePath + strRawFile, SrcFilePath + strSourceFileName);
                                }

                                // variable to hold header & footer process flag
                                bool bProcessHeaderFooter = false;
                                if (strProcessHeaderFooter.Trim().ToLower() == "true")
                                    bProcessHeaderFooter = true;

                                // Variable to store to process header and footer for unchunk files
                                bool bIsSegProcessHF = false;

                                if (File.Exists(SrcFilePath + strSourceFileName))
                                {
                                    FileInfo fInfo = new FileInfo(SrcFilePath + strSourceFileName);
                                    if (fInfo.Length == 0)
                                    {
                                        // Invoke Audti method to log information
                                        if (bEnableLog)
                                            Utility.Audit(STRSFMLOGCOMP, "Started & ended" + " with error: Source file " + SrcFilePath + strSourceFileName + " does not have data (0 bytes file).", StrConString);
                                        //code to generate Acknowledge file
                                        GenerateAcknowledgeMentFile(strSourceFileName, "ERROR-EMPTFILE", strChunkNumber);
                                        Utility.Audit(STRSFMLOGCOMP, "Source input file '" + SrcFilePath + strSourceFileName + "' is empty." + strSourceFileName, StrConString);

                                        //Exiting the application
                                        Environment.Exit(2);
                                    }

                                    else
                                    {
                                        // For Footer count in case where header and footer not availble in Sourcefiles.
                                        RecCount = -1;

                                    }
                                   // Gireesh> added below code to runn ACCPED package
                                    if (strPackageName.Contains("ACCPED"))
                                    {
                                        proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[srcTable].Value\";" + "\"" + strSourceTableName + "\"" + " /SET \"\\Package.Variables[srcFilePath].Value\";" + "\"\\\"" + SrcFilePath.Trim()  + "\\\"\"";
                                    }
                                    else
                                        //once package are updated pass rec count  as a parameter
                                        //AKS> Commented this for oled sorce connection 11_0ct_2015
                                        //proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[srcFileName].Value\";" + "\"" + strSourceFileName + "\"" + " /SET \"\\Package.Variables[srcTable].Value\";" + "\"" + strSourceTableName + "\"" + " /SET \"\\Package.Variables[srcFilePath].Value\";" + "\"" + SrcFilePath.TrimEnd('\\') + "\"";
                                        //AKS> Using below argument for flat file source connection 11_0ct_2015
                                        proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[srcTable].Value\";" + "\"" + strSourceTableName + "\"" + " /SET \"\\Package.Variables[srcFilePath].Value\";" + "\"\\\"" + SrcFilePath.Trim() + strSourceFileName.Trim() + "\\\"\"";
                                }
                                else
                                {
                                    if (bEnableLog)
                                    {
                                        //incase of file does not exist skipping the package exceution
                                        errHeader = 99;
                                        Utility.Audit(STRSFMLOGCOMP, "Input source file does not exist in specified path " + SrcFilePath + strSourceFileName, StrConString);
                                    }
                                }
                            }

                            else
                            {
                                if (bEnableLog)
                                {
                                    //incase of file does not exist skipping the package exceution
                                    errHeader = 99;
                                    Utility.Audit(STRSFMLOGCOMP, "Input source file does not exist in specified path " + strRawfilePath + strSourceFileName, StrConString);
                                }

                            }
                       // }
                    }

                }

                else if (strType.ToUpper() == "GENERATE")
                {
                    if (strSourceTableName.ToUpper().Contains("X02"))
                        proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[checkSum].Value\";" + strCheckSum + " /SET \"\\Package.Variables[checksumPath].Value\";" + "\"" + StrCheckSumPath + "\\\"" + " /SET \"\\Package.Variables[X02FilePath].Value\";" + "\"" + strX02FilePath + "\\\"" + " /SET \"\\Package.Variables[CON_TRANS_OUT_CNT].Value\";" + Convert.ToInt32(strX02TransformOutCount) + " /SET \"\\Package.Variables[X02_Status].Value\";" + "\"" + strX02StatusAccountType + "\"" + " /SET \"\\Package.Variables[X02_ClosureDate].Value\";" + "\"" + strX02ClosureDate + "\"";
                       
                    //proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[checkSum].Value\";" + strCheckSum + " /SET \"\\Package.Variables[checksumPath].Value\";" + "\"" + StrCheckSumPath + "\"" + " /SET \"\\Package.Variables[X02FilePath].Value\";" + "\"" + strX02FilePath + "\"";


                    else if (strSourceTableName.ToUpper().Contains("X03"))
                        proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[checkSum].Value\";" + strCheckSum + " /SET \"\\Package.Variables[checksumPath].Value\";" + "\"" + StrCheckSumPath + "\\\"" + " /SET \"\\Package.Variables[X03FilePath].Value\";" + "\"" + strX03FilePath + "\\\"" + " /SET \"\\Package.Variables[CON_TRANS_OUT_CNT].Value\";" + Convert.ToInt32(strX03TransformOutCount) + " /SET \"\\Package.Variables[X03_Status].Value\";" + "\"" + strX03StatusAccountType + "\"" + " /SET \"\\Package.Variables[X03_ClosureDate].Value\";" + "\"" + strX03ClosureDate + "\"";
                      //  proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[checkSum].Value\";" + strCheckSum + " /SET \"\\Package.Variables[checksumPath].Value\";" + "\"" + StrCheckSumPath + "\"" + " /SET \"\\Package.Variables[X02FilePath].Value\";" + "\"" + strX03FilePath + "\"";
                }
                //else if (strType.ToUpper() == "RECONCILIATION")
                //{
                //    //Code to check Exception package and assign exception  path
                //    if (strPackageName.Replace(".dtsx", "").Trim() == strExceptionPackageName.Trim())
                //    {
                //        proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[tgtFilePath].Value\";" + "\"" + strExceptionFilePath + "\"";
                //    }
                   
                //    else if (strPackageName.Replace(".dtsx", "").Trim() == strX02countverifyPackageName.Trim())
                //    {
                //        proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[tgtFilePath].Value\";" + "\"" + strX02CountVerifyFilePath + "\"";
                //    }
                //    else
                //        proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[checkSum].Value\";" + strCheckSum + " /SET \"\\Package.Variables[checksumPath].Value\";" + "\"" + StrCheckSumPath + "\"" + " /SET \"\\Package.Variables[tgtFilePath].Value\";" + "\"" + strSASWorkingFilePath + "\"";


                //}
                else if (strType.ToUpper() == "CONTROL")
                {
                    proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[srcFileName].Value\";" + "\"\\\"" + SrcFilePath + strSourceFileName + "\\\"\"" + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber;

                }

                else if (strType.ToUpper() == "CASCTRANSMORT") //EARLIER CASCADE
                {
                    if (strPackageName.Replace(".dtsx", "") == strCascMortChunkPackageName)
                    {
                        proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[tgtFilePath].Value\";" + "\"" + strMortChunkFilePath + "\"";
                    }
                    else
                    {
                        proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber;

                    }
                }

                else if (strType.ToUpper().Contains("CASCADE"))
                {
                    bIsMultiNode = true;
                    bIsPrimaryNode = true;
                    string strMultinode = System.Configuration.ConfigurationManager.ConnectionStrings["MultiNodeProcess"].ConnectionString;
                    if (strMultinode.Trim().ToLower() == "false")
                        bIsMultiNode = false;

                    int iLinkServerCnt = 0;
                    Boolean bCascadeTranche = true;
                    string strDatabaseNode2 = string.Empty;
                    string strDatabaseNode3 = string.Empty;
                    string strServernameNode2 = string.Empty;
                    string strServernameNode3 = string.Empty;
                    string strPrimaryNode = string.Empty;

                    strDatabaseNode2 = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseNameNode2"].ConnectionString;
                    strDatabaseNode3 = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseNameNode3"].ConnectionString;
                    strServernameNode2 = System.Configuration.ConfigurationManager.ConnectionStrings["ServerNameNode2"].ConnectionString;
                    strServernameNode3 = System.Configuration.ConfigurationManager.ConnectionStrings["ServerNameNode3"].ConnectionString;
                    strPrimaryNode = System.Configuration.ConfigurationManager.ConnectionStrings["PrimaryNode"].ConnectionString;

                    if (bIsMultiNode)
                    {
                        if (strPrimaryNode.Trim().ToLower() == "false")
                            bIsPrimaryNode = false;
                        if (bIsPrimaryNode)
                            iLinkServerCnt = CheckLinkServerConnection(strServernameNode2, strServernameNode3);
                        if ((iLinkServerCnt != 2) && (bIsPrimaryNode))
                        {
                            bCascadeTranche = false;
                        }
                        //TFS BUG id 12273
                        if (strServernameNode2.Trim() == strServernameNode3.Trim() && iLinkServerCnt ==1) 
                        {
                            bCascadeTranche = true;
                        }

                    }
                    

                    if (bCascadeTranche)
                    {
                        if (strType.ToUpper() == "CASCADEALS")
                        {
                            proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + "00" + " /SET \"\\Package.Variables[trancheNumber].Value\";" + strTrancheNum +
                                                                                                " /SET \"\\Package.Variables[databaseNode2].Value\";" + "\"" + strDatabaseNode2 + "\"" + " /SET \"\\Package.Variables[databaseNode3].Value\";" + "\"" + strDatabaseNode3 + "\"" +
                                                                                                " /SET \"\\Package.Variables[IsMultiNode].Value\";" + bIsMultiNode + " /SET \"\\Package.Variables[IsPrimaryNode].Value\";" + bIsPrimaryNode + " /SET \"\\Package.Variables[ServernameNode2].Value\";" + "\"" + strServernameNode2 + "\"" + " /SET \"\\Package.Variables[ServernameNode3].Value\";" + "\"" + strServernameNode3 + "\"" +
                                                                                               " /SET \"\\Package.Variables[tgtFilePath].Value\";" + "\"" + strALSFilePath + "\"";

                        }
                        else if (strType.ToUpper() == "CASCADEEXP")
                        {
                            proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + "00" + " /SET \"\\Package.Variables[trancheNumber].Value\";" + strTrancheNum +
                                                                                                 " /SET \"\\Package.Variables[databaseNode2].Value\";" + "\"" + strDatabaseNode2 + "\"" + " /SET \"\\Package.Variables[databaseNode3].Value\";" + "\"" + strDatabaseNode3 + "\"" +
                                                                                                " /SET \"\\Package.Variables[IsMultiNode].Value\";" + bIsMultiNode + " /SET \"\\Package.Variables[IsPrimaryNode].Value\";" + bIsPrimaryNode + " /SET \"\\Package.Variables[ServernameNode2].Value\";" + "\"" + strServernameNode2 + "\"" + " /SET \"\\Package.Variables[ServernameNode3].Value\";" + "\"" + strServernameNode3 + "\"" +
                                                                                                " /SET \"\\Package.Variables[tgtFilePath].Value\";" + "\"" + strExpFilePath + "\"";

                        }
                        else if (strType.ToUpper() == "CASCADEMORT")
                        {
                            proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + "00" + " /SET \"\\Package.Variables[trancheNumber].Value\";" + strTrancheNum +
                                                                                                 " /SET \"\\Package.Variables[databaseNode2].Value\";" + "\"" + strDatabaseNode2 + "\"" + " /SET \"\\Package.Variables[databaseNode3].Value\";" + "\"" + strDatabaseNode3 + "\"" +
                                                                                                " /SET \"\\Package.Variables[IsMultiNode].Value\";" + bIsMultiNode + " /SET \"\\Package.Variables[IsPrimaryNode].Value\";" + bIsPrimaryNode + " /SET \"\\Package.Variables[ServernameNode2].Value\";" + "\"" + strServernameNode2 + "\"" + " /SET \"\\Package.Variables[ServernameNode3].Value\";" + "\"" + strServernameNode3 + "\"" +
                                                                                                " /SET \"\\Package.Variables[tgtFilePath].Value\";" + "\"" + strMortFilePath + "\"";

                        }
                        else if (strType.ToUpper() == "CASCADEAKA")
                        {
                            proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + "00" + " /SET \"\\Package.Variables[trancheNumber].Value\";" + strTrancheNum +
                                                                                                " /SET \"\\Package.Variables[databaseNode2].Value\";" + "\"" + strDatabaseNode2 + "\"" + " /SET \"\\Package.Variables[databaseNode3].Value\";" + "\"" + strDatabaseNode3 + "\"" +
                                                                                                " /SET \"\\Package.Variables[IsMultiNode].Value\";" + bIsMultiNode + " /SET \"\\Package.Variables[IsPrimaryNode].Value\";" + bIsPrimaryNode + " /SET \"\\Package.Variables[ServernameNode2].Value\";" + "\"" + strServernameNode2 + "\"" + " /SET \"\\Package.Variables[ServernameNode3].Value\";" + "\"" + strServernameNode3 + "\"" +
                                                                                                " /SET \"\\Package.Variables[tgtFilePath].Value\";" + "\"" + strAKAFilePath + "\"";

                        }

                        else if (strType.ToUpper() == "CASCADERPT") 
                        {
                            proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + "00" + " /SET \"\\Package.Variables[trancheNumber].Value\";" + strTrancheNum +                                                                                               
                                                                                                " /SET \"\\Package.Variables[IsMultiNode].Value\";" + bIsMultiNode + " /SET \"\\Package.Variables[IsPrimaryNode].Value\";" + bIsPrimaryNode + " /SET \"\\Package.Variables[ServernameNode2].Value\";" + "\"" + strServernameNode2 + "\"" + " /SET \"\\Package.Variables[ServernameNode3].Value\";" + "\"" + strServernameNode3 + "\"" +
                                                                                                " /SET \"\\Package.Variables[tgtFilePath].Value\";" + "\"" + strRptFilePath + "\"";

                        }
                        
                    }
                    else
                    {
                        if (bEnableLog)
                            Utility.Audit(STRSFMLOGCOMP, STRSFMSTARTLOG + "Unable to make link server connection between primary and non primary node. ", StrConString);
                        //to Skip execution of packages, initializing  to non zero value
                        errHeader = 99;
                    }

                }
                //ADDED TRANSCUSTXREF AND STATS FOR DOUBLE SWITCH AND STATS GEATHEREING
                else if (strType.ToUpper() == "TRANSFORMATION" || strType.ToUpper() == "TRANSCUSTXREF" || strType.ToUpper() == "STATS")
                {

                    proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber;

                }

                else if (strType.ToUpper() == "X02CNT" || strType.ToUpper() == "X03CNT")
                {

                    proStartPkg.StartInfo.Arguments = proStartPkg.StartInfo.Arguments + " /SET \"\\Package.Variables[chunkNumber].Value\";" + strChunkNumber + " /SET \"\\Package.Variables[ReleaseName].Value\";" + strReleaseName;

                }

                
                if (errHeader == 0 && errFooter == 0)
                {
                    if (strEnableLQStatus.Trim().ToLower() == "true")
                        bEnableLQStatus = true;
                    //AKS> Commented as status in loadqueue updated as 'STARTED' but file is locked by another processes and moved this method at line 1001
                    //if (bEnableLQStatus)
                    //    UpdateLoadQueueStatus(strPackageName, strChunkNumber, strTrancheNum);

                    string PackageOutputSavePathFolder = @strPackageOutputPathFolder;
                    string PackageOutput = string.Empty;
                    DateTime dtNow = DateTime.Now;
                    string PackageOutputSavePath = string.Empty, CurrentDate = string.Empty;
                    if (bEnablePackageOutput)
                    {
                        CurrentDate = dtNow.Year.ToString() +
                                (dtNow.Month.ToString()).PadLeft(2, '0') +
                                (dtNow.Day.ToString()).PadLeft(2, '0') +
                                (dtNow.Hour.ToString()).PadLeft(2, '0') +
                                (dtNow.Minute.ToString()).PadLeft(2, '0') +
                                (dtNow.Second.ToString()).PadLeft(2, '0');
                        PackageOutputSavePath = Path.Combine(PackageOutputSavePathFolder, strPackageName.Replace(".dtsx", "") + "_" + CurrentDate);


                        proStartPkg.StartInfo.UseShellExecute = false;
                        proStartPkg.StartInfo.RedirectStandardOutput = true;
                    }

                    //Start the Process to execute the package only for source because checking the source file is locked or not
                    if (strType.ToUpper() == "SOURCE")
                    {
                       
                        F = new FileInfo(SrcFilePath + Path.GetFileNameWithoutExtension(strSourceFileName) + ".csv");
                        
                        bool isFileLock = IsFileLocked(F);
                        if (!isFileLock)
                        {
                            //Updating loadqueue status as 'STARTED' if file is not locked by another processes
                            if (bEnableLQStatus)
                                UpdateLoadQueueStatus(strPackageName, strChunkNumber, strTrancheNum);

                            proStartPkg.Start();
                            if (bEnablePackageOutput)
                            {
                                PackageOutput = proStartPkg.StandardOutput.ReadToEnd();
                                proStartPkg.WaitForExit();
                                SaveOutput(PackageOutputSavePath, PackageOutput, strpac);
                            }
                            if (bEnableLog)
                                Utility.Audit(STRSFMLOGCOMP, STRSFMSTARTLOG + " for source  type " + strType + " and started the package " + strPackageName, StrConString);
                        }
                    }
                    //AKS> Start the Process to execute the packages (Control, Stats, Transformation, X02, X02CNT) except source
                    else
                    {

                        //Updating loadqueue status as 'STARTED' if file is not locked by another processes
                        if (bEnableLQStatus)
                            UpdateLoadQueueStatus(strPackageName, strChunkNumber, strTrancheNum);

                        proStartPkg.Start();
                        if (bEnablePackageOutput)
                        {
                            PackageOutput = proStartPkg.StandardOutput.ReadToEnd();
                            proStartPkg.WaitForExit();
                            SaveOutput(PackageOutputSavePath, PackageOutput, strpac);
                        }
                        if (bEnableLog)
                            Utility.Audit(STRSFMLOGCOMP, STRSFMSTARTLOG + " for source  type " + strType + " and started the package " + strPackageName, StrConString);
                    }

                    
                }
                else
                {
                    if (bEnableLog && errHeader != 99)
                        Utility.Audit(STRSFMLOGCOMP, "There is a mismatch in header and footer in source file " + strSourceFileName + ". HeaderMsg: " + strHeader + " and FooterMSG:" + strFooter, StrConString);

                }

            }
            catch (Exception ex)
            {
                // Logging exception
                Utility.Exception_System(strTrancheNum, strChunkNumber, STRSFMLOGCOMP, "ExceutePackage()", "STM", ex.Message + "Package name : " + strPackageName, StrConString);
                // Exiting the application
                Environment.Exit(3);
            }


        }
        /// <summary>
        /// Output of package is saved to following path
        /// </summary>
        /// <param name="PathToSave"></param>
        /// <param name="TextToSave"></param>
        public static void SaveOutput(string PathToSave, string TextToSave, string strpac)
        {
            try
            {
                using (StreamWriter X02FH1_Writer = new StreamWriter(PathToSave))
                {
                    X02FH1_Writer.WriteLine(TextToSave);
                }

            }
            catch (Exception ex)
            {
                // Logging exception
                Utility.Exception_System("01", strChunkNumber, STRSFMLOGCOMP, "SaveOutput()", "STM", ex.Message + "Package name : " + strpac, StrConString);
            }
        }
        private static bool IsPackageRunning(string strPackageName)
        {
            string CurrentDate = string.Empty;
            DateTime dtNow = DateTime.Now;
            bool bPackageRunning = false;
            CurrentDate = dtNow.Year.ToString() +
                              (dtNow.Month.ToString()).PadLeft(2, '0') +
                              (dtNow.Day.ToString()).PadLeft(2, '0') +
                              (dtNow.Hour.ToString()).PadLeft(2, '0') +
                              (dtNow.Minute.ToString()).PadLeft(2, '0') +
                              (dtNow.Second.ToString()).PadLeft(2, '0');
            var p = Process.GetProcessesByName("DTExec");
            foreach (var item in p)
            {
                string str = item.StartInfo.Arguments;
                str = str + " FILE NAME " + item.StartInfo.FileName;
                SaveOutput(@"D:\IsPackageRunning" + strPackageName + "_" + CurrentDate, str,strPackageName);
                if (str.Contains(strPackageName))
                {
                    bPackageRunning = true;
                }
            }
            return bPackageRunning;
        }

        private static void csvPreprocessor(string rawfile, string processedfile)
        {
            try
            {
                if (strIsScrambled.Trim().ToLower() == "true")
                {
                    //AKS> Calling CSVProcessor_Processed.exe if strIsScrambled is true
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"C:\Unisys\UDTS\Components\CSVProcessor_Processed.exe ";
                    startInfo.Arguments = rawfile; //It is case sensitive. If actual path is in UPPER case then same should be in config file also 
                    startInfo.RedirectStandardOutput = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = false;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    Process.Start(startInfo);
                    

                }
                else
                {
                    //AKS> Calling CSVPreProcessor.exe if strIsScrambled is false
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"C:\Unisys\UDTS\Components\CSVPreProcessor.exe ";
                    startInfo.Arguments = rawfile; //It is case sensitive. If actual path is in UPPER case then same should be in config file also 
                    startInfo.RedirectStandardOutput = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = false;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    Process.Start(startInfo);

                   
                }

              

            }
            catch (Exception ex)
            {
                //Loging the exception
                Utility.Exception_System("01", strChunkNumber, STRSFMLOGCOMP, "csvPreprocessor()", "STM", ex.Message, StrConString);
                Environment.Exit(0);

            }

        }

        /// <summary>
        /// Code to get the count of the link server which are connected to primary node
        /// </summary>
        /// <param name="strServernameNode2"></param>
        /// <param name="strServernameNode2_2"></param>
        /// <returns></returns>
        private static int CheckLinkServerConnection(string strServernameNode2, string strServernameNode3)
        {
            //Variable to load no of package in progress 
            int nConnExist = 0;
            // create an instance of sql connection
            SqlConnection objCon = new SqlConnection(StrConString);

            // Pass the Stored Procedure name
            SqlCommand objCmd = new SqlCommand("PR_UDMH_STM_CHK_LNKSERVER_CONN", objCon);

            // selecting the command Type as SSP
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.Parameters.Add("@servernameNode2", SqlDbType.VarChar).Value = strServernameNode2;
            objCmd.Parameters.Add("@servernameNode3", SqlDbType.VarChar).Value = strServernameNode3;
            try
            {
                if (objCon.State == ConnectionState.Closed)
                    objCon.Open();
                nConnExist = (int)objCmd.ExecuteScalar();
            }

            catch (Exception ex)
            {
                // Logging exception
                Utility.Exception_System(strTrancheNumber, strChunkNumber, STRSFMLOGCOMP, "CheckLinkServerConnection()", "STM", ex.Message, StrConString);
                // Exiting the application
                Environment.Exit(2);
            }
            finally
            {
                //Closing connection
                if (objCon.State == ConnectionState.Open)
                    objCon.Close();
                //Disposing Connection Object
                objCon.Dispose();
            }
            return nConnExist;

        }

        /// <summary>
        /// Method to update status in loadqueue table
        /// </summary>
        /// <param name="strPackageName"></param>
        ///  <param name="strChunkNum"></param>
        ///  <param name="strTranchNum"></param>
        private static void UpdateLoadQueueStatus(string strPackageName, string strChunkNum, string strTranchNum)
        {
            SqlConnection objCon = new SqlConnection(StrConString);

            // Pass the Stored Procedure name
            SqlCommand objCmd = new SqlCommand("PR_UDMH_STM_Update_LQ", objCon);

            // selecting the command Type as SSP
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.Parameters.Add("@PackageName", SqlDbType.VarChar).Value = strPackageName.Replace(".dtsx", "");
            objCmd.Parameters.Add("@ChunkNum", SqlDbType.VarChar).Value = strChunkNum;
            objCmd.Parameters.Add("@TrancheNum", SqlDbType.VarChar).Value = strTranchNum;
            try
            {
                if (objCon.State == ConnectionState.Closed)
                    objCon.Open();
                objCmd.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                // Logging exception
                Utility.Exception_System(strTrancheNumber, strChunkNumber, STRSFMLOGCOMP, "UpdateLoadQueueStatus()", "STM", ex.Message, StrConString);
                // Exiting the application
                Environment.Exit(2);
            }
            finally
            {
                //Closing connection
                if (objCon.State == ConnectionState.Open)
                    objCon.Close();
                //Disposing Connection Object
                objCon.Dispose();
            }
        }
        #endregion

        #region<<Code to generate acknowledgement file>>
        /// <summary>
        /// Code to generate acknowledgement file
        /// </summary>
        /// <param name="strPackageName"></param>
        private static void GenerateAcknowledgeMentFile(string strPackagename, string strErrorMessage, string strChunkNumber)
        {
            //Variable to hold the filename name to pass to Database.
            string strDBFilename = string.Empty;
            SqlConnection conn = new SqlConnection(StrConString);
            SqlCommand cmd = new SqlCommand("PR_UDMH_ACK_FILEGEN", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ErrorCode", SqlDbType.VarChar).Value = strErrorMessage;
            cmd.Parameters.Add("@ChunkNumber", SqlDbType.VarChar).Value = strChunkNumber;
            cmd.Parameters.Add("@PackageName", SqlDbType.VarChar).Value = strPackagename.Replace(".dtsx", "");
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                // strAckFile = (string)cmd.ExecuteScalar();
                // cmd.ExecuteScalar();
                cmd.ExecuteNonQuery();
                //close the connection object
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            catch (Exception ex)
            {

                if (conn.State == ConnectionState.Open)
                    conn.Close();
                Utility.Exception_System(strTrancheNumber, strChunkNumber, STRSFMLOGCOMP, "GenerateAcknowledgeMentFile()", "STM", ex.Message, StrConString);
                // Exiting the application
                Environment.Exit(2);
            }

        }
        #endregion

        #region<<Code to get the list of Package which are ready to execute from loadqueue table>>
        /// <summary>
        /// Code to get the list of Package which are ready to execute from loadqueue table
        /// </summary>
        /// <returns></returns>
        public static IList<UDTS_STM_LoadQueue> GetDataFromLoadQueue()
        {
            IList<UDTS_STM_LoadQueue> LoadQueue = null;
            LoadQueue = new List<UDTS_STM_LoadQueue>();
            // create an instance of sql connection
            SqlConnection conn = new SqlConnection(StrConString);
            // Pass the Stored Procedure name
            SqlCommand cmd = new SqlCommand("PR_UDMH_STM_GET_DATA_LOADQUEUE", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                if (dr != null)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            UDTS_STM_LoadQueue obj_Type = new UDTS_STM_LoadQueue();
                            obj_Type.chunkNumber = dr["ChunkNumber"].ToString().Trim();
                            obj_Type.tranchNumber = dr["TrancheNumber"].ToString().Trim();
                            obj_Type.SourceFileName = dr["Sourcename"].ToString().Trim();
                            obj_Type.packageName = dr["SSIS_Package_Name"].ToString().Trim();
                            obj_Type.SourceTableName = dr["Staging_Table_Name"].ToString().Trim();
                            obj_Type.Type = dr["CATEGORY"].ToString().Trim();
                            obj_Type.Priority = dr["Priority"].ToString().Trim();
                            obj_Type.Status = dr["Status"].ToString().Trim();
                            LoadQueue.Add(obj_Type);
                        }
                    }
                    //checking for datareader is opened
                    if (!dr.IsClosed)
                        dr.Close();
                }
                //close the connection object
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            catch (Exception ex)
            {
                if (!dr.IsClosed)
                    dr.Close();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                Utility.Exception_System(strTrancheNumber, strChunkNumber, STRSFMLOGCOMP, "GetDataFromLoadQueue()", "STM", ex.Message, StrConString);
                // Exiting the application
                Environment.Exit(2);
            }
            return LoadQueue;
        }
        #endregion

        #region<<Method  to get Number of packages are in Progress>>
        /// <summary>
        /// Method  to get Number of packages are in Progress
        /// </summary>
        /// <returns></returns>
        public static int NoOfPackageInProgress()
        {
            //Variable to load no of package in progress 
            int nInprogressCount = 0;
            // create an instance of sql connection
            SqlConnection objCon = new SqlConnection(StrConString);
            // Pass the Stored Procedure name
            SqlCommand objCmd = new SqlCommand("PR_UDMH_STM_NUM_PKG_Progress", objCon);
            // selecting the command Type as SSP
            objCmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (objCon.State == ConnectionState.Closed)
                    objCon.Open();
                nInprogressCount = (int)objCmd.ExecuteScalar();
            }

            catch (Exception ex)
            {
                // Logging exception
                Utility.Exception_System(strTrancheNumber, strChunkNumber, STRSFMLOGCOMP, "NoOfPackageInProgress()", "STM", ex.Message, StrConString);
                // Exiting the application
                Environment.Exit(2);
            }
            finally
            {
                //Closing connection
                if (objCon.State == ConnectionState.Open)
                    objCon.Close();
                //Disposing Connection Object
                objCon.Dispose();
            }
            return nInprogressCount;
        }

        #endregion


    }
}

