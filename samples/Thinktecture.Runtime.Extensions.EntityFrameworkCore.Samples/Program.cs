using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Thinktecture.EnumLikeClasses;
using Thinktecture.ValueTypes;

namespace Thinktecture
{
   public class Program
   {
      public static void Main()
      {
         var loggerFactory = GetLoggerFactory();
         var logger = loggerFactory.CreateLogger<Program>();

         using var ctx = CreateContext(loggerFactory);

         InsertProduct(ctx, new Product(Guid.NewGuid(), ProductName.Create("Apple"), ProductCategory.Fruits));

         try
         {
            InsertProduct(ctx, new Product(Guid.NewGuid(), ProductName.Create("Pear"), ProductCategory.Get("Invalid Category")));
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

      private static ILoggerFactory GetLoggerFactory()
      {
         var serilog = new LoggerConfiguration()
                       .WriteTo.Console()
                       .Destructure.AsScalar<ProductCategory>()
                       .Destructure.AsScalar<ProductName>()
                       .CreateLogger();

         return new LoggerFactory()
            .AddSerilog(serilog);
      }
   }
}
