using Dapper;
using Mini.Abstract;
using Mini.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Repositories
{
    public class SqlServerRepository : ISqlServerRepository<Product>
    {
        private IDbConnection conSql;
        private IProductStore<Product> _productStore;
        #region Constructor
        public SqlServerRepository(IProductStore<Product> productStore)
        {
            this._productStore = productStore;
        }
        public IDbConnection GetConSql(string conString)
        {
            conSql = new SqlConnection(conString);
            return conSql;
        }
        #endregion
        public async Task<IEnumerable<Product>> Get(string conString, string Query, bool SyncAll)
        {
            try
            {
                conSql = GetConSql(conString);
                var products = await conSql.QueryAsync<Product>(Query, commandTimeout: 600);
                products = products.Where(w => !string.IsNullOrWhiteSpace(w.product_code) || !string.IsNullOrWhiteSpace(w.qty) || !string.IsNullOrWhiteSpace(w.price)).ToList();
                return await this._productStore.UpdateAsync(products, SyncAll);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }    
    }
}
