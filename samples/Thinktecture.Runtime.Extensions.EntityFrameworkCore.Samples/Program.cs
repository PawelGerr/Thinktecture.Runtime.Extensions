using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Thinktecture.EnumLikeClasses;
using Thinktecture.ValueObjects;

namespace Thinktecture
{
   public class Program
   {
      public static void Main()
      {
         var loggerFactory = GetLoggerFactory(out var loggingLevelSwitch);
         var logger = loggerFactory.CreateLogger<Program>();

         using var ctx = CreateContext(loggerFactory);

         InsertProduct(ctx, new Product(Guid.NewGuid(), ProductName.Create("Apple"), ProductCategory.Fruits, SpecialProductType.Special, Boundary.Create(1, 2)));

         try
         {
            loggingLevelSwitch.MinimumLevel = LogEventLevel.Fatal;
            InsertProduct(ctx, new Product(Guid.NewGuid(), ProductName.Create("Pear"), ProductCategory.Get("Invalid Category"), SpecialProductType.Special, Boundary.Create(1, 2)));
            loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;
         }
         catch (DbUpdateException)
         {
            logger.LogError("Error during persistence of invalid category.");
         }

         var products = ctx.Products.AsNoTracking().Where(p => p.Category == ProductCategory.Fruits).ToList();
         logger.LogInformation("Loaded products: {@Products}", products);
      }

      private static void InsertProduct(ProductsDbContext ctx, Product apple)
      {
         ctx.Products.Add(apple);
         ctx.SaveChanges();
      }

      private static void DeleteAllProducts(ProductsDbContext ctx)
      {
         ctx.Products.RemoveRange(ctx.Products.ToList());
         ctx.SaveChanges();
      }

      private static ProductsDbContext CreateContext(ILoggerFactory loggerFactory)
      {
         var options = new DbContextOptionsBuilder<ProductsDbContext>()
                       .UseSqlServer("Server=localhost;Database=TT-Runtime-Extensions-Demo;Integrated Security=true")
                       .UseLoggerFactory(loggerFactory)
                       .EnableSensitiveDataLogging()
                       .Options;

         var ctx = new ProductsDbContext(options);
         ctx.Database.EnsureCreated();
         DeleteAllProducts(ctx);

         return ctx;
      }

      private static ILoggerFactory GetLoggerFactory(out LoggingLevelSwitch loggingLevelSwitch)
      {
         loggingLevelSwitch = new LoggingLevelSwitch();

         var serilog = new LoggerConfiguration()
                       .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                       .Destructure.AsScalar<ProductCategory>()
                       .Destructure.AsScalar<ProductName>()
                       .MinimumLevel.ControlledBy(loggingLevelSwitch)
                       .CreateLogger();

         return new LoggerFactory()
            .AddSerilog(serilog);
      }
   }
}
