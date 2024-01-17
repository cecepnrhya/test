using Mini.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Core.Util;
using System.Threading.Tasks;
using Mini.Abstract;
using Core.Abstract;
using Microsoft.EntityFrameworkCore;
using Mini.Infrastructure;

namespace Mini.Concrete
{
    public class ProductStore : IProductStore<Product>
    {
        private GofushionContext context;
        public ProductStore(GofushionContext Context)
        {
            this.context = Context;
        }
        public IQueryable<Product> Products => this.context.Products.Where(w => w.merchant_code == ApplicationVariable.MerchantCode);
        public async Task<IEnumerable<Product>> UpdateAsync(IEnumerable<Product> entities, bool SyncAll)
        {
            var entitiesCodes = entities.Select(s => s.product_code).ToList();
            // detach model process
            var locals = context.Set<Product>()
                        .Local
                        .Where(x => entitiesCodes.Contains(x.product_code))
                        .ToList();
            if (locals.Count > 0)
            {
                foreach (var local in locals)
                {
                    this.context.Entry(local).State = EntityState.Detached;
                }
            }
            List<Product> products = new List<Product>();
            var productExist = await this.Products.ToListAsync();

            // delete
            if (productExist.Count != 0)
            {
                IEnumerable<Product> deleteProducts = productExist.Except(entities, (c, g) => c.Id == g.Id && c.product_code == g.product_code && c.merchant_code == g.merchant_code);
                this.context.Products.RemoveRange(deleteProducts);
                IEnumerable<Product> customDeleteProducts = deleteProducts.Select(s => new Product
                {
                    product_code = s.product_code,
                    price = s.price,
                    qty = "0", // define zero for delete object confirm javear
                    merchant_code = s.merchant_code
                });
                products.AddRange(customDeleteProducts);

                // update 
                IEnumerable<Product> updateProducts = entities.Except(productExist, (c, g) =>
                    c.Id == g.Id
                    && c.product_code == g.product_code
                    && c.price == g.price
                    && c.qty == g.qty
                    && c.merchant_code == g.merchant_code
                );
                foreach (var updateproduct in updateProducts)
                {
                    var productDb = await this.context.Products.SingleOrDefaultAsync(w => w.Id == updateproduct.Id && w.product_code == updateproduct.product_code && w.merchant_code == updateproduct.merchant_code);
                    if (productDb != null)
                    {
                        productDb.price = updateproduct.price;
                        productDb.qty = updateproduct.qty;
                        products.Add(updateproduct);
                        this.context.Products.Update(productDb);
                    }
                }
            }

            // add
            IEnumerable<Product> newProducts = entities.Except(productExist, (c, g) => c.Id == g.Id 
                                                && c.product_code == g.product_code 
                                                && c.merchant_code == g.merchant_code);
            this.context.Products.AddRange(newProducts);
            products.AddRange(newProducts);
            await this.context.SaveChangesAsync();

            // Sync All in sqllite
            if (SyncAll)
            {
                return await this.Products.ToListAsync();
            }
            return products;
        }
    }
}
