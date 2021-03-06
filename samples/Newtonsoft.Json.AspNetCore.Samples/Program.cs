﻿using System;
using System.Net.Http;
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

namespace Thinktecture
{
   public class Program
   {
      // ReSharper disable once InconsistentNaming
      public static async Task Main()
      {
         var loggerFactory = CreateLoggerFactory();
         var server = StartServerAsync(loggerFactory);

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
         // 	http://localhost:5000/api/specialProductType/groceries
         // 	http://localhost:5000/api/specialProductType/special
         // 	http://localhost:5000/api/specialProductType/invalid
         // 	http://localhost:5000/api/productName/bread
         // 	http://localhost:5000/api/productName/a
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
         await DoRequestAsync(logger, client, "productType/invalid"); // invalid
         await DoRequestAsync(logger, client, "productTypeWithJsonConverter/groceries");
         await DoRequestAsync(logger, client, "productTypeWithJsonConverter/invalid"); // invalid
         await DoRequestAsync(logger, client, "specialProductType/groceries");
         await DoRequestAsync(logger, client, "specialProductType/special");
         await DoRequestAsync(logger, client, "specialProductType/invalid"); // invalid
         await DoRequestAsync(logger, client, "productName/bread");
         await DoRequestAsync(logger, client, "productName/a"); // invalid
      }

      private static async Task DoRequestAsync(ILogger logger, HttpClient client, string url)
      {
         logger.LogInformation("Making request with url '{Url}'", url);

         using var response = await client.GetAsync("http://localhost:5000/api/" + url);

         var content = await response.Content.ReadAsStringAsync();
         logger.LogInformation("Server responded with: [{StatusCode}] {Response}", response.StatusCode, content);
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
}
