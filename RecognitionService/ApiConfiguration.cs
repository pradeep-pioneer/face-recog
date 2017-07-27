using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace RecognitionService
{
    internal class ApiConfiguration : ServiceControl
    {
        IDisposable webApplication;
        public bool Start(HostControl hostControl)
        {
            Trace.WriteLine(string.Format("{0}:\tStarting Web Interface.", DateTime.Now));
            webApplication = WebApp.Start<OwinConfiguration>(string.Format("http://localhost:{0}", 7860));
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Trace.WriteLine(string.Format("{0}:\tStopping Web Interface.", DateTime.Now));
            webApplication.Dispose();
            return true;
        }
    }
}
