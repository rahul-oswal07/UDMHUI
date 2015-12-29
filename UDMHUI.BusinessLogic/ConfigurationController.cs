using System.Configuration;

namespace UDMHUI.BusinessLogic
{
    public class ConfigurationController
    {

        private const string DBPATH = "DBPath";
        private const string LOOKUPPATH = "LookupPath";
        private const string DTEXECPATH = "DTExecPath";
        private const string SQLUTILITYPATH = "SQLUtilityPath";
        private const string XO2PATH = "XO2Path";
        private const string DATABASENAME = "DatabaseName";
        private const string SQLSERVERINSTANCENAME = "SQLServerInstanceName";
        private const string PACKAGEPATH = "PackagePath";


        private static ConfigurationController _default = new ConfigurationController();

        public static ConfigurationController Default
        {
            get { return _default; }
        }

        public string DBPath
        {
            get { return ConfigurationManager.AppSettings[DBPATH].ToString(); }
        }
        public string LookupPath
        {
            get { return ConfigurationManager.AppSettings[LOOKUPPATH].ToString(); }
        }
        public string DTExecPath
        {
            get { return ConfigurationManager.AppSettings[DTEXECPATH].ToString(); }
        }
        public string SQLUtilityPath
        {
            get { return ConfigurationManager.AppSettings[SQLUTILITYPATH].ToString(); }
        }
        public string XO2Path
        {
            get { return ConfigurationManager.AppSettings[XO2PATH].ToString(); }
        }
        public string DatabaseName
        {
            get { return ConfigurationManager.AppSettings[DATABASENAME].ToString(); }
        }
        public string SQLServerInstanceName
        {
            get { return ConfigurationManager.AppSettings[SQLSERVERINSTANCENAME].ToString(); }
        }
        public string PackagePath
        {
            get { return ConfigurationManager.AppSettings[PACKAGEPATH].ToString(); }
        }


        public string MasterDb
        {
            get { return ConfigurationManager.ConnectionStrings["masterdb"].ConnectionString; }
        }

        public string UDHMRUNDB
        {
            get { return ConfigurationManager.ConnectionStrings["UDMH"].ConnectionString; }
        }


        /// <summary>
        /// Change the value for specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void ChangeKeyValue(string key, string value, bool isConnectionString, bool isAppSetting)
        {
            if (isAppSetting)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection App_Section = (AppSettingsSection)config.GetSection("appSettings");
                KeyValueConfigurationCollection app_settings = App_Section.Settings;
                KeyValueConfigurationElement element = app_settings[key];
                if (element != null)
                {
                    element.Value = value;
                    app_settings.Remove(key);
                    app_settings.Add(element);
                    config.Save(ConfigurationSaveMode.Full, true);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
            if (isConnectionString)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringsSection connectionString_Section = (ConnectionStringsSection)config.GetSection("appSettings");
                ConnectionStringSettingsCollection connectionString_settings = connectionString_Section.ConnectionStrings;
                ConnectionStringSettings element = connectionString_settings[key];
                if (element != null)
                {
                    element.ConnectionString = "blah";
                    connectionString_settings.Remove(key);
                    connectionString_settings.Add(element);
                    config.Save(ConfigurationSaveMode.Full, true);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
        }
    }
}
