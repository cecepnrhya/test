using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using Topshelf;

namespace Mini
{
    internal static class ConfigureService
    {
        internal static void Configure()
        {
            HostFactory.Run(configure =>
            {
                configure.Service<GofushionService>(service =>
                {
                    service.ConstructUsing(s => new GofushionService());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                //Setup Account that window service use to run.  
                configure.RunAsLocalSystem();
                configure.SetServiceName("Gofushion");
                configure.SetDisplayName("Gofushion");
                configure.SetDescription("Gofushion Service");
                configure.OnException(ex => Log.Fatal(ex.Message));
                configure.StartAutomaticallyDelayed();
                configure.EnableServiceRecovery(recoveryOption =>
                {
                    recoveryOption.RestartService(0);
                });

            });
        }
    }
}
