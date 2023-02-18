using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Thinktecture.AspNetCore.ModelBinding;
using Thinktecture.SmartEnums;
using Thinktecture.Text.Json.Serialization;
using Thinktecture.Validation;
using Thinktecture.ValueObjects;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Thinktecture;

public class Program
{
   // ReSharper disable once InconsistentNaming
   public static async Task Main()
   {
      var startMinimalWebApi = true;

      var loggerFactory = CreateLoggerFactory();
      var app = startMinimalWebApi
                   ? StartMinimalWebApiAsync(loggerFactory)
                   : StartServerAsync(loggerFactory); // MVC controllers

      await app;

      // calls
      // 	http://localhost:5000/api/category/fruits
      // 	http://localhost:5000/api/categoryWithConverter/fruits
      // 	http://localhost:5000/api/group/1
      // 	http://localhost:5000/api/group/42
      // 	http://localhost:5000/api/groupWithConverter/1
      // 	http://localhost:5000/api/groupWithConverter/42
      // 	http://localhost:5000/api/productType/groceries
      // 	http://localhost:5000/api/productType/invalid
      // 	http://localhost:5000/api/productTypeWithJsonConverter/groceries
      // 	http://localhost:5000/api/productTypeWithJsonConverter/invalid
      // 	http://localhost:5000/api/productName/bread
      // 	http://localhost:5000/api/productName/a
      // 	http://localhost:5000/api/boundary
      await DoHttpRequestsAsync(loggerFactory.CreateLogger<Program>(), startMinimalWebApi);

      await Task.Delay(5000);
   }

   private static async Task DoHttpRequestsAsync(ILogger logger, bool forMinimalWebApi)
   {
      using var client = new HttpClient();

      await DoRequestAsync(logger, client, "category/fruits");
      await DoRequestAsync(logger, client, "categoryWithConverter/fruits");
      await DoRequestAsync(logger, client, "group/1");
      await DoRequestAsync(logger, client, "group/42");      // invalid
      await DoRequestAsync(logger, client, "group/invalid"); // invalid
      await DoRequestAsync(logger, client, "groupWithConverter/1");
      await DoRequestAsync(logger, client, "groupWithConverter/42"); // invalid
      await DoRequestAsync(logger, client, "productType/groceries");
      await DoRequestAsync(logger, client, "productType?productType=groceries");
      await DoRequestAsync(logger, client, "productType", "groceries");
      await DoRequestAsync(logger, client, "productType/invalid"); // invalid

      if (forMinimalWebApi)
         await DoRequestAsync(logger, client, "productTypeWithFilter?productType=invalid"); // invalid

      await DoRequestAsync(logger, client, "productType", "invalid");                              // invalid
      await DoRequestAsync(logger, client, "productTypeWrapper", new { ProductType = "invalid" }); // invalid
      await DoRequestAsync(logger, client, "productTypeWithJsonConverter/groceries");
      await DoRequestAsync(logger, client, "productTypeWithJsonConverter/invalid"); // invalid

      await DoRequestAsync(logger, client, "productName/bread");
      await DoRequestAsync(logger, client, "productName/a");   // invalid (Product name cannot be 1 character long.)
      await DoRequestAsync(logger, client, "productName/%20"); // product name is null
      await DoRequestAsync(logger, client, "productName", "bread");
      await DoRequestAsync(logger, client, "productName", "a"); // invalid (Product name cannot be 1 character long.)
      await DoRequestAsync(logger, client, "productName", " "); // invalid (Product name cannot be empty.)

      await DoRequestAsync(logger, client, "otherProductName/bread");
      await DoRequestAsync(logger, client, "otherProductName/a");   // invalid (Product name cannot be 1 character long.)
      await DoRequestAsync(logger, client, "otherProductName/%20"); // product name is null
      await DoRequestAsync(logger, client, "otherProductName", "bread");
      await DoRequestAsync(logger, client, "otherProductName", "a"); // invalid (Product name cannot be 1 character long.)

      await DoRequestAsync(logger, client, "boundary", BoundaryWithJsonConverter.Create(1, 2));
      await DoRequestAsync(logger, client, "boundary", jsonBody: "{ \"lower\": 2, \"upper\": 1 }");
   }

   private static async Task DoRequestAsync(ILogger logger, HttpClient client, string url, object? body = null, string? jsonBody = null)
   {
      var hasBody = body is not null || jsonBody is not null;

      if (hasBody)
      {
         logger.LogInformation("POST request with url '{Url}' and body '{Body}'", url, body ?? jsonBody);
      }
      else
      {
         logger.LogInformation("GET request with url '{Url}'", url);
      }

      var request = new HttpRequestMessage(hasBody ? HttpMethod.Post : HttpMethod.Get, "http://localhost:5000/api/" + url);

      if (body is not null)
         request.Content = JsonContent.Create(body);

      if (jsonBody is not null)
         request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

      using var response = await client.SendAsync(request);

      var content = await response.Content.ReadAsStringAsync();
      logger.LogInformation("Server responded with: [{StatusCode}] {Response}\n", response.StatusCode, content);
   }

   private static Task StartServerAsync(ILoggerFactory loggerFactory)
   {
      var webHost = new HostBuilder()
                    .ConfigureWebHostDefaults(builder =>
                                              {
                                                 builder.UseKestrel()
                                                        .Configure(app =>
                                                                   {
                                                                      app.UseRouting();
                                                                      app.UseEndpoints(endpoints => endpoints.MapControllers());
                                                                   });
                                              })
                    .ConfigureServices(collection =>
                                       {
                                          collection.AddSingleton(loggerFactory);
                                          collection.AddControllers(options => options.ModelBinderProviders.Insert(0, new ValueObjectModelBinderProvider()))
                                                    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new ValueObjectJsonConverterFactory()));
                                       })
                    .Build();

      return webHost.StartAsync();
   }

   private static Task StartMinimalWebApiAsync(ILoggerFactory loggerFactory)
   {
      var builder = WebApplication.CreateBuilder();
      builder.Services
             .AddSingleton(loggerFactory)
             .ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new ValueObjectJsonConverterFactory()));

      var app = builder.Build();

      var group = app.MapGroup("/api");

      group.MapGet("category/{category}", (ProductCategory category) => new { Value = category, category.IsValid });
      group.MapGet("categoryWithConverter/{category}", (ProductCategoryWithJsonConverter category) => new { Value = category, category.IsValid });
      group.MapGet("group/{group}", (ProductGroup group) => new { Value = group, group.IsValid });
      group.MapGet("groupWithConverter/{group}", (ProductGroupWithJsonConverter group) => new { Value = group, group.IsValid });
      group.MapGet("productType/{productType}", (ProductType productType) => productType);
      group.MapGet("productType", (ProductType productType) => productType);
      group.MapGet("productTypeWithFilter", async (BoundValueObject<ProductType> productType) => productType.Value)
           .AddEndpointFilter((context, next) =>
                              {
                                 var value = context.GetArgument<IBoundParam>(0);

                                 if (value.Error is not null)
                                    return new ValueTask<object?>(Results.BadRequest(value.Error));

                                 return next(context);
                              });
      group.MapPost("productType", ([FromBody] ProductType productType) => productType);
      group.MapPost("productTypeWrapper", ([FromBody] ProductTypeWrapper productType) => productType);
      group.MapGet("productTypeWithJsonConverter/{productType}", (ProductTypeWithJsonConverter productType) => productType);
      group.MapGet("productName/{name}", (ProductName name) => name);
      group.MapPost("productName", ([FromBody] ProductName name) => name);
      group.MapGet("otherProductName/{name}", (OtherProductName? name) => name);
      group.MapPost("otherProductName", ([FromBody] OtherProductName name) => name);
      group.MapPost("boundary", ([FromBody] BoundaryWithJsonConverter boundary) => boundary);

      return app.StartAsync();
   }

   private static ILoggerFactory CreateLoggerFactory()
   {
      var serilog = new LoggerConfiguration()
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .CreateLogger();

      var loggerFactory = new LoggerFactory();
      loggerFactory.AddSerilog(serilog);

      return loggerFactory;
   }
}
