#region << Revision History>>
/**
 * Author: Rajneesh
 *
 * Creation Date: 5th November 2013
 *
 * Description: This application is used to split the X02 file based on the count passed.
 *
 *
 * Revision History:
 *-----------------------------------------------------------------------------------------------------
 * Version    	Date        	Developer         		Description
 *-----------------------------------------------------------------------------------------------------
 * 1.0     	05/11/2013 	    Rajneesh		        Initial Version
 * 1.1          12/11/2012      Narayan                 Fixed time out error and logging exception.
 * 1.2       10/02/2014      Nagendra/Aditya         Unicode Encoding added for Handling NON_ASCII Characters
 * 1.3       11/02/2014      Nagendra/Aditya         Encoding changed from Unicode to Code-1252
 * 
 *-----------------------------------------------------------------------------------------------------
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransformOutputSplit
{
    class Program
    {
        /// <summary>
        /// Code to get connection String from config file 
        /// </summary>
        static string constring = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];

        static void Main(string[] args)
        {
            Int32 split_cnt = Convert.ToInt32(args[0].ToString().Trim());
            String FileLoc = args[3].ToString().Replace("~", " ");
            String xsrctbl = args[2].ToString().Replace("~", " ");
            String gp = args[1].ToString();
            String source = args[4].ToString();

            //Int32 split_cnt = 5;
            //String FileLoc = "D:\\OUTPUT\\NANI\\";
            //String xsrctbl = "TBL_UDMH_X02";
            //String gp = "9263";

            //Unisys.UDTS.Utility.Utility.Audit("TRANSPLIT", "split_cnt " + split_cnt + " xsrctbl " + xsrctbl + " gp " + gp + " source " + source, constring);

            String srctbl = "";
            String tbltyp = "";

            if (xsrctbl == "TB_UDMH_X02")
            {
                srctbl = "[TB_UDMH_X02_SPLIT]";
                tbltyp = "X02";
            }


            dbConnection db = new dbConnection(constring);

            db.splitRecord(split_cnt, xsrctbl, srctbl, FileLoc, gp, tbltyp, source, constring);
        }
    }
}
