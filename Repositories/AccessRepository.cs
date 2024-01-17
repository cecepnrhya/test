using Mini.Abstract;
using Mini.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using Dapper;
using System.Linq;

namespace Mini.Repositories
{
    public class AccessRepository : IAccessRepository<Product>
    {
        private IDbConnection conAccess;
        private IProductStore<Product> _productStore;
        #region Constructor
        public AccessRepository(IProductStore<Product> productStore)
        {
            this._productStore = productStore;
        }
        #endregion
        public IDbConnection GetConAccess(string conString)
        {
            conAccess = new OdbcConnection(conString);
            return conAccess;
        }
        public async Task<IEnumerable<Product>> Get(string ConString, string Query, bool SyncAll)
        {
            try
            {
                conAccess = GetConAccess(ConString);
                var products = await conAccess.QueryAsync<Product>(Query, commandTimeout: 600);
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
