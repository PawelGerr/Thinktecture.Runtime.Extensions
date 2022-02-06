using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Thinktecture.SmartEnums;
using Thinktecture.ValueObjects;

namespace Thinktecture;

public class Program
{
   public static async Task Main()
   {
      var loggingLevelSwitch = new LoggingLevelSwitch();
      var serviceProvider = CreateServiceProvider(loggingLevelSwitch);

      await InitializeAsync(serviceProvider);

      await DoDatabaseRequestsAsync(serviceProvider, loggingLevelSwitch);
   }

   private static async Task InitializeAsync(IServiceProvider serviceProvider)
   {
      await using var scope = serviceProvider.CreateAsyncScope();
      var ctx = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

      await ctx.Database.EnsureDeletedAsync();
      await ctx.Database.EnsureCreatedAsync();

      await DeleteAllProductsAsync(ctx);
   }

   private static async Task DoDatabaseRequestsAsync(IServiceProvider serviceProvider, LoggingLevelSwitch loggingLevelSwitch)
   {
      await using var scope = serviceProvider.CreateAsyncScope();

      var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
      var ctx = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

      await InsertProductAsync(ctx, new Product(Guid.NewGuid(), ProductName.Create("Apple"), ProductCategory.Fruits, SpecialProductType.Special, Boundary.Create(1, 2)));

      try
      {
         loggingLevelSwitch.MinimumLevel = LogEventLevel.Fatal;
         await InsertProductAsync(ctx, new Product(Guid.NewGuid(), ProductName.Create("Pear"), ProductCategory.Get("Invalid Category"), SpecialProductType.Special, Boundary.Create(1, 2)));
         loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;
      }
      catch (DbUpdateException)
      {
         logger.LogError("Error during persistence of invalid category");
      }

      var products = await ctx.Products.AsNoTracking().Where(p => p.Category == ProductCategory.Fruits).ToListAsync();
      logger.LogInformation("Loaded products: {@Products}", products);
   }

   private static async Task InsertProductAsync(ProductsDbContext ctx, Product apple)
   {
      ctx.Products.Add(apple);
      await ctx.SaveChangesAsync();
   }

   private static async Task DeleteAllProductsAsync(ProductsDbContext ctx)
   {
      ctx.Products.RemoveRange(ctx.Products.ToList());
      await ctx.SaveChangesAsync();
   }

   private static IServiceProvider CreateServiceProvider(LoggingLevelSwitch loggingLevelSwitch)
   {
      return new ServiceCollection()
             .AddLogging(builder =>
                         {
                            var serilogLogger = new LoggerConfiguration()
                                                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                                                .Destructure.AsScalar<ProductCategory>()
                                                .Destructure.AsScalar<ProductName>()
                                                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                                                .CreateLogger();

                            builder.AddSerilog(serilogLogger);
                         })
             .AddDbContext<ProductsDbContext>(builder => builder.UseSqlServer("Server=localhost;Database=TT-Runtime-Extensions-Demo;Integrated Security=true")
                                                                .EnableSensitiveDataLogging()
                                                                .UseValueObjectValueConverter(configureEnumsAndKeyedValueObjects: property =>
                                                                                                                                  {
                                                                                                                                     if (property.ClrType == typeof(SpecialProductType))
                                                                                                                                     {
                                                                                                                                        var maxLength = SpecialProductType.Items.Max(i => i.Key.Length);
                                                                                                                                        property.SetMaxLength(RoundUp(maxLength));
                                                                                                                                     }
                                                                                                                                     else if (property.ClrType == typeof(ProductCategory))
                                                                                                                                     {
                                                                                                                                        var maxLength = ProductCategory.Items.Max(i => i.Name.Length);
                                                                                                                                        property.SetMaxLength(RoundUp(maxLength));
                                                                                                                                     }
                                                                                                                                     else if (property.ClrType == typeof(ProductName))
                                                                                                                                     {
                                                                                                                                        property.SetMaxLength(200);
                                                                                                                                     }
                                                                                                                                  }))
             .BuildServiceProvider();
   }

   private static int RoundUp(int value)
   {
      return value + (10 - value % 10);
   }
}
