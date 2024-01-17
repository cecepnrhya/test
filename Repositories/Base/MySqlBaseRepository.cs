using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Mini.Repositories.Base;

namespace Mini.Repositories.Base
{
    public class MySqlBaseRepository : BaseConfig, IDisposable
    {
        protected readonly IDbConnection conMysql;
        public MySqlBaseRepository()
        {
            conMysql = new MySqlConnection(config["ConnectionStrings:MYSQLCONNECTION"]);    
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
