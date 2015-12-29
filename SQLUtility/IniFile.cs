/*
 * Author: Sowmya
 * Creation Date: 23rd June 2015
 * Description: This code applies for accessing Ini file
 * Revision History:
 *-----------------------------------------------------------------------------------------------------
 * Version    	Date        	Developer         		Description
 *-----------------------------------------------------------------------------------------------------
   1.0          06/23/2015       Sowmya              Initial version
 *-----------------------------------------------------------------------------------------------------
 */


using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Ini
{
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public IniFile(string INIPath)
        {
            path = INIPath;
        }


        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            255, this.path);
            return temp.ToString();

        }


        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }


    }
}