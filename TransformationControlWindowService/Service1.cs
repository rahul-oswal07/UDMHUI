# region <<Revision History>>

/**
 * Author: Narayan Swamy
 *
 * Creation Date:  	07/18/2011 	
 *
 * Description: This is a windows service code to get all the chunk numbers which are not started and check the control file if exists in load queue then start STM Application
 *
 *
 * Revision History:
 *-----------------------------------------------------------------------------------------------------
 * Version    	Date        	Developer         		Description
 *-----------------------------------------------------------------------------------------------------
 * 1.0.0.0	       	09/5/2013 	    Narayan		            Initial Version
 * 1.0.0.1          08/6/2015       Amit                    Phoenix Version
 * 1.1.0.0          12/08/2015      Amit                    Removed Irrelevant code for Casscade and XO3 and added reference to updated Utility.dll.
 *-----------------------------------------------------------------------------------------------------
 **/


# endregion

#region <<NameSpace>>
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Timers;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using Unisys.UDTS.Utility;
using System.Reflection;
using System.Configuration;
#endregion

namespace UNISYS.UDTS.WinService
{
    public partial class UDTS_WinService : ServiceBase
    {
        #region<<Variables & Constants used in TCWS Application>>
        /// <summary>
        /// To get Connection String from config file
        /// </summary>
        public static string StrConString = "";

        /// <summary>
        /// To get Number of target type from config file
        /// </summary>
        public static string StrNoOfX02TargetType = "";


        /// <summary>
        ///  To get the path to the execute exe  in the location specified in the config file
        /// </summary>
        public static string StrTsEexPath = "";

        /// <summary>
        ///  To get the path to the  STM execute exe  in the location specified in the config file
        /// </summary>
        public static string StrSTMExePath = "";

        /// <summary>
        /// To get log enabler flag fron config file.
        /// </summary>
        public static string strEnableLog = "";

        /// <summary>
        /// To get flag to stop service from the config file.
        /// </summary>
        public static string strStopService = "";

        /// <summary>
        ///  //To get Service Name from config file
        /// </summary>
        public static string StrServiceName = "";

        /// <summary>
        ///  //To get Service display Name from config file
        /// </summary>
        public static string StrServiceDispName = "";

        /// <summary>
        /// Declare the count variable to get the no. of segment 01 remaining to load or no. of chunk to start
        /// </summary>
        int iChunkCountToLoad = 0;


        /// <summary>
        /// DEclaring the constant Variable for component name
        /// </summary>
        const string STRWINDOWSERVICE = "Windows Service";

        /// <summary>
        /// Variable to hold the log enabler
        /// </summary>
        public static bool bEnableLog = true;

        /// <summary>
        /// Variable to hold the flag to stop the service after geting started
        /// </summary>
        public static bool bStopService = true;

        //Declaration of Timers to run the Service on Daily Basis
        private System.Timers.Timer timerWs = new System.Timers.Timer();

        //Declaration of Timers to run the Service on Daily Basis
        private System.Timers.Timer timerWsControlFile = new System.Timers.Timer();

        // Declaration of Variables used to track the timer Intervals for Control File
        private int intervalCF;

        // Declaration of Variables used to track the timer Intervals for Transformation 
        private int intervalTS;

        //Declaring  variable  to hold chunk number
        string strChunkNumber = string.Empty;

        //Declaring  variable  to hold tranch number
        string strTranchNumber = string.Empty;

        /// <summary>
        /// To control the Ts to start. one TS for one chunk even it fires multiple times.
        /// </summary>
        public static bool[] bChunktoStart = new bool[2];

        /// <summary>
        /// To get the path to the  file watcher execute exe  in the location specified in the config file
        /// </summary>
        public static string strFileWatcherEXEPath = string.Empty;

        /// <summary>
        /// To get flag to enable filewatcher from the config file.
        /// </summary>
        public static string strEnableFW = string.Empty;

        #endregion

        /// <summary>
        /// Initialize Windows services
        /// </summary>
        public UDTS_WinService()
        {
            InitializeComponent();
            StrConString = GetConfigurationValue("ConnectionString");
            StrNoOfX02TargetType = GetConfigurationValue("X02TargetTypeCount");
            StrTsEexPath = GetConfigurationValue("TSEXEPath");
            StrSTMExePath = GetConfigurationValue("STMEXEPath");
            strEnableLog = GetConfigurationValue("EnableLog");
            strStopService = GetConfigurationValue("StopServiceOnNoChunkTostart");
            StrServiceName = GetConfigurationValue("ServiceName"); //"UDTS_WinService";
            StrServiceDispName = GetConfigurationValue("ServiceDisplayName"); //"UDTS_WinService"; 
            //Set the value from config file
            if (strEnableLog.Trim().ToLower() == "false")
                bEnableLog = false;
            //Set the value from config file
            if (strStopService.Trim().ToLower() == "false")
                bStopService = false;
            if (StrServiceName.Trim().Length == 0)
            {
                StrServiceName = "UDTSWinService_Instance";
            }
            if (StrServiceDispName.Trim().Length == 0)
            {
                StrServiceDispName = "UGSI Data Transformation Solution Service Instance";
            }

            strFileWatcherEXEPath = GetConfigurationValue("FileWatcherEXEPath");
            strEnableFW = GetConfigurationValue("EnableFilewatcher");
            this.ServiceName = StrServiceName;

        }

        /// <summary>
        ///  Function to read the configuration items.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetConfigurationValue(string key)
        {
            Assembly service = Assembly.GetAssembly(typeof(UDTS_WinService));
            Configuration config = ConfigurationManager.OpenExeConfiguration(service.Location);
            if (config.AppSettings.Settings[key] != null)
            {
                return config.AppSettings.Settings[key].Value;
            }
            else
            {
                throw new IndexOutOfRangeException("UDTS_WinService: Settings collection does not contain the requested key:" + key);
            }
        }

        /// <summary>
        /// //Method that Executes on every start of Windows Service, timer is started.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            //To check filewatcher is enabled
            if (strEnableFW.ToUpper().Trim() == "TRUE")
            {
                StartFileWatcher();
            }
            this.OnStartService();
        }

        /// <summary>
        /// Method to initiate filewatcher application.
        /// </summary>
        public void StartFileWatcher()
        {
            Process objpro = new Process();
            // Geting the file name from  config file
            objpro.StartInfo.FileName = strFileWatcherEXEPath;            
            //Start the Process
            objpro.Start();
            objpro.Dispose();
            if (bEnableLog)
                Utility.Audit(STRWINDOWSERVICE, "FileWatcher initiated for Core files from WS", StrConString);
        }

        /// <summary>
        /// Added this public function to test from another form.
        /// </summary>
        public void OnStartService()
        {
            // on start read the config file parameters 
            StrNoOfX02TargetType = System.Configuration.ConfigurationManager.AppSettings["X02TargetTypeCount"];
            StrConString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            strEnableLog = System.Configuration.ConfigurationManager.AppSettings["EnableLog"];
            strStopService = System.Configuration.ConfigurationManager.AppSettings["StopServiceOnNoChunkTostart"];
            //Set the value from config file
            if (strEnableLog.Trim().ToLower() == "false")
                bEnableLog = false;
            //Set the value from config file
            if (strStopService.Trim().ToLower() == "false")
                bStopService = false;

            //objUtility.Audit(STRWINDOWSERVICE, "Started the windows service", StrConString);
            if (bEnableLog)
                Utility.Audit(STRWINDOWSERVICE, "started the windows service: " + StrServiceName, StrConString);


            ////Initailzation of Event_Handler for the timer control, this Handler is triggered once the timer control is elapsed with the Preset interval.             
            timerWsControlFile.Elapsed += new System.Timers.ElapsedEventHandler(OnElapsedTime);

            //set interval to app config interval time  and coverting into ms eg 5 sec (= 5*1000 milliseconds)
            if (System.Configuration.ConfigurationManager.AppSettings["intervalCF"] != null && int.TryParse(System.Configuration.ConfigurationManager.AppSettings["intervalCF"], out intervalCF) && intervalCF > 0)
                intervalCF = (intervalCF * 1000);
            timerWsControlFile.Interval = intervalCF;

            ////Initailzation of Event_Handler for the timer control, this Handler is triggered once the timer control is elapsed with the Preset interval.             
            timerWs.Elapsed += new System.Timers.ElapsedEventHandler(OnElapsedTimeTS);

            //set interval to app config interval time  and coverting into ms eg 5 sec (= 5*1000 milliseconds)
            if (System.Configuration.ConfigurationManager.AppSettings["intervalTS"] != null && int.TryParse(System.Configuration.ConfigurationManager.AppSettings["intervalTS"], out intervalTS) && intervalTS > 0)
                intervalTS = (intervalTS * 1000);
            timerWs.Interval = intervalTS;

            //Setting the timer to start on every start of windows service.
            timerWsControlFile.Start();

            //Calling Method, on every start of windows service.
            this.CheckControlFile();

        }

        /// <summary>
        /// Event Handler for timer control, this event is triggered once the timer is elapsed with its preset interval.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            timerWsControlFile.Enabled = false;
            this.CheckControlFile();
        }

        /// <summary>
        /// Event Handler for timer control, this event is triggered once the timer is elapsed with its preset interval.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnElapsedTimeTS(object source, ElapsedEventArgs e)
        {
            timerWs.Enabled = false;
            this.ServiceTask();
            timerWs.Enabled = true;
        }

        /// <summary>
        /// Code to check the control file in load queue 
        /// </summary>
        private void CheckControlFile()
        {
            //Variable to load no control file status if count is greater than zero mean control file is there in load queue table
            int nControlfileStatus = 0;
            // create an instance of sql connection
            SqlConnection objCon = new SqlConnection(StrConString);

            // Pass the Stored Procedure name
            SqlCommand objCmd = new SqlCommand("PR_UDMH_TCWS_CheckControl_File", objCon);

            // selecting the command Type as SSP
            objCmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if (objCon.State == ConnectionState.Closed)
                    objCon.Open();
                nControlfileStatus = (int)objCmd.ExecuteScalar();
                if (nControlfileStatus > 0)
                {
                    //Declare the object  of process
                    Process objpro = new Process();
                    // Geting the file name from  config file
                    objpro.StartInfo.FileName = StrSTMExePath;
                    //Start the Process
                    objpro.Start();
                    objpro.Dispose();
                    if (bEnableLog)
                        Utility.Audit(STRWINDOWSERVICE, "Started the STM application: " + StrSTMExePath, StrConString);
                    //stop the control file timer
                    timerWsControlFile.Enabled = false;
                    timerWsControlFile.Stop();
                    // Method ot get the Chunk which are ready to execute          
                    this.ServiceTask();
                    //enabling the timer
                    timerWs.Enabled = true;
                    timerWs.Start();
                }
                else
                {
                    timerWsControlFile.Enabled = true;

                }

            }

            catch (Exception ex)
            {
                // Logging exception 
                Utility.Exception_System(strTranchNumber, strChunkNumber, "CheckControlFile()", "None", "WS", ex.Message, StrConString);
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

        /// <summary>
        /// //Method to get the chunk numbers which are not yet started, and start the chunk processing
        /// </summary>
        public void ServiceTask()
        {
            SqlConnection conn = new SqlConnection(StrConString);
            try
            {
                //Logging the Audit // Added by Prabhash
                if (bEnableLog)
                    Utility.Audit(STRWINDOWSERVICE, "Checking to start the transformation application for X02. X02 Target type count:" + StrNoOfX02TargetType, StrConString);
                Boolean bRefreshValueFlag = false;
                Boolean bChunkFound = false;
                string strRefreshValueFlang = "";
                strRefreshValueFlang = GetConfigurationValue("RefreshValueFlag");
                if (strRefreshValueFlang.ToUpper() == "TRUE")
                {
                    bRefreshValueFlag = true;
                }

                //To read and  refresh all the below mentioned  parameter 
                if (bRefreshValueFlag)
                {
                    //used GetConfigurationValue() or read the config file item during runtime as the appseting does not read the items in runtime.
                    StrNoOfX02TargetType = GetConfigurationValue("X02TargetTypeCount");
                    strEnableLog = GetConfigurationValue("EnableLog");
                    strStopService = GetConfigurationValue("StopServiceOnNoChunkTostart");
                }
                int nChunkNum = 0;
                //SqlConnection conn = new SqlConnection(StrConString);
                SqlCommand cmd = new SqlCommand("PR_UDMH_TCWS_GetChunkToStart", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                //Declaring and assigning the value as a parameter
                cmd.Parameters.Add("@X02TargetTypeCount", SqlDbType.Int).Value = StrNoOfX02TargetType; 
                //Declaring the output parameter
                cmd.Parameters.Add("@chunkCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters["@chunkCount"].Direction = ParameterDirection.Output;
                //set to zero to wait untill operation completes.
                cmd.CommandTimeout = 0;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                //Checking the null values of datareader
                if (dr != null)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {

                            //Get chunk and tranch values from database
                            strChunkNumber = dr["MODULE_TYPE"].ToString();
                           // strTranchNumber = dr["TRANCHE_NUMBER"].ToString();
                            if (strChunkNumber.ToUpper().Trim() == "1" || strChunkNumber.ToUpper().Trim() == "X02")
                            {
                                nChunkNum =1;
                            }
                            else if (strChunkNumber.ToUpper().Trim() == "2")
                            {
                                nChunkNum = 2;
                            }
                                
                            bChunkFound = true;//added by pcm
                            //Checking the wheather the exe exits in the exe path specified in config file
                            if (File.Exists(StrTsEexPath))
                            {
                                if (!bChunktoStart[nChunkNum - 1])//if chunk TS not started
                                {
                                    //Logging the Audit 
                                    if (bEnableLog)
                                        Utility.Audit(STRWINDOWSERVICE, "Started the TSA application for chunk " + strChunkNumber, StrConString);
                                    //Execute the EXE
                                    StartEXE(strChunkNumber, "01"); //strChunkNumber is nothing but X02
                                    bChunktoStart[nChunkNum - 1] = true;
                                }
                            }
                            else
                            {
                                //Logging the Audit
                                if (bEnableLog)
                                    Utility.Audit(STRWINDOWSERVICE, "EXE is missing in the path for target type " + strChunkNumber, StrConString);

                            }
                        }
                    }
                    dr.Close();
                }
                //Logging the Audit // Added by Prabhash
                if ((bEnableLog) && (!bChunkFound))
                    Utility.Audit(STRWINDOWSERVICE, "No Reocrd found to start the transformation application. X02 Target type count:" + StrNoOfX02TargetType, StrConString);
                // Geting the icheckCount for how many chunks are left
                iChunkCountToLoad = Convert.ToInt32(cmd.Parameters["@chunkCount"].Value);
                if ((iChunkCountToLoad == 0))//&& (bStopService)) // check how  many chunk to start 
                {
                    //variable to hold X02 completion status
                    int iX02CmpStatus = 0;
                    iX02CmpStatus = CheckX02CMP();
                    if (iX02CmpStatus == 1)
                    {
                      
                        if (bEnableLog)
                            Utility.Audit(STRWINDOWSERVICE, " X02 packages are Completed", StrConString);
                    }
                    

                }

            }
            catch (Exception ex)
            {
                Utility.Exception_System(strTranchNumber, strChunkNumber, STRWINDOWSERVICE, "ServiceTask()", "WS", ex.Message, StrConString);
                Environment.Exit(0);
            }
            finally
            {
                //Closing connection
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                //Disposing Connection Object
                conn.Dispose();
            }


        }

        /// <summary>
        /// Method to check x02 packages is executed for all the chunks.
        /// </summary>
        /// <returns></returns>
        private int CheckX02CMP()
        {
            //Variable to load no of package in progress 1--> X02 completed in  primary , 0--> not completed
            int iX02CMP = 0;
            // create an instance of sql connection
            SqlConnection objCon = new SqlConnection(StrConString);

            // Pass the Stored Procedure name
            SqlCommand objCmd = new SqlCommand("[PR_UDMH_TCWS_OPGENSTS]", objCon);

            // selecting the command Type as SSP
            objCmd.CommandType = CommandType.StoredProcedure;

            try
            {
                if (objCon.State == ConnectionState.Closed)
                    objCon.Open();
                iX02CMP = (int)objCmd.ExecuteScalar();
            }

            catch (Exception ex)
            {
                // Logging exception
                Utility.Exception_System(strTranchNumber, strChunkNumber, STRWINDOWSERVICE, "CheckX02CMP()", "WS", ex.Message, StrConString);
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
            return iX02CMP;
        }
        
        /// <summary>
        /// Method used to execute the chunk which are ready to execute.
        /// </summary>
        /// <param name="chunkNumber" ></param>
        /// <param name="tranchNumber"></param>
        private void StartEXE(string strChunkNumber, string strTranchNumber)
        {
            try
            {
                //Declare the object  of process
                Process objpro = new Process();
                // Geting the file name from  config file
                objpro.StartInfo.FileName = StrTsEexPath;
                //Passing the parameter chunk  and tranch
                objpro.StartInfo.Arguments = strChunkNumber + " " + strTranchNumber; // Added space
                //Start the Process
                objpro.Start();
                objpro.Dispose();
            }
            catch (Exception ex)
            {
                //Loging the exception
                Utility.Exception_System(strTranchNumber, strChunkNumber, STRWINDOWSERVICE, "StartEXE()", "WS", ex.Message, StrConString);
                Environment.Exit(0);

            }
        }

        /// <summary>
        ///  //Method that Executes on every stop of Windows Service, stoping the timer when service is stoped.
        /// </summary>
        protected override void OnStop()
        {
            Array.Clear(bChunktoStart, 0, bChunktoStart.Length); //Reset to False.
            // To stop the service
            timerWs.Stop();
            //Loging the stop of the service
            if (bEnableLog)
                Utility.Audit(STRWINDOWSERVICE, "Stoped the windows service: " + StrServiceName, StrConString);
        }

    }
}
