#region << Revision History>>
/**
 * Author: Aditya
 *
 * Creation Date: 22nd January 2015.
 * 
 * Description: This code takes a source file name as an input parameter from conect direct and initiates the associated package.
 *
 *
 * Revision History:
 *-----------------------------------------------------------------------------------------------------
 * Version    	Date        	Developer         		Description
 *-----------------------------------------------------------------------------------------------------
 * 1.0.0.0     	22/01/2015 	    Aditya		            Initial Version
 * 1.0.0.1      05/06/2015      Amit                    PHOENIX Version
 * 1.1.0.0      11/08/2015      Amit                    Deleted Irrelevant methods (Casscade and XO3..etc) and added reference to updated version of Utility.dll
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
using System.IO;
using Unisys.UDTS.Utility;
using System.Text.RegularExpressions;
#endregion


namespace UNISYS.UDTS.SourceDataManager
{
    /// <summary> 
    /// This code takes a source file name as an input parameter from conect direct and initiates the associated package.
    /// </summary>
    public class UDTH_SDM
    {
        /// <summary>
        ///  code to get connection String from config file
        /// </summary>
        static string StrConString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

        /// <summary>
        /// Code to get Package path  from config file.
        /// </summary>
        static string StrPkgPath = ConfigurationManager.ConnectionStrings["PkgPath"].ConnectionString;

        /// <summary>
        /// Code to get log enabler flag from config file.
        /// </summary>
        static string strEnableLog = ConfigurationManager.ConnectionStrings["EnableLog"].ConnectionString;

        /// <summary>
        /// Code to get  Priority value from config file.
        /// </summary>
        static int nPriority = Convert.ToInt16(ConfigurationManager.ConnectionStrings["Priority"].ConnectionString);

        /// <summary>
        /// Code to get Source type from config file.
        /// </summary>
        static string strSourceType = ConfigurationManager.ConnectionStrings["SourceType"].ConnectionString;

        /// <summary>
        /// Constant to store the application name.
        /// </summary>
        const string STRSDMLOGCOMP = "SDM Application";

        /// <summary>
        /// Constant to store the log for SFM start.
        /// </summary>
        const string STRSDMSTARTLOG = "SDM Application Started";

        /// <summary>
        /// Constant to store the log for SFM completion.
        /// </summary>
        const string STRSDMCOMPLOG = "SDM Application completed";



        #region << Main Class >>
        /// <summary>
        /// This code accepts a file name as a parameter from the Connect Direct and executes the associated SSIS package using Dtexec.exe SSIS utility.
        /// </summary>
        static void Main(string[] args)
        {

            // Variable to hold the name of the source file
            string strSourceName = string.Empty;

            // Variable to hold the category of the source file
            string strSourceCat = string.Empty;

            // Variable to hold the name of the package
            string strPackageName = string.Empty;

            //Variable to hold the chunk number.
            string strChunkNumber = string.Empty;

            //Variable to hold the Tranch number.
            string strTrancheNumber = string.Empty;

            //Variable to hold the Staging Table Name.
            string strStagingTableName = string.Empty;

            //Variable to hold the filename name to pass to Database.
            string strDBFilename = string.Empty;

            // variable to hold the log enabler
            bool bEnableLog = true;

            int nResult = -1;

            if (strEnableLog.Trim().ToLower() == "false")
                bEnableLog = false;

            // create an instance of sql connection
            SqlConnection objCon = null;
            try
            {
                //Check if the number of parameter is one
                if (args.Length < 1)
                {

                    // Invoke Audit method to log information
                    if (bEnableLog)
                        Utility.Audit(STRSDMLOGCOMP, "Started & ended" + " with error: invalid number of aruguments. Example: PHOENIX-SDM <Sourcename>", StrConString);

                    //Exiting the application
                    Environment.Exit(0);
                }

                //Get source file name
                strSourceName = args[0].ToString().Trim();
                strSourceCat = args[1].ToString().Trim();

                //Invoking Audit method for logging for starting
                if (bEnableLog)
                    Utility.Audit(STRSDMLOGCOMP, STRSDMSTARTLOG + " for " + strSourceName, StrConString);


                //Get the uniqueness of the source file
                int nSegPos = 0;

                if (strSourceCat == "SOURCE")
                {

                        strDBFilename = strSourceName;
                        strSourceType = strSourceCat;
                }

                else if (strSourceCat == "CONTROL")
                {
                    if (strSourceName.ToLower().Contains(".ctl.sta"))
                    {
                        nSegPos = strSourceName.ToLower().IndexOf(".ctl.sta");
                        strDBFilename = strSourceName.Substring(0, nSegPos + 8);
                        nPriority = 1;
                        strSourceType = strSourceCat;
                    }
                    else if (strSourceName.ToLower().Contains(".ctl.end"))
                    {
                        nSegPos = strSourceName.ToLower().IndexOf(".ctl.end");
                        strDBFilename = strSourceName.Substring(0, nSegPos + 8);
                        nPriority = 1;
                        strSourceType = strSourceCat;
                    }
                }
                else
                {
                    strDBFilename = strSourceName;      // pass the filename as it to the DB

                }
                
                //calling a method to load Pre data.
                if (strSourceType=="SOURCE")
                {
                    LoadPredata(strDBFilename, bEnableLog);    
                }              
                objCon = new SqlConnection(StrConString);
                // Pass the Stored Procedure name
                SqlCommand objCmd = new SqlCommand("PR_UDMH_SDM_Check_File_Package", objCon);

                // selecting the command Type as SP
                objCmd.CommandType = CommandType.StoredProcedure;

                // passing the parameters
                objCmd.Parameters.Add("@fileName", SqlDbType.VarChar).Value = strDBFilename;
                objCmd.Parameters.Add("@packagePath", SqlDbType.VarChar).Value = StrPkgPath;
                objCmd.Parameters.Add("@category", SqlDbType.VarChar).Value = strSourceType;
                objCmd.Parameters.Add("@priority", SqlDbType.VarChar).Value = nPriority;
                objCmd.Parameters.Add("@audit_enable", SqlDbType.Bit).Value = bEnableLog;
                objCmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;
                //Checking Connection state & opening connection
                if (objCon.State == ConnectionState.Closed)
                    objCon.Open();

                // Execute the stored Procedure using a ExecuteScalar
                 objCmd.ExecuteScalar();

                //Get the output parameter from DB 
                nResult = Convert.ToInt32(objCmd.Parameters["@result"].Value);


                //Invoking Audit method for logging of completion
                if (bEnableLog)
                    Utility.Audit(STRSDMLOGCOMP, STRSDMCOMPLOG + " for " + strSourceName, StrConString);


            }
            catch (Exception ex)
            {
                //Invoking Exception method
                Utility.Exception_System(strTrancheNumber, strChunkNumber, STRSDMLOGCOMP, strSourceName, "PHOENIX-SDM", ex.Message, StrConString);

                //Exiting the application
                Environment.Exit(1);
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
        /// Method to load Pre data based on file name
        /// </summary>
        /// <param name="strDBFilename"></param>
        /// <param name="bEnableLog"></param>
        private static void LoadPredata(string strDBFilename, bool bEnableLog)
        {
            SqlConnection objCon = new SqlConnection(StrConString);

            try
            {
                    objCon = new SqlConnection(StrConString);
                    // Pass the Stored Procedure name
                    SqlCommand objCmd = new SqlCommand("PR_UDMH_SDM_CHK_SEGMENT", objCon);

                    // selecting the command Type as SSP
                    objCmd.CommandType = CommandType.StoredProcedure;

                    // passing the parameters
                    objCmd.Parameters.Add("@segment", SqlDbType.VarChar).Value = "SEGMENT";
                    objCmd.Parameters.Add("@audit_enable", SqlDbType.Bit).Value = bEnableLog;
                    //Checking Connection state & opening connection
                    if (objCon.State == ConnectionState.Closed)
                        objCon.Open();
                    objCmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                //Invoking Exception method
                Utility.Exception_System("01", "01", STRSDMLOGCOMP, "01", "LoadPredata", ex.Message, StrConString);

                //Exiting the application
                Environment.Exit(1);
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

    }
}

