using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace RecognitionService
{
    class Program
    {
        static int Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                //Load 
                x.SetStartTimeout(TimeSpan.FromSeconds(2));
                x.SetStopTimeout(TimeSpan.FromSeconds(10));
                x.Service<ApiConfiguration>();
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetDescription("Hard Coded Recognition Service");
                x.SetDisplayName("face recognition");
                x.SetServiceName("face-recognizer");
                Console.WriteLine("Server is now running on http://localhost:{0}", 7860);
            });
            return (int)exitCode;
        }
    }
}
