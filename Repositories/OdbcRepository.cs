using Dapper;
using Mini.Abstract;
using Mini.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Repositories
{
    public class OdbcRepository : IOdbcRepository<Product>
    {
        private IDbConnection conOdbc;
        private IProductStore<Product> _productStore;
        #region Constructor
        public OdbcRepository(IProductStore<Product> productStore)
        {
            this._productStore = productStore;
        }
        #endregion
        public IDbConnection GetConOdbc(string conString)
        {
            conOdbc = new OdbcConnection(conString);
            return conOdbc;
        }
        public async Task<IEnumerable<Product>> Get(string ConString, string Query, bool SyncAll)
        {
            try
            {
                conOdbc = GetConOdbc(ConString);
                var products = await conOdbc.QueryAsync<Product>(Query, commandTimeout: 600);
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
