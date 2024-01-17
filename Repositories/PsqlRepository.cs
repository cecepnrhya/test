using Dapper;
using Mini.Abstract;
using Mini.Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Mini.Repositories
{
    public class PsqlRepository : IPsqlRepository<Product>
    {
        private IDbConnection conPsql;
        private IProductStore<Product> _productStore;
        #region Constructor
        public PsqlRepository(IProductStore<Product> productStore)
        {
            this._productStore = productStore;
        }
        #endregion
        public IDbConnection GetConPsql(string conString)
        {
            conPsql = new NpgsqlConnection(conString);
            return conPsql;
        }
        public async Task<IEnumerable<Product>> Get(string conString,string Query, bool SyncAll)
        {
            try
            {
                conPsql = GetConPsql(conString);
                var products = await conPsql.QueryAsync<Product>(Query, commandTimeout: 600);
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
