# region << Revision History >>
/**
 * Author: Narayan Swamy
 *
 * Creation Date:  3rd Sep 2013
 *
 * Description: This code will accept two parameter tranch and chunk number, for all  target type  it checks wheather 
//all dependent file are completed for a particular target type if yes execute the corresponding package .
 *
 *
 * Revision History:
 *-----------------------------------------------------------------------------------------------------
 * Version    	Date        	Developer         		Description
 *-----------------------------------------------------------------------------------------------------
 * 1.0.0.0     3-09-2013 	    Narayan		            Initial Version 
 * 1.0.0.1     8-06-2015        Amit                    Phoenix Version             
 * 1.1.0.0     13-08-2015       Amit                    Removed irrelevant code (Casscade and XO3..etc) and added reference to updated version of Utility.dll 
 * *-----------------------------------------------------------------------------------------------------
 **/

# endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unisys.UDTS.Utility;
using System.Timers;
using System.Data.SqlClient;
using System.Data;

namespace UNISYS.UDTS.TranformationServiceApplication
{
    class UDTH_TS
    {
        /// <summary>
        /// code to get connection String fron config file 
        /// </summary>
        static string StrConString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; 

        /// <summary>
        ///  //To get time delay  from application configuration file.
        /// </summary>
        static int interval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["interval"]);

        /// <summary>
        ///  //To get X02 Package name .
        /// </summary>
        static string X02PackageName = System.Configuration.ConfigurationManager.AppSettings["X02PackageName"];

        /// <summary>
        ///  //To get X02 count verify Package name .
        /// </summary>
        static string X02CountVerifyPackageName = System.Configuration.ConfigurationManager.AppSettings["X02CountVerifyPackageName"];

        /// <summary>
        /// Code to get log enabler flag fron config file.
        /// </summary>
        static string strEnableLog = System.Configuration.ConfigurationManager.AppSettings["EnableLog"];

        /// <summary>
        /// Declaring variable to get Parameters from cmd line arugument.
        /// </summary>
        static string StrFileType = string.Empty;
        static string StrTranchNumber = string.Empty;
        static string StrPackagename = string.Empty;
        const string STRTRANSFORMATIONNAME = "TSA";
        /// <summary>
        /// Variable to hold the log enabler
        /// </summary>
        public static bool bEnableLog = true;


        //Declaration of Timers to run the Service on Daily Basis 
        private static System.Timers.Timer timerTS = new System.Timers.Timer();


        /// <summary>
        /// This main class implements the logic for tranformation services for executing transformation packages
        /// </summary>
        /// <param name="args"></param>
        #region <<Method : Main >>
        static void Main(string[] args)
        {
            try
            {
                //Set the value from config file
                if (strEnableLog.Trim().ToLower() == "false")
                    bEnableLog = false;

                //loging the start of the chunk 
                StrFileType = args[0].ToString().Trim();
                if (bEnableLog)
                    Utility.Audit(STRTRANSFORMATIONNAME, "Started executing the packages for File type " + StrFileType, StrConString); 
                if (args.Length == 2)
                {
                    StrFileType = args[0].ToString().Trim();
                    StrTranchNumber = args[1].ToString().Trim();
                    //Initailzation of Event_Handler for the timer control, this Handler is triggered once the timer control is elapsed with the Preset interval.
                    //Replace callback function to proper timer elapsed function where you can enable and disbale the timer.
                    timerTS.Elapsed += new System.Timers.ElapsedEventHandler(OnElapsedTime);
                    timerTS.Interval = interval * 1000;
                    // Call the function time directly to start process and later it  will fire using the elapse functin event
                    CheckTargetType(StrTranchNumber, StrFileType);
                    timerTS.Enabled = true;
                    timerTS.Start();
                    Console.ReadLine();
                }
                else
                {
                    StrFileType = args[0].ToString().Trim();
                    //UDTS_Utility objUtility = new UDTS_Utility();
                    if (bEnableLog)
                        Utility.Audit(STRTRANSFORMATIONNAME, "Incorrect no. of aruguments passed in Transformation application for target type " + StrFileType + ".", StrConString);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                //adding exception to table
                Utility.Exception_System(StrTranchNumber, StrFileType, STRTRANSFORMATIONNAME, "Main()", "TS", ex.Message, StrConString);
                Environment.Exit(1);
            }

        }
        #endregion


        /// <summary>
        /// Event Handler for timer control, this event is triggered once the timer is elapsed with its preset interval. Also the time can be enabled and disable using this function.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        #region <<Method : OnElapsedTime >>
        private static void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            timerTS.Enabled = false;
            CheckTargetType(StrTranchNumber, StrFileType);
            timerTS.Enabled = true;
        }
        #endregion

        #region<<Methods used for TS application>>
        /// <summary>
        /// To check the Target type which are ready to load  and transfer data to load queue  table  ang get count if 
        /// </summary>
        /// <param name="StrTranchNumber"></param>
        /// <param name="StrChunkNumber"></param>
        private static void CheckTargetType(string StrTranchNumber, string StrFiletype) 
        {
            int icount = 0;
            //count for Exception and Reconciliation
            int iRecCount = 0;

            //variable to check x02 verify count packages are executed.
            int iX02cnt = 0;
            SqlConnection conn = new SqlConnection(StrConString);
            try
            {
                // Pass the Stored Procedure name
                SqlCommand cmd = new SqlCommand("PR_UDMH_TSA_loadQueue_TSPackage", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                // declaring  tranchnumber as parameter
                cmd.Parameters.Add("@tranchNumber", SqlDbType.NVarChar, 10);
                // declaring  file type 
                cmd.Parameters.Add("@fileType", SqlDbType.NVarChar, 10);
                // declaring  category 
                cmd.Parameters.Add("@Category", SqlDbType.NVarChar, 20);
                // declaring  count as output parameter 
                cmd.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
                // Pass the value tranchnumber to the SP
                cmd.Parameters["@tranchNumber"].Value = StrTranchNumber;
                // Pass the value file type to the SP
                cmd.Parameters["@fileType"].Value = StrFiletype;
                // Pass the value catrgory to the SP
                cmd.Parameters["@Category"].Value = "TRANSFORMATION";

                cmd.Parameters.Add("@audit_enable", SqlDbType.Bit).Value = bEnableLog;
                // Pass the value count to the SP as output parameter
                cmd.Parameters["@count"].Direction = ParameterDirection.Output;

                //set to zero to wait untill operation completes.
                cmd.CommandTimeout = 0;

                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                cmd.ExecuteScalar();
                icount = Convert.ToInt32(cmd.Parameters["@count"].Value);
                
                //if count is 0 mean all the target type for that chunk has been done
                if (icount == 0)
                {
                    if (StrFiletype.ToUpper() == "X02")
                        iRecCount = LoadX02PackageToLoadQueue(X02PackageName, "01", StrTranchNumber, "X02", "TB_UDMH_X02", 1, "GENERATE", "X02");

                    if (iRecCount == 1)
                    {

                        if (StrFiletype.ToUpper() == "X02")
                        {
                            LoadX02PackageToLoadQueue(X02CountVerifyPackageName, "01", StrTranchNumber, "X02CNTVERIFY", " ", 5, "X02CNT", "X02");
                        }
                        if (StrFiletype.ToUpper() == "X02")
                        {

                            iX02cnt = ChkX02countCMP("X02");
                        }

                        if (iX02cnt == 1)
                        {
                            if (bEnableLog)
                                Utility.Audit(STRTRANSFORMATIONNAME, "Closed the Transformation Application for the file type " + StrFileType, StrConString);
                            //To Stop the application
                            Environment.Exit(0);
                        }
                                          
                    }
                }

            }
            catch (Exception ex)
            {
                //Logging the exception
                Utility.Exception_System(StrTranchNumber, "01", STRTRANSFORMATIONNAME, "CheckTargetType()", "TS", ex.Message, StrConString);
                Environment.Exit(1);
            }
            finally
            {
                //Closing connection
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                //Disposing Connection Object
                conn.Dispose();
            }

        }

        private static int ChkX02countCMP(string filetype)
        {
            int iCountX02 = 0;
            SqlConnection conn = new SqlConnection(StrConString);
            try
            {
                SqlCommand cmd = new SqlCommand("PR_UDMH_TSA_TARGETCNT_CMP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@fileType", SqlDbType.VarChar).Value = filetype;
                              
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                iCountX02 = (int)cmd.ExecuteScalar();
            }

            catch (Exception ex)
            {
                //Invoking Exception method
                Utility.Exception_System("01", "01", STRTRANSFORMATIONNAME, "PR_UDMH_TSA_TARGETCNT_CMP()", "TS", ex.Message, StrConString);

                //Exiting the application
                Environment.Exit(3);
            }
            finally
            {
                //Closing connection
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                //Disposing Connection Object
                conn.Dispose();
            }
            return iCountX02;
            
        }

        /// <summary>
        /// Code to insert X02 Details into LoadQueue table
        /// </summary>
        /// <param name="X02PackageName"></param>
        /// <param name="StrChunkNumber"></param>
        /// <param name="StrTranchNumber"></param>
        private static int LoadX02PackageToLoadQueue(string strPackageName, string StrChunkNumber, string StrTranchNumber, string strSourceFilename, string strSourceTableName, int strpriority, string strCategory, string strfiletype)
        {
            int iCount = -1;
            SqlConnection conn = new SqlConnection(StrConString);
            try
            {

                // Pass the Stored Procedure name
                SqlCommand objCmd = new SqlCommand("PR_UDMH_TSA_LoadQueue_INSERT", conn);

                // selecting the command Type as SSP
                objCmd.CommandType = CommandType.StoredProcedure;

                //passing the parameters

                objCmd.Parameters.Add("@TrancheNumber", SqlDbType.VarChar).Value = StrTranchNumber;
                objCmd.Parameters.Add("@fileType", SqlDbType.VarChar).Value = strfiletype;
                objCmd.Parameters.Add("@SourceFilename", SqlDbType.VarChar).Value = strSourceFilename;
                objCmd.Parameters.Add("@SSIS_Package_Name", SqlDbType.VarChar).Value = strPackageName;
                objCmd.Parameters.Add("@Source_Table_Name", SqlDbType.VarChar).Value = strSourceTableName;
                objCmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = strCategory;
                objCmd.Parameters.Add("@Priority", SqlDbType.VarChar).Value = strpriority;
                objCmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = "READY";
                objCmd.Parameters.Add("@X02Count", SqlDbType.Int).Direction = ParameterDirection.Output; 

                //Checking Connection state & opening connection
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                //execute stored procedure
                objCmd.ExecuteNonQuery();

                iCount = Convert.ToInt32(objCmd.Parameters["@X02Count"].Value);

            }
            catch (Exception ex)
            {
                //Invoking Exception method
                Utility.Exception_System(StrTranchNumber, StrChunkNumber, STRTRANSFORMATIONNAME, "LoadX02PackageToLoadQueue()", "TS", ex.Message, StrConString);

                //Exiting the application
                Environment.Exit(3);
            }
            finally
            {
                //Closing connection
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                //Disposing Connection Object
                conn.Dispose();
            }

            return iCount;

        }

        #endregion



    }
}
