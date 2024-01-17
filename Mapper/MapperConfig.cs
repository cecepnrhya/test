using Microsoft.Extensions.DependencyInjection;
using Mini.Abstract;
using Mini.Entities;
using Mini.Repositories;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Mapper
{
    public class MapperConfig
    {
        private static readonly HttpClient client = new HttpClient();
        public static Config ToConfig(string Config)
        {
            Config config = new Config();
            try
            {
                dynamic objConfig = JsonConvert.DeserializeObject(Config);
                string configString = objConfig.data.configuration_text;
                dynamic decodeConfig = JsonConvert.DeserializeObject(
                    ASCIIEncoding.ASCII.GetString(
                        Convert.FromBase64String(configString)));
                config.Engine = decodeConfig.engine;
                config.ConString = decodeConfig.connectionstring;
                config.Query = decodeConfig.query;
                config.DashEndpoint = decodeConfig.dashendpoint;
                config.GoaEndpoint = decodeConfig.goaendpoint;
                config.Path = decodeConfig.path;
                config.SheetName = decodeConfig.sheetname;
                config.HeaderCode = decodeConfig.headercode;
                config.HeaderName = decodeConfig.headername;
                config.HeaderQty = decodeConfig.headerqty;
                config.HeaderPrice = decodeConfig.headerprice;
                config.Delimiter = decodeConfig.delimiter;
                config.with2digit = decodeConfig.with2digit;
                config.QtyDigit = decodeConfig.qtydigit;
            }
            catch (Exception)
            {
                throw;
            }
            return config;
        }
    }
}