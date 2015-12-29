using System.Configuration;
namespace UDHMHelperTool.BusinessLogic
{
    public class ConfigurationController
    {


        private static ConfigurationController _default = new ConfigurationController();

        public static ConfigurationController Default
        {
            get { return _default; }
        }

        public string DestinationPath
        {
            get { return ConfigurationManager.AppSettings["DestinationPath"].ToString(); }
        }
        public string ZipPath
        {
            get { return ConfigurationManager.AppSettings["ZipFilePath"].ToString(); }
        }
        public string FileListPath
        {
            get { return ConfigurationManager.AppSettings["FileListPath"].ToString(); }
        }



    }
}
