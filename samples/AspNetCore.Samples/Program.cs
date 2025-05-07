using System;
using System.Linq;
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
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Thinktecture.AspNetCore.ModelBinding;
using Thinktecture.Controllers;
using Thinktecture.Helpers;
using Thinktecture.SmartEnums;
using Thinktecture.Swashbuckle;
using Thinktecture.Text.Json.Serialization;
using Thinktecture.Unions;
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

      await DoHttpRequestsAsync(loggerFactory.CreateLogger<Program>(), startMinimalWebApi);

      await Task.Delay(60000);
   }

   private static async Task DoHttpRequestsAsync(ILogger logger, bool forMinimalWebApi)
   {
      using var client = new HttpClient();

      await DoRequestAsync(logger, client, "category/fruits");
      await DoRequestAsync(logger, client, "group/1");
      await DoRequestAsync(logger, client, "group/42");      // invalid
      await DoRequestAsync(logger, client, "group/invalid"); // invalid
      await DoRequestAsync(logger, client, "productType/groceries");
      await DoRequestAsync(logger, client, "productType?productType=groceries");
      await DoRequestAsync(logger, client, "productType", "groceries");
      await DoRequestAsync(logger, client, "productType/invalid");           // invalid
      await DoRequestAsync(logger, client, "boundaryWithFactories/1:2");     // uses custom factory "[ObjectFactory<string>]"
      await DoRequestAsync(logger, client, "boundaryWithFactories/invalid"); // invalid

      if (forMinimalWebApi)
         await DoRequestAsync(logger, client, "productTypeWithFilter?productType=invalid"); // invalid

      await DoRequestAsync(logger, client, "productType", "invalid");                              // invalid
      await DoRequestAsync(logger, client, "productTypeWrapper", new { ProductType = "invalid" }); // invalid

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

      await DoRequestAsync(logger, client, "boundary", Boundary.Create(1, 2));
      await DoRequestAsync(logger, client, "boundary", jsonBody: "{ \"lower\": 2, \"upper\": 1 }");

      await DoRequestAsync(logger, client, $"enddate/{DateOnly.FromDateTime(DateTime.Now):O}");
      await DoRequestAsync(logger, client, "enddate", DateOnly.FromDateTime(DateTime.Now));

      await DoRequestAsync(logger, client, "textOrNumber/Number|42");

      await DoRequestAsync(logger, client, "notification/channels");
      await DoRequestAsync(logger, client, "notification/channels/email", "Test email");
   }

   private static async Task DoRequestAsync(ILogger logger, HttpClient client, string url, object? body = null, string? jsonBody = null)
   {
      var hasBody = body is not null || jsonBody is not null;
      var request = new HttpRequestMessage(hasBody ? HttpMethod.Post : HttpMethod.Get, "http://localhost:5000/api/" + url);

      if (body is not null)
         request.Content = JsonContent.Create(body);

      if (jsonBody is not null)
         request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

      if (hasBody)
      {
         logger.LogInformation("POST request with url '{Url}' and body '{Body}'", url, await request.Content!.ReadAsStringAsync());
      }
      else
      {
         logger.LogInformation("GET request with url '{Url}'", url);
      }

      using var response = await client.SendAsync(request);

      var content = await response.Content.ReadAsStringAsync();
      logger.LogInformation("Server responded with: [{StatusCode}] {Response}\n", response.StatusCode, content);
   }

   private static Task StartServerAsync(ILoggerFactory loggerFactory)
   {
      var webHost = new HostBuilder()
                    .ConfigureServices(services =>
                    {
                       services.AddSingleton(loggerFactory)
                               .AddSingleton<EmailNotificationSender>()
                               .AddSingleton<SmsNotificationSender>();
                       services.AddMvc(options => options.ModelBinderProviders.Insert(0, new ThinktectureModelBinderProvider()))
                               .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new ThinktectureJsonConverterFactory()))
                               .AddApplicationPart(typeof(DemoController).Assembly);

                       services.AddEndpointsApiExplorer()
                               .AddSwaggerGen(options => options.SwaggerDoc("demo", new OpenApiInfo { Title = "Demo API", Version = "v1" }))
                               .AddThinktectureOpenApiFilters(options => options.SmartEnumSchemaExtension = SmartEnumSchemaExtension.VarNamesFromStringRepresentation);
                    })
                    .ConfigureWebHostDefaults(builder =>
                    {
                       builder.UseKestrel()
                              .Configure(app =>
                              {
                                 app.UseRouting();
                                 app.UseEndpoints(endpoints =>
                                 {
                                    endpoints.MapSwagger();
                                    endpoints.MapControllers();
                                 });
                              });
                    })
                    .Build();

      return webHost.StartAsync();
   }

   private static Task StartMinimalWebApiAsync(ILoggerFactory loggerFactory)
   {
      var builder = WebApplication.CreateBuilder();
      builder.Services
             .AddSingleton(loggerFactory)
             .AddSingleton<EmailNotificationSender>()
             .AddSingleton<SmsNotificationSender>()
             .ConfigureHttpJsonOptions(options =>
             {
                options.SerializerOptions.Converters.Add(new ThinktectureJsonConverterFactory());
             })
             .AddEndpointsApiExplorer()
             .AddSwaggerGen(options => options.SwaggerDoc("demo", new OpenApiInfo { Title = "Demo API", Version = "v1" }))
             .AddThinktectureOpenApiFilters(options => options.SmartEnumSchemaExtension = SmartEnumSchemaExtension.VarNamesFromStringRepresentation);

      var app = builder.Build();

      app.MapSwagger();

      var routeGroup = app.MapGroup("/api");

      routeGroup.MapGet("category/{category}", (ProductCategory category) => new { Value = category, category.IsValid });
      routeGroup.MapGet("group/{group}", (ProductGroup group) => new { Value = group, group.IsValid });
      routeGroup.MapGet("productType/{productType}", (ProductType productType) => productType);
      routeGroup.MapGet("productType", (ProductType productType) => productType);
      routeGroup.MapGet("productTypeWithFilter", (BoundValueObject<ProductType, ProductTypeValidationError> productType) => ValueTask.FromResult(productType.Value))
                .AddEndpointFilter((context, next) =>
                {
                   var value = context.GetArgument<IBoundParam>(0);

                   if (value.Error is not null)
                      return new ValueTask<object?>(Results.BadRequest(value.Error));

                   return next(context);
                });
      routeGroup.MapGet("boundaryWithFactories/{boundary}", (BoundaryWithFactories boundary) => boundary);
      routeGroup.MapPost("productType", ([FromBody] ProductType productType) => productType);
      routeGroup.MapPost("productTypeWrapper", ([FromBody] ProductTypeWrapper productType) => productType);
      routeGroup.MapGet("productName/{name}", (ProductName name) => name);
      routeGroup.MapPost("productName", ([FromBody] ProductName name) => name);
      routeGroup.MapGet("otherProductName/{name}", (OtherProductName? name) => name);
      routeGroup.MapPost("otherProductName", ([FromBody] OtherProductName name) => name);
      routeGroup.MapPost("boundary", ([FromBody] Boundary boundary) => boundary);

      routeGroup.MapGet("enddate/{date}", (OpenEndDate date) => date);
      routeGroup.MapPost("enddate", ([FromBody] OpenEndDate date) => date);

      routeGroup.MapGet("textOrNumber/{textOrNumber}", (TextOrNumberSerializable textOrNumber) => textOrNumber);

      routeGroup.MapGet("notification/channels", () =>
      {
         var channels = NotificationChannelTypeDto.Items.Select(c => c.Name);
         return Results.Ok(channels);
      });
      routeGroup.MapPost("notification/channels/{type}",
                         async (
                            NotificationChannelTypeDto type,
                            [FromBody] string message,
                            [FromServices] IServiceProvider serviceProvider) =>
                         {
                            var notificationSender = type.GetNotificationSender(serviceProvider);
                            await notificationSender.SendAsync(message);
                         });

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
