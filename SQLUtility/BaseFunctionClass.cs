/*
 * Author: Soumya
 * Creation Date: 23rd June 2015
 * Description: This code applies formatting for sql queries
 * Revision History:
 *-----------------------------------------------------------------------------------------------------
 * Version    	Date        	Developer         		Description
 *-----------------------------------------------------------------------------------------------------
   1.0          06/23/2015       Soumya              Initial version
 *-----------------------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Data;
using System.Configuration;
using System.Configuration.Provider;
using System.Text.RegularExpressions;
using Ini;
using System.Data.SqlTypes;


namespace SQLUtility
{
    public static class BaseFunctionClass
    {
        

        //To access INI file Variables
        public static string GetValueGivenKey()
        {

            IniFile ini = new IniFile("C:\\Unisys\\UDTS\\Components\\SQLException.ini");
            string Value = ini.IniReadValue("Section1", "Key1");

            return Value;
        }

        #region "date Function"
        //DATE
        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString FN_FORMATDATE(SqlDateTime FLD_VAL, SqlInt32 EXP_LEN, SqlString FLD_NAME, SqlString REC_TYPE, SqlString X02_KEY)
        {
            if (FLD_VAL.IsNull)
            {
                return "00000000";
            }
            DateTime Date = Convert.ToDateTime(FLD_VAL.ToString());

            string Date1 = Date.ToString("yyyyMMdd").Replace("-", "").Replace("/", "");

            if (Date1.Length == 8)
            {

                return Date1.Substring(0, 8);
            }
            if (FLD_VAL.ToString().Length == (int)EXP_LEN)
            {
                return FLD_VAL.ToString();
            }
            return new NotImplementedException().ToString();
        }
        #endregion

        #region UDF <FN_FORMATVARCHAR>

        [SqlFunction(DataAccess = DataAccessKind.Read)]
        public static SqlString FN_FORMATVARCHAR(SqlString FLD_VAL, SqlInt32 EXP_LEN, SqlString FLD_NAME, SqlString REC_TYPE, SqlString X02_KEY)
        {

            if (FLD_VAL.IsNull)
            {
                string var = string.Empty;
                return var.PadRight((int)EXP_LEN, ' ');
            }

            if (FLD_VAL.ToString().Length < (int)EXP_LEN)
            {
                return FLD_VAL.ToString().PadRight((int)EXP_LEN, ' ');
            }
            if (FLD_VAL.ToString().Length > (int)EXP_LEN)
            {
                FN_FORMAT_EXCEPTION(REC_TYPE.ToString(), "Length OverRun", "W", X02_KEY.ToString(), FLD_NAME.ToString(), FLD_VAL.ToString(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                return FLD_VAL.ToString().Substring(0, (int)EXP_LEN);
            }
            if (FLD_VAL.ToString().Length == (int)EXP_LEN)
            {
                return FLD_VAL.ToString();
            }
            return new NotImplementedException().ToString();

        }
        #endregion

        #region UDF <FN_FORMATINT>

        [SqlFunction(DataAccess = DataAccessKind.Read)]
        public static SqlString FN_FORMATINT(SqlInt64 FLD_VAL, SqlInt32 EXP_LEN, SqlString FLD_NAME, SqlString REC_TYPE, SqlString X02_KEY)
        {
            if (FLD_VAL.IsNull)
            {
                return Convert.ToString(string.Empty.PadRight((int)EXP_LEN, '0').ToString());
            }
            if (Convert.ToString(FLD_VAL).Length < (int)EXP_LEN)
            {
                return Convert.ToString(FLD_VAL.ToString().PadLeft((int)EXP_LEN, '0'));
            }
            if (Convert.ToString(FLD_VAL).Length > (int)EXP_LEN)
            {
                FN_FORMAT_EXCEPTION(REC_TYPE.ToString(), "Length OverRun", "W", X02_KEY.ToString(), FLD_NAME.ToString(), FLD_VAL.ToString(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                return FLD_VAL.ToString().Substring(0, (int)EXP_LEN);
            }
            if (FLD_VAL.ToString().Length == (int)EXP_LEN)
            {
                return FLD_VAL.ToString();
            }

            return new NotImplementedException().ToString();
        }
        #endregion

        #region UDF <FN_FORMATDECIMAL>

        [SqlFunction(DataAccess = DataAccessKind.Read)]
        public static SqlString FN_FORMATDECIMAL(SqlDecimal FLD_VAL, SqlInt32 EXP_LEN, SqlString FLD_NAME, SqlString REC_TYPE, SqlString X02_KEY)
        {
            if (FLD_VAL.IsNull)
            {
                return string.Empty.PadRight((int)EXP_LEN, '0');
            }

            string number = FLD_VAL.ToString();
            string[] a = new string[2];
            a = number.Split('.');
            char[] rev = a[1].ToCharArray();
            Array.Reverse(rev);
            string reversed = new string(rev);
            char[] revcharArray = reversed.ToCharArray();
            for (int i = 0; i < revcharArray.Length; i++)
            {
                if (revcharArray[i].Equals('0'))
                {
                    reversed = reversed.Remove(0, 1);
                }
                else
                {
                    break;
                }
            }
            char[] straightchar = reversed.ToCharArray();
            Array.Reverse(straightchar);
            string straight = a[0] + new string(straightchar);
            if (straight.Length == (int)EXP_LEN)
            {
                return straight.ToString();
            }
            if (straight.Length > EXP_LEN)
            {
                straight = straight.Substring(0, (int)EXP_LEN);
                FN_FORMAT_EXCEPTION(REC_TYPE.ToString(), "Length OverRun", "W", X02_KEY.ToString(), FLD_NAME.ToString(),  FLD_VAL.ToString(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,string.Empty);
                return straight.ToString();
            }
            if (straight.Length < EXP_LEN)
            {

                straight = straight.PadLeft((int)EXP_LEN, '0');
                return straight.ToString();
            }
            return new NotImplementedException().ToString();
        }
        
        #endregion

        #region UDF<FN_REGEXISMATCH>
        [SqlFunction(DataAccess = DataAccessKind.Read)]
        public static bool FN_REGEXISMATCH(string PATTERN, string INPUT)
        {
            bool bIsMatch = false;
            if (INPUT != null)
            {

                Regex r1 = new Regex(PATTERN.TrimEnd(null));
                bIsMatch = r1.Match(INPUT.TrimEnd(null)).Success;
            }
            return bIsMatch;
        }
        #endregion

        #region UDF<FN_REGEXMATCHREPLACE>
        [SqlFunction(DataAccess = DataAccessKind.Read)]
        public static string FN_REGEXMATCHREPLACE(string PATTERN, string INPUT)
        {
            string strMatchReplace = string.Empty;
            if (INPUT != null)
            {

                Regex r1 = new Regex(PATTERN.TrimEnd(null));
                strMatchReplace = r1.Replace(INPUT.TrimEnd(null), "");
            }
            return strMatchReplace;
        }
        #endregion

        #region UDF<FN_REGEXMATCHEXTRACT>
        [SqlFunction(DataAccess = DataAccessKind.Read)]
        public static string FN_REGEXMATCHEXTRACT(string PATTERN, string INPUT)
        {
            string strMatch = string.Empty;
            if (INPUT != null)
            {

                Regex r1 = new Regex(PATTERN.TrimEnd(null));
                Match matchInputPattern = r1.Match(INPUT.TrimEnd(null));
                if (matchInputPattern != null)
                {
                    strMatch = matchInputPattern.Value;
                }
            }
            return strMatch;
        }
        #endregion
          
        //Insertion when exception Occurs
        public static SqlString FN_FORMAT_EXCEPTION(String strEXCEPT_PACKAGE, String strEXCEPT_CODE, String strEXCEPT_SEVERITY, String strEXCEPT_ACCOUNT_NO, String strEXCEPT_SRC_FIELD_1,
            String strEXCEPT_SRC_VALUE_1, String strEXCEPT_SRC_FIELD_2, String strEXCEPT_SRC_VALUE_2, String strEXCEPT_SRC_FIELD_3, String strEXCEPT_SRC_VALUE_3,
            String strEXCEPT_SRC_FIELD_4, String strEXCEPT_SRC_VALUE_4, String strEXCEPT_SRC_FIELD_5, String strEXCEPT_SRC_VALUE_5, String strEXCEPT_SRC_FIELD_6,
            String strEXCEPT_SRC_VALUE_6)
        {
           
            // create an instance of sql connection
           SqlConnection con = new SqlConnection(GetValueGivenKey());

            // create an instance of sql command passing the SP name
            SqlCommand cmd = new SqlCommand("PR_UDMH_EXCEPTION_INSERT", con);

            // selecting the command Type as SSP
            cmd.CommandType = CommandType.StoredProcedure;

            //passing the parameters
            cmd.Parameters.Add("@EXCEPT_PACKAGE", SqlDbType.VarChar).Value = strEXCEPT_PACKAGE;
            cmd.Parameters.Add("@EXCEPT_CODE", SqlDbType.VarChar).Value = strEXCEPT_CODE;
            cmd.Parameters.Add("@EXCEPT_SEVERITY", SqlDbType.VarChar).Value = strEXCEPT_SEVERITY;
            cmd.Parameters.Add("@EXCEPT_ACCOUNT_NO", SqlDbType.VarChar).Value = strEXCEPT_ACCOUNT_NO;
            cmd.Parameters.Add("@EXCEPT_SRC_FIELD_1", SqlDbType.VarChar).Value = strEXCEPT_SRC_FIELD_1;
            cmd.Parameters.Add("@EXCEPT_SRC_VALUE_1", SqlDbType.VarChar).Value = strEXCEPT_SRC_VALUE_1;
            cmd.Parameters.Add("@EXCEPT_SRC_FIELD_2", SqlDbType.VarChar).Value = strEXCEPT_SRC_FIELD_2;
            cmd.Parameters.Add("@EXCEPT_SRC_VALUE_2", SqlDbType.VarChar).Value = strEXCEPT_SRC_VALUE_2;
            cmd.Parameters.Add("@EXCEPT_SRC_FIELD_3", SqlDbType.VarChar).Value = strEXCEPT_SRC_FIELD_3;
            cmd.Parameters.Add("@EXCEPT_SRC_VALUE_3", SqlDbType.VarChar).Value = strEXCEPT_SRC_VALUE_3;
            cmd.Parameters.Add("@EXCEPT_SRC_FIELD_4", SqlDbType.VarChar).Value = strEXCEPT_SRC_FIELD_4;
            cmd.Parameters.Add("@EXCEPT_SRC_VALUE_4", SqlDbType.VarChar).Value = strEXCEPT_SRC_VALUE_4;
            cmd.Parameters.Add("@EXCEPT_SRC_FIELD_5", SqlDbType.VarChar).Value = strEXCEPT_SRC_FIELD_5;
            cmd.Parameters.Add("@EXCEPT_SRC_VALUE_5", SqlDbType.VarChar).Value = strEXCEPT_SRC_VALUE_5;
            cmd.Parameters.Add("@EXCEPT_SRC_FIELD_6", SqlDbType.VarChar).Value = strEXCEPT_SRC_FIELD_6;
            cmd.Parameters.Add("@EXCEPT_SRC_VALUE_6", SqlDbType.VarChar).Value = strEXCEPT_SRC_VALUE_6;
           
            try
            {
                //Checking Connection state & opening connection
                if (con.State == ConnectionState.Closed)
                    con.Open();
                //execute stored procedure
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                // Display the exception message in console.
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                //Closing connection
                if (con.State == ConnectionState.Open)
                    con.Close();
                //Disposing Connection Object
                con.Dispose();

            }
            return "Exception handled";
        }
              
  

    }


}