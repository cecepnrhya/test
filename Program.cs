using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mini;
using Mini.Abstract;
using Mini.Concrete;
using Mini.Entities;
using Mini.Infrastructure;
using Mini.Mapper;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class Program : IDesignTimeDbContextFactory<GofushionContext>
{
    public static IEnumerable<Product> products;
    public static IConfiguration JsonConfig = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .Build();
    public GofushionContext CreateDbContext(string[] args)
    {
        string connectionString = JsonConfig["LocalConString"];
        DbContextOptionsBuilder<GofushionContext> optionsBuilder = new DbContextOptionsBuilder<GofushionContext>()
            .UseSqlite(connectionString);
        return new GofushionContext(optionsBuilder.Options);
    }
    static void Main(string[] args)
    {
        // ConfigureService.Configure();
        Startup.Exec();
        //Console.ReadLine();
	    Thread.Sleep(Timeout.Infinite);
    }
}
