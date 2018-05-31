using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Thinktecture.Runtime.Extensions.Samples.EnumLikeClass;

namespace Thinktecture.Runtime.Extensions.EntityFrameworkSamples
{
	public class Program
	{
		public static void Main()
		{
			var logger = GetLogger();

			using (var ctx = CreateContext())
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

				var products = ctx.Products.ToList();
				logger.Information("Loaded products: {@products}", products);
			}

			logger.Information("Press ENTER to exit.");
			Console.ReadLine();
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

		private static ProductsDbContext CreateContext()
		{
			var options = new DbContextOptionsBuilder<ProductsDbContext>()
			              .UseSqlite("Data source=./products.db")
			              .Options;

			var ctx = new ProductsDbContext(options);
			ctx.Database.EnsureCreated();
			DeleteAllProducts(ctx);

			return ctx;
		}

		private static ILogger GetLogger()
		{
			return new LoggerConfiguration()
			       .WriteTo.Console()
			       .Destructure.AsScalar<ProductCategory>()
			       .CreateLogger();
		}
	}
}
