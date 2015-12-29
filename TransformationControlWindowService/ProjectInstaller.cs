using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Configuration;


namespace UNISYS.UDTS.WinService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        
        public ProjectInstaller()
        {
            InitializeComponent();          
            this.Installers.Add(GetServiceInstaller());
            this.Installers.Add(GetServiceProcessInstaller());                            
        } 
                

        /// <summary>
        /// Get the service installer
        /// </summary>
        /// <returns></returns>
        private ServiceInstaller GetServiceInstaller()
        {
            ServiceInstaller installer = new ServiceInstaller();
            string StrServiceName = GetConfigurationValue("ServiceName");
            if (StrServiceName.Trim().Length == 0)
            {
                StrServiceName = "UDTSWinService_Instance";
            }
            string StrServiceDispName = GetConfigurationValue("ServiceDisplayName");
            if (StrServiceDispName.Trim().Length == 0)
            {
                StrServiceDispName = "UGSI Data Transformation Solution Service Instance";
            }
            installer.ServiceName = StrServiceName;
            installer.DisplayName = StrServiceDispName;
            installer.Description = StrServiceDispName;
            return installer;
        }

        /// <summary>
        /// Get the service process installer.
        /// </summary>
        /// <returns></returns>
        private ServiceProcessInstaller GetServiceProcessInstaller()
        {           
            ServiceProcessInstaller procinstaller = new ServiceProcessInstaller();
            procinstaller.Account = ServiceAccount.LocalSystem;
            return procinstaller;
        }

        /// <summary>
        /// To override Install Method
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1 = null;
            foreach (object installer in this.Installers)
                try
                {
                    serviceProcessInstaller1 = (System.ServiceProcess.ServiceProcessInstaller)
                    installer;
                }
                catch (Exception exc)
                {

                }

            if (serviceProcessInstaller1 != null)
            {
                //Get the parameter value from user either Accountid(1) or Local system(2) 
                string strInstallType = Context.Parameters["TEST"].Trim();
                serviceProcessInstaller1.Account =
                    System.ServiceProcess.ServiceAccount.LocalSystem;
                if (strInstallType.Trim() == "1")
                {
                    serviceProcessInstaller1.Account =
                    System.ServiceProcess.ServiceAccount.User;
                }
                else
                {
                    serviceProcessInstaller1.Account =
                    System.ServiceProcess.ServiceAccount.LocalSystem;
                    serviceProcessInstaller1.Username = null;
                    serviceProcessInstaller1.Password = null;

                }

            }
            base.Install(stateSaver);
        }

        
        /// <summary>
        /// Function to read the configuration items/keys.
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

       
    }
}
