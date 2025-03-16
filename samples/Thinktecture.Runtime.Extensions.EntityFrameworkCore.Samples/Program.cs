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
   // ReSharper disable once InconsistentNaming
   public static async Task Main()
   {
      var loggingLevelSwitch = new LoggingLevelSwitch();
      var serviceProvider = CreateServiceProvider(loggingLevelSwitch);

      await InitializeAsync(serviceProvider);

      await DoProductDemosAsync(serviceProvider, loggingLevelSwitch);
      await DoMessagesDemosAsync(serviceProvider, loggingLevelSwitch);
   }

   private static async Task InitializeAsync(IServiceProvider serviceProvider)
   {
      await using var scope = serviceProvider.CreateAsyncScope();
      var ctx = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

      await ctx.Database.EnsureDeletedAsync();
      await ctx.Database.EnsureCreatedAsync();
   }

   private static async Task DoProductDemosAsync(IServiceProvider serviceProvider, LoggingLevelSwitch loggingLevelSwitch)
   {
      await using var scope = serviceProvider.CreateAsyncScope();

      var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
      var ctx = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

      logger.LogInformation("""


                            ==== Demo for Products ====

                            """);

      var today = OpenEndDate.Create(DateTime.Today);

      await InsertProductAsync(
         ctx,
         new Product(
            Guid.NewGuid(),
            ProductName.Create("Apple"),
            ProductCategory.Fruits,
            ProductType.Groceries,
            DayMonth.Create(1, 15),
            Boundary.Create(1, 2), today));

      try
      {
         // provoke exception by providing invalid ProductCategory
         loggingLevelSwitch.MinimumLevel = LogEventLevel.Fatal;
         await InsertProductAsync(
            ctx,
            new Product(
               Guid.NewGuid(),
               ProductName.Create("Pear"),
               ProductCategory.Get("Invalid Category"),
               ProductType.Groceries,
               DayMonth.Create(1, 15),
               Boundary.Create(1, 2)));
      }
      catch (DbUpdateException)
      {
         logger.LogError("Error during persistence of invalid category");
      }
      finally
      {
         ctx.ChangeTracker.Clear(); // remove invalid product from the EF context
         loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;
      }

      var products = await ctx.Products.AsNoTracking().Where(p => p.Category == ProductCategory.Fruits).ToListAsync();
      logger.LogInformation("Loaded products: {@Products}", products);

      var deliveryDate = DayMonth.Create(1, 15);
      products = await ctx.Products.AsNoTracking().Where(p => p.ScheduledDeliveryDate == deliveryDate).ToListAsync();
      logger.LogInformation("Loaded products: {@Products}", products);

      // returns the product "Apple" because "EndDate" equals "today" and (today >= today) evaluates to true
      products = await ctx.Products.AsNoTracking().Where(p => p.EndDate >= today).ToListAsync();
      logger.LogInformation("Products with End Data >= Today: {@Products}", products);

      // returns nothing because "EndDate" equals "today" and (today > today) evaluates to false
      products = await ctx.Products.AsNoTracking().Where(p => p.EndDate > today).ToListAsync();
      logger.LogInformation("Products with End Data > Today: {@Products}", products);

      var product = ctx.Products.Single();
      product.EndDate = OpenEndDate.Infinite;
      await ctx.SaveChangesAsync();

      // same query as the previous one but now "EndDate" equals "infinite" and (infinite > today) evaluates to true
      products = await ctx.Products.AsNoTracking().Where(p => p.EndDate > today).ToListAsync();
      logger.LogInformation("(After changing EndDate) Products with End Data > Today: {@Products}", products);
   }

   private static async Task InsertProductAsync(ProductsDbContext ctx, Product apple)
   {
      ctx.Products.Add(apple);
      await ctx.SaveChangesAsync();
   }

   private static async Task DoMessagesDemosAsync(IServiceProvider serviceProvider, LoggingLevelSwitch loggingLevelSwitch)
   {
      await using var scope = serviceProvider.CreateAsyncScope();

      var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
      var ctx = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

      logger.LogInformation("""


                            ==== Demo for Message States ====

                            """);

      var message = new Message
                    {
                       Id = Guid.NewGuid(),
                       States = [new MessageState.Initial()]
                    };
      ctx.Messages.Add(message);
      await ctx.SaveChangesAsync();

      ctx.ChangeTracker.Clear(); // remove message from the EF context

      // Message states: [{"Order": 1, "$type": "Initial"}]
      message = await ctx.Messages.SingleAsync();
      logger.LogInformation("Message states: {@States}", message.States);

      // Trasition to "Parsed" state
      message.States.Add(new MessageState.Parsed(DateTime.Now));
      await ctx.SaveChangesAsync();

      ctx.ChangeTracker.Clear(); // remove message from the EF context

      // Message states: [{"Order": 1, "$type": "Initial"}, {"CreatedAt": "2025-03-16T10:53:35.7020302", "Order": 2, "$type": "Parsed"}]
      message = await ctx.Messages.SingleAsync();
      logger.LogInformation("Message states: {@States}", message.States);

      var currentState = message.States.OrderBy(s => s.Order).Last();

      // Parsed (at 03/16/2025 10:52:35)
      currentState.Switch(
         initial: _ => logger.LogInformation("Initial state"),
         parsed: s => logger.LogInformation("Parsed (at {ParsedAt})", s.CreatedAt),
         processed: s => logger.LogInformation("Processed (at {ProcessedAt})", s.CreatedAt),
         error: s => logger.LogInformation("Error: {Error}", s.Message)
      );
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
             .AddDbContext<ProductsDbContext>(builder => builder.UseSqlServer("Server=localhost;Database=TT-Runtime-Extensions-Demo;Integrated Security=true;TrustServerCertificate=true")
                                                                .EnableSensitiveDataLogging()
                                                                .UseValueObjectValueConverter(configureEnumsAndKeyedValueObjects: property =>
                                                                {
                                                                   if (property.ClrType == typeof(ProductType))
                                                                   {
                                                                      var maxLength = ProductType.Items.Max(i => i.Key.Length);
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
