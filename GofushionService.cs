using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mini
{
    public class GofushionService
    {
        public void Start()
        {
            Log.Information("Service started");
            Console.WriteLine("Service started");
            Log.CloseAndFlush();
            Startup.Exec();
        }
        public void Stop()
        {
            Log.Information("Service stopped");
            Log.CloseAndFlush();
        }
    }
}
