using Mini.Abstract;
using Mini.Entities;
using Mini.Infrastructure;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Repositories
{
    public class CsvRepository : ICsvRepository<Product>
    {
        private IProductStore<Product> _productStore;
        #region Constructor
        public CsvRepository(IProductStore<Product> productStore)
        {
            this._productStore = productStore;
        }
        #endregion
        public async Task<IEnumerable<Product>> Get(string Path, string HeaderCode, string HeaderName, string HeaderQty, string HeaderPrice, bool SyncAll, string Delimieter)
        {
            var DataProducts = GetDataTableFromCsv(Path, Delimieter);
            IEnumerable<Product> products = DataProducts.AsEnumerable().Select(x => new Product
            {
                product_code = (string)(x[HeaderCode]),
                product_name = (string)(x[HeaderName]),
                qty = (string)(x[HeaderQty]),
                price = (string)(x[HeaderPrice]),
                merchant_code = ApplicationVariable.MerchantCode
            }).ToList();
            products = products.Where(w => !string.IsNullOrWhiteSpace(w.product_code)|| !string.IsNullOrWhiteSpace(w.qty) || !string.IsNullOrWhiteSpace(w.price)).ToList();
            return await this._productStore.UpdateAsync(products, SyncAll);
        }
        private DataTable GetDataTableFromCsv(string path, string delimiter, bool hasHeader = true)
        {
            using (var pck = new ExcelPackage())
            {
                ExcelWorksheet ws = null;
                ws = pck.Workbook.Worksheets.Add("Sheet1");
                ExcelTextFormat format = new ExcelTextFormat()
                {
                    Delimiter = delimiter.ToCharArray()[0]
                };
                ws.Cells[1, 1].LoadFromText(File.ReadAllText(path), format);
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }
    }
}
