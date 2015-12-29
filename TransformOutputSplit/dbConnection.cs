using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Unisys.UDTS.Utility;

namespace TransformOutputSplit
{

    public class dbConnection
    {
        private SqlConnection con;
        private SqlDataReader ftrdr = null;
        private SqlDataReader sqlrdr = null;
        private SqlDataReader fhrdr = null;
        private Int32 group_rank;
        private Int32 group_cnt;
        private Int32 mod;
        private string strDataColumn = "";

        public dbConnection(String constring)
        {
            con = new SqlConnection();
            con.ConnectionString = constring;
        }

        private SqlConnection openConnection()
        {
            if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
            {
                con.Open();

            }
            return con;
        }

        private SqlConnection closeConnection()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return con;
        }


        public void splitRecord(Int32 split_cnt, String xsrctbl, String srctbl, String FileLoc, String grp_rnk, String tbltyp, String source, String Strconstring)
        {

            group_rank = Convert.ToInt32(grp_rnk);
            if (split_cnt != 0)
            {
                mod = group_rank % split_cnt;
                //   group_cnt = (group_rank / split_cnt);
                if (mod != 0)
                {
                    group_cnt = (group_rank / split_cnt) + 1;
                }
                else
                {
                    group_cnt = group_rank / split_cnt;
                }

                int i = 1;
                int j = 1;
      
                int dataset = group_cnt;


                if (mod < split_cnt && mod > 0)
                {
                    dataset = group_cnt;
                    mod--;
                }
                else
                {
                    dataset = group_cnt;
                }

                try
                {
                    int batchID = 1;
                    String defFileLoc = FileLoc;
                    SqlCommand fhcmd = new SqlCommand();
                    fhcmd.CommandTimeout = 0;   //Add by Narayan
                    fhcmd.Connection = openConnection();
                    fhcmd.CommandText = "SELECT Datacolumn FROM " + xsrctbl + " where RecordType= '" + tbltyp + "_FH1' ";
                    fhrdr = fhcmd.ExecuteReader();
                    while (fhrdr.Read())
                    {
                        strDataColumn += fhrdr[0].ToString();

                    }
                    
                    String strDataColumnWithBatchID = String.Concat(strDataColumn.Substring(0, 124), batchID.ToString().PadLeft(5, '0'));
                    String FH1Datacolumn = strDataColumnWithBatchID;
                    String DataColumn = strDataColumn;
                    fhcmd.Connection = closeConnection();
                    strDataColumn = null;
                    //strDataColumnWithBatchID = null;

                    while (i <= split_cnt)
                    {

                        FH1Datacolumn = String.Concat(DataColumn.Substring(0, 124), batchID.ToString().PadLeft(5, '0'));
                        batchID++;
                        if (j <= group_rank)
                        {
                            if (i > 0 && i <= 9)
                            {
                                FileLoc = FileLoc + source + tbltyp + "File_0" + i;
                            }
                            else
                            {
                                FileLoc = FileLoc + source + tbltyp + "File_" + i;
                            }
                       
                            StreamWriter X02FH1_Writer = new StreamWriter(FileLoc, false, Encoding.GetEncoding(1252));//ENCODING ADDED FOR HANDLING NON-ASCII CHARACTERS
                            X02FH1_Writer.WriteLine(FH1Datacolumn.PadRight(2000));
                            X02FH1_Writer.Close();


                            //Selecting all data from the table for the block of Group_Rank                           
                            SqlCommand cmd = new SqlCommand("PR_" + tbltyp + "_SPLIT_GEN", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;  //Add by Narayan
                            cmd.Parameters.Add(new SqlParameter("@START_AT", SqlDbType.Int)).Value = j;
                            cmd.Parameters.Add(new SqlParameter("@END_AT", SqlDbType.Int)).Value = group_cnt;
                            cmd.Connection = openConnection();
                            sqlrdr = cmd.ExecuteReader();
                            StreamWriter X02DataColumn_Writer = new StreamWriter(FileLoc, true, Encoding.GetEncoding(1252));//ENCODING ADDED FOR HANDLING NON-ASCII CHARACTERS
                            while (sqlrdr.Read())
                            {

                                strDataColumn = "\n" + sqlrdr[0].ToString();
                                X02DataColumn_Writer.WriteLine((strDataColumn.TrimStart()).PadRight(2000)); // Added by Narayan for memory out of bound exception.
                            }
                            sqlrdr.Close();
                            X02DataColumn_Writer.Close();

                            strDataColumn = null;
                            SqlCommand ftcmd = new SqlCommand("PR_" + tbltyp + "_FT1_SPLIT", con);
                            ftcmd.CommandType = CommandType.StoredProcedure;
                            ftcmd.CommandTimeout = 0;  //Add by Narayan
                            ftcmd.Parameters.Add(new SqlParameter("@START_AT", SqlDbType.Int)).Value = j;
                            ftcmd.Parameters.Add(new SqlParameter("@END_AT", SqlDbType.Int)).Value = group_cnt;
                            ftcmd.Connection = openConnection();
                            ftrdr = ftcmd.ExecuteReader();

                            while (ftrdr.Read())
                            {
                                strDataColumn += ftrdr[0].ToString();
                            }

                            String FT1Datacolumn = strDataColumn;
                            ftcmd.Connection = closeConnection();
                            strDataColumn = null;
                            StreamWriter X02FT1_Writer = new StreamWriter(FileLoc, true, Encoding.GetEncoding(1252));//ENCODING ADDED FOR HANDLING NON-ASCII CHARACTERS
                            X02FT1_Writer.WriteLine(FT1Datacolumn.PadRight(2000));
                            X02FT1_Writer.Close();
                            strDataColumn = "";
                            FT1Datacolumn = "";
                            FileLoc = defFileLoc;
                        }
                        
                        i++;
                        if (group_cnt <= group_rank)
                        {
                            j = group_cnt + 1;
                            group_cnt += dataset;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Logging exception
                    Utility.Exception_System("01", "01", "TRANSFORMSPLIT", "SplitRecord()", "SPLITOUT", ex.Message, Strconstring);
                    // Exiting the application
                    Environment.Exit(1);
                }
                finally
                {
                    //Closing connection
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    con.Dispose();
                }
            }
        }
    }
}

