using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Thinktecture.Runtime.Extensions.Samples.EnumLikeClass;

namespace Thinktecture
{
   public class Program
   {
      public static void Main()
      {
         var loggerFactory = GetLoggerFactory();
         var logger = loggerFactory.CreateLogger<Program>();

         using (var ctx = CreateContext(loggerFactory))
         {
            InsertProduct(ctx, new Product
                               {
                                  Id = Guid.NewGuid(),
                                  Name = "Apple",
                                  Category = ProductCategory.Fruits
                               });

            InsertProduct(ctx, new Product
                               {
                                  Id = Guid.NewGuid(),
                                  Name = "Pear",
                                  Category = ProductCategory.Get("Invalid Category")
                               });

            var products = ctx.Products.Where(p => p.Category == ProductCategory.Fruits).ToList();
            logger.LogInformation("Loaded products: {@products}", products);
         }
      }

      private static void InsertProduct([NotNull] ProductsDbContext ctx, [NotNull] Product apple)
      {
         ctx.Products.Add(apple);
         ctx.SaveChanges();
      }

      private static void DeleteAllProducts([NotNull] ProductsDbContext ctx)
      {
         ctx.Products.RemoveRange(ctx.Products.ToList());
         ctx.SaveChanges();
      }

      [NotNull]
      private static ProductsDbContext CreateContext(ILoggerFactory loggerFactory)
      {
         var options = new DbContextOptionsBuilder<ProductsDbContext>()
                       .UseSqlServer("Server=localhost;Database=TT-Runtime-Extensions-Demo;Integrated Security=true")
                       .UseLoggerFactory(loggerFactory)
                       .AddEnumSupport()
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
                       .CreateLogger();

         return new LoggerFactory()
            .AddSerilog(serilog);
      }
   }
}