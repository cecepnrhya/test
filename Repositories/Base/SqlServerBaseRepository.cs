using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Mini.Repositories.Base
{
    public class SqlServerBaseRepository : BaseConfig, IDisposable
    {
        protected readonly IDbConnection conSqlServer;
        public SqlServerBaseRepository()
        {
            conSqlServer = new SqlConnection(config["ConnectionStrings:SQLSERVERCONNECTION"]);
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
