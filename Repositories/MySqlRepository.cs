using Dapper;
using Mini.Abstract;
using Mini.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Mini.Repositories
{
    public class MySqlRepository : IMySqlRepository<Product>
    {
        private IDbConnection conMysql;
        private IProductStore<Product> _productStore;
        #region Constructor
        public MySqlRepository(IProductStore<Product> productStore)
        {
            this._productStore = productStore;
        }
        #endregion
        public IDbConnection GetConMysql(string conString)
        {
            conMysql = new MySqlConnection(conString);
            return conMysql;
        }
        public async Task<IEnumerable<Product>> Get(string conString, string Query, bool SyncAll)
        {
            try
            {
                conMysql = GetConMysql(conString);
                var products = await conMysql.QueryAsync<Product>(Query, commandTimeout:600);
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
