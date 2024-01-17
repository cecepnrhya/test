using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Mini.Abstract;
using Mini.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Repositories
{
    public class FireBirdRepository : IFireBirdRepository<Product>
    {
        private IDbConnection conFire;
        private IProductStore<Product> _productStore;
        #region Constructor
        public FireBirdRepository(IProductStore<Product> productStore)
        {
            this._productStore = productStore;
        }
        #endregion
        public IDbConnection GetConFire(string conString)
        {
            conFire = new FbConnection(conString);
            return conFire;
        }
        public async Task<IEnumerable<Product>> Get(string ConString, string Query, bool SyncAll)
        {
            try
            {
                conFire = GetConFire(ConString);
                var products = await conFire.QueryAsync<Product>(Query, commandTimeout: 600);
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
