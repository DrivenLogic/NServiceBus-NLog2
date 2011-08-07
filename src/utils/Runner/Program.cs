using System;
using System.Collections.Specialized;
using System.Linq;
using NServiceBus.Utils;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {

            if(!MsmqInstallation.IsInstallationGood())
            {
                Console.WriteLine("MSMQ is not configured correctly for use with NServiceBus");

                if(!args.ToList().Contains("/i"))
                {
                    Console.WriteLine("Please run with /i to reconfigure MSMQ");
                    return;
                }
            }
            MsmqInstallation.StartMsmqIfNecessary();

            DtcUtil.StartDtcIfNecessary();

            PerformanceCounterInstallation.InstallCounters();
        }
    }
}
