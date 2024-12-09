using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Thinktecture.AspNetCore.ModelBinding;
using Thinktecture.Json;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Thinktecture;

public class Program
{
   // ReSharper disable once InconsistentNaming
   public static async Task Main()
   {
      var loggerFactory = CreateLoggerFactory();
      var server = StartServerAsync(loggerFactory);

      await DoHttpRequestsAsync(loggerFactory.CreateLogger<Program>());

      await server;
   }

   private static async Task DoHttpRequestsAsync(ILogger logger)
   {
      using var client = new HttpClient();

      await DoRequestAsync(logger, client, "category/fruits");
      await DoRequestAsync(logger, client, "categoryWithConverter/fruits");
      await DoRequestAsync(logger, client, "group/1");
      await DoRequestAsync(logger, client, "group/42"); // invalid
      await DoRequestAsync(logger, client, "groupWithConverter/1");
      await DoRequestAsync(logger, client, "groupWithConverter/42"); // invalid
      await DoRequestAsync(logger, client, "productType/groceries");
      await DoRequestAsync(logger, client, "productType/invalid");                                 // invalid
      await DoRequestAsync(logger, client, "productType", "invalid");                              // invalid
      await DoRequestAsync(logger, client, "productTypeWrapper", new { ProductType = "invalid" }); // invalid
      await DoRequestAsync(logger, client, "productTypeWithJsonConverter/groceries");
      await DoRequestAsync(logger, client, "productTypeWithJsonConverter/invalid"); // invalid
      await DoRequestAsync(logger, client, "productName/bread");
      await DoRequestAsync(logger, client, "productName/a"); // invalid
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
                                 .AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new ValueObjectNewtonsoftJsonConverter()));
                    })
                    .Build();

      return webHost.RunAsync();
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
