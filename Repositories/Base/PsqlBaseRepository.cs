using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Mini.Repositories.Base
{
    public class PsqlBaseRepository : BaseConfig, IDisposable
    {
        protected readonly IDbConnection conPsql;
        public PsqlBaseRepository()
        {
            conPsql = new NpgsqlConnection(config["ConnectionStrings:POSTGRESCONNECTION"]);
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
