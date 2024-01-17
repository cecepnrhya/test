using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mini.Repositories.Base
{
    public class BaseConfig
    {
        public IConfiguration config;
        public BaseConfig()
        {
            config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        }
    }
}
