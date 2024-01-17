using Mini.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mini.Infrastructure
{
    public class ApiCall
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<string> EndPointCall(string url)
        {
            var configs = await client.GetStringAsync(url);
            return configs;
        }
        public static async Task<HttpResponseMessage> Post(string url, HttpContent content) {
            var result = await client.PostAsync(url, content);
            return result;
        }
    }
}