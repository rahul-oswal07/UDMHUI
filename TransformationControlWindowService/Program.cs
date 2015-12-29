using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace UNISYS.UDTS.WinService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new UDTS_WinService() 
            };
            ServiceBase.Run(ServicesToRun);

            //Uncomment below code to debug the service and comment the above code
            //#if(!DEBUG)
            //            {

            //                ServiceBase[] ServicesToRun;
            //                ServicesToRun = new ServiceBase[] 
            //            { 
            //                new UDTS_WinService() 
            //            };
            //                ServiceBase.Run(ServicesToRun);
            //            }
            //#else
            //            {
            //                UDTS_WinService myServ = new UDTS_WinService();
            //                myServ.ServiceTask();
            //            }
            //#endif
            
        }
    }
}
