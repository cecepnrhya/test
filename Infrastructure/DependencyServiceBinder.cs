using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mini.Abstract;
using Mini.Concrete;
using Mini.Entities;
using Mini.Repositories;
using Serilog;

namespace Mini.Infrastructure
{
    public class DependencyServiceBinder
    {
        public static ServiceProvider ConfigureServices(string conString)
        {
            return new ServiceCollection()
                .AddDbContext<GofushionContext>(options => options.UseSqlite(conString))
                .AddTransient<ISqlServerRepository<Product>, SqlServerRepository>()
                .AddTransient<IMySqlRepository<Product>, MySqlRepository>()
                .AddTransient<IPsqlRepository<Product>, PsqlRepository>()
                .AddTransient<IExcelRepository<Product>, ExcelRepository>()
                .AddTransient<ICsvRepository<Product>, CsvRepository>()
                .AddTransient<IAccessRepository<Product>, AccessRepository>()
                .AddTransient<IFireBirdRepository<Product>, FireBirdRepository>()
                .AddTransient<IOdbcRepository<Product>, OdbcRepository>()
                .AddTransient<IProductStore<Product>, ProductStore>()
                .BuildServiceProvider();
        }
    }
}