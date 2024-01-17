using Mini.Entities;
using Mini.Infrastructure;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Concrete
{
    public class ProductPost
    {
        public static async Task<int> ProductPostAsync(string url, IEnumerable<Product> products)
        {
            Console.WriteLine("POST DATA");
            int i = 0;
            if (products.Count() != 0)
            {
                products = products.Select(s => new Product
                {
                    product_code = s.product_code,
                    product_name = s.product_name,
                    price = CustomParse(s.price).ToString(),
                    qty = CustomParse(s.qty).ToString(),
                });
                // convert to json format
                var prods = new DataProduct { products = products };
                var payload = JsonConvert.SerializeObject(prods);
                var response = await ApiCall.Post(url, new StringContent(payload, Encoding.UTF8, "application/json"));
                Log.Information(await response.RequestMessage.Content.ReadAsStringAsync());
                if (response.IsSuccessStatusCode == false)
                {
                    i = -1;
                    Log.Error("Post data to goapotik failed. please make sure that endpoint work normally");
                }
                else
                {
                    Log.Information(await response.Content.ReadAsStringAsync());
                }
            }
            return i;
        }
        public static decimal? CustomParse(string incomingValue)
        {
            decimal val;
            double devide = 10;
            if (incomingValue.Contains('.') || incomingValue.Contains(','))
            {
                if (incomingValue.Contains('.'))
                {
                    var pow = incomingValue.Length - incomingValue.IndexOf(".") - 1;
                    devide = Math.Pow(devide, pow);
                }
                else
                {
                    var pow = incomingValue.Length - incomingValue.IndexOf(",") - 1;
                    devide = Math.Pow(devide, pow);
                }

                if (!decimal.TryParse(incomingValue.Replace(",", "").Replace(".", ""), NumberStyles.Number, CultureInfo.InvariantCulture, out val))
                    return null;
                if (devide > 10)
                    return val / Convert.ToDecimal(devide);
                else
                    return val;
            }
            else
            {
                return decimal.Parse(incomingValue);
            }
        }
    }
}
