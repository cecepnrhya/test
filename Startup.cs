using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mini.Abstract;
using Mini.Concrete;
using Mini.Entities;
using Mini.Infrastructure;
using Mini.Mapper;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Mini.Infrastructure.Helper;

namespace Mini
{
    public class Startup
    {
        public static IEnumerable<Product> products;
        public static IConfiguration JsonConfig = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
         .Build();
        public static void Exec()
        {
            // remove logs JsonConfig["LogFile"]
            try
            {
                // Check if file exists with its full path    
                if (File.Exists(JsonConfig["LogFile"]))
                {
                    // If file found, delete it    
                    File.Delete(JsonConfig["LogFile"]);
                }
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }
            // setup log
            Log.Logger = new LoggerConfiguration()
            .WriteTo
            .File(JsonConfig["LogFile"])
            .CreateLogger();

            // Define Database from migrations history
            Program p = new Program();
            using (GofushionContext gofuContext = p.CreateDbContext(null))
            {
                gofuContext.Database.Migrate();
            }
            // setup our DI
            var serviceProvider = DependencyServiceBinder.ConfigureServices(JsonConfig["LocalConString"]);

            // firestore setup
            string dir = Directory.GetCurrentDirectory();
            string CredentialPath = dir + "/" + JsonConfig["GoogleCredentialFileName"];
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", CredentialPath);
            FirestoreDb FireStore = FirestoreDb.Create(JsonConfig["FirestoreDB"]);

            // firestore integration
            DocumentReference DocBulk = FireStore
                .Collection(JsonConfig["FirestoreListen"])
                .Document(JsonConfig["DocID"]);
            Console.WriteLine("Ready to listen");
            // firestore listen process
            FirestoreChangeListener ListenerBulk = DocBulk.Listen(async snapshot =>
            {
                Console.WriteLine("going to try catch");
                //do the actual work here. and use switch case and try catch
                try
                {
                    Console.WriteLine("Starting listen");
                    var subMerchants = JsonConfig.GetSection("SubMerchant").Get<string[]>();
                    if (subMerchants != null)
                    {
                        foreach (var subMerchant in subMerchants)
                        {
                            ApplicationVariable.MerchantCode = subMerchant;
                            Console.WriteLine("Listening Process " + subMerchant);
                            bool SyncAll = false;
                            int i = 0;
                            string url = JsonConfig["Endpoint"] + subMerchant;
                            string config = await ApiCall.EndPointCall(url);
                            Config configModel = MapperConfig.ToConfig(config);
                            if (snapshot.Exists)
                            {
                                try
                                {
                                    SyncAll = snapshot.GetValue<Boolean>("SyncAll");
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex.Message);
                                    SyncAll = false;
                                }

                                switch (configModel.Engine)
                                {
                                    case "SQLSERVER":
                                        var sqlRepo = serviceProvider.GetService<ISqlServerRepository<Product>>();
                                        products = ValidateProduct(await sqlRepo.Get(configModel.ConString, configModel.Query, SyncAll));
                                        i = await ProductPost.ProductPostAsync(configModel.GoaEndpoint, products);
                                        break;
                                    case "MYSQL":
                                        var mysqlRepo = serviceProvider.GetService<IMySqlRepository<Product>>();
                                        products = ValidateProduct(await mysqlRepo.Get(configModel.ConString, configModel.Query, SyncAll));
                                        i = await ProductPost.ProductPostAsync(configModel.GoaEndpoint, products);
                                        break;
                                    case "PSQL":
                                        var psqlRepo = serviceProvider.GetService<IPsqlRepository<Product>>();
                                        products = ValidateProduct(await psqlRepo.Get(configModel.ConString, configModel.Query, SyncAll));
                                        i = await ProductPost.ProductPostAsync(configModel.GoaEndpoint, products);
                                        break;
                                    case "EXCEL":
                                        Console.WriteLine("Prepare to retrive data from file "+ ApplicationVariable.MerchantCode);
                                        var excelRepo = serviceProvider.GetService<IExcelRepository<Product>>();
                                        await excelRepo.Get(configModel.Path, configModel.HeaderCode, configModel.HeaderName, configModel.HeaderQty, configModel.HeaderPrice, SyncAll);
                                        // Console.WriteLine(products);
                                        // products = ValidateProduct(await excelRepo.Get(configModel.Path, configModel.HeaderCode, configModel.HeaderName, configModel.HeaderQty, configModel.HeaderPrice, SyncAll));
                                        // i = await ProductPost.ProductPostAsync(configModel.GoaEndpoint, products);
                                        // Helpers.RenameFile(configModel.Path);
                                        break;
                                    case "CSV":
                                        var csvRepo = serviceProvider.GetService<ICsvRepository<Product>>();
                                        products = ValidateProduct(await csvRepo.Get(configModel.Path, configModel.HeaderCode, configModel.HeaderName, configModel.HeaderQty, configModel.HeaderPrice, SyncAll, configModel.Delimiter));
                                        i = await ProductPost.ProductPostAsync(configModel.GoaEndpoint, products);
                                        Helpers.RenameFile(configModel.Path);
                                        break;
                                    case "ACCESS":
                                        var accessRepo = serviceProvider.GetService<IAccessRepository<Product>>();
                                        products = ValidateProduct(await accessRepo.Get(configModel.ConString, configModel.Query, SyncAll));
                                        i = await ProductPost.ProductPostAsync(configModel.GoaEndpoint, products);
                                        break;
                                    case "ODBC":
                                        var odbcRepo = serviceProvider.GetService<IOdbcRepository<Product>>();
                                        products = ValidateProduct(await odbcRepo.Get(configModel.ConString, configModel.Query, SyncAll));
                                        i = await ProductPost.ProductPostAsync(configModel.GoaEndpoint, products);
                                        break;
                                    case "FIREBIRD":
                                        var fireRepo = serviceProvider.GetService<IFireBirdRepository<Product>>();
                                        products = ValidateProduct(await fireRepo.Get(configModel.ConString, configModel.Query, SyncAll));
                                        i = await ProductPost.ProductPostAsync(configModel.GoaEndpoint, products);
                                        break;
                                    default:
                                        break;
                                }
                                Log.Information($"===================== Sync at {DateTime.Now.ToString()} =======================");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message);
                    Log.Fatal(ex.Message);
                }
            });
        }
        public static IEnumerable<Product> ValidateProduct(IEnumerable<Product> products)
        {
            return products = products.Where(w => !string.IsNullOrWhiteSpace(w.product_code) || !string.IsNullOrWhiteSpace(w.qty) || !string.IsNullOrWhiteSpace(w.price)).ToList();
        }
    }
}
