using Mini.Abstract;
using Mini.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using OfficeOpenXml;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using Mini.Infrastructure;

namespace Mini.Repositories
{
    public class ExcelRepository : IExcelRepository<Product>
    {
        private IProductStore<Product> _productStore;
        #region Constructor
        public ExcelRepository(IProductStore<Product> productStore)
        {
            this._productStore = productStore;
        }
        #endregion
        public async Task<IEnumerable<Product>> Get(string Path, string HeaderCode, string HeaderName, string HeaderQty, string HeaderPrice, bool SyncAll)
        {
            Console.WriteLine("Get Data " + ApplicationVariable.MerchantCode);
            var products = GetProductsFromExcel(Path, HeaderCode, HeaderName, HeaderQty, HeaderPrice);
            Console.WriteLine(products);
            products = products.Where(w => !string.IsNullOrWhiteSpace(w.product_code) || !string.IsNullOrWhiteSpace(w.qty) || !string.IsNullOrWhiteSpace(w.price)).ToList();
            return await this._productStore.UpdateAsync(products, SyncAll);
        }
        private IEnumerable<Product> GetProductsFromExcel(string path, string HeaderCode, string HeaderName, string HeaderQty, string HeaderPrice)
        {
            List<Product> products = new List<Product>();
            string sFileExtension = Path.GetExtension(path).ToLower();
            ISheet sheet;
            using (var stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                stream.Position = 0;
                if (sFileExtension == ".xls")
                {
                    HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                    sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                }
                else
                {
                    XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                    sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                }
            }

            // Set cell Number HeaderRow
            IRow headerRow = sheet.GetRow(0);
            for (int index = headerRow.FirstCellNum; index < headerRow.LastCellNum; index++)
            {
                String cellValue = (headerRow.GetCell(index).StringCellValue.Trim());
                if (cellValue == HeaderCode) HeaderCode = index.ToString();
                if (cellValue == HeaderName) HeaderName = index.ToString();
                if (cellValue == HeaderQty) HeaderQty = index.ToString();
                if (cellValue == HeaderPrice) HeaderPrice = index.ToString();
            }

            for (int index = (sheet.FirstRowNum + 1); index <= sheet.LastRowNum; index++) //Read Excel File init row seq
            {
                IRow row = sheet.GetRow(index);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue; // init column seq and mapping it
                Product product = new Product
                {
                    product_code = row.GetCell(Convert.ToInt32(HeaderCode)).ToString().Trim(),
                    product_name = row.GetCell(Convert.ToInt32(HeaderName)).ToString().Trim(),
                    qty = row.GetCell(Convert.ToInt32(HeaderQty)).ToString().Trim(),
                    price = row.GetCell(Convert.ToInt32(HeaderPrice)).ToString().Trim(),
                    merchant_code = ApplicationVariable.MerchantCode,
                };
                if (!string.IsNullOrEmpty(product.product_code) && !string.IsNullOrEmpty(product.product_name) && !string.IsNullOrEmpty(product.qty) && !string.IsNullOrEmpty(product.price))
                {
                    products.Add(product);
                }
            }
            return products.AsEnumerable();
        }
        /// <summary>
        /// Read Excel using EPPlus. this function for read data from xlsx only. not support for .xls
        /// </summary>
        /// <param name="path"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        private DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
        {
            using (var pck = new ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
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
