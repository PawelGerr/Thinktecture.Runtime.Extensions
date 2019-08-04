﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using Thinktecture.Json;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Thinktecture
{
   public class Program
   {
      public static async Task Main()
      {
         var loggerFactory = CreateLoggerFactory();
         var server = StartServerAsync(loggerFactory);

         // calls
         // 	http://localhost:5000/api/category/fruits
         // 	http://localhost:5000/api/categoryWithConverter/fruits
         // 	http://localhost:5000/api/group/1
         // 	http://localhost:5000/api/groupWithConverter/1
         await DoHttpRequestsAsync(loggerFactory.CreateLogger<Program>());

         await server;
      }

      private static async Task DoHttpRequestsAsync(ILogger logger)
      {
         using (var client = new HttpClient())
         {
            await DoRequestAsync(logger, client, "category/fruits");
            await DoRequestAsync(logger, client, "categoryWithConverter/fruits");
            await DoRequestAsync(logger, client, "group/1");
            await DoRequestAsync(logger, client, "groupWithConverter/1");
         }
      }

      private static async Task DoRequestAsync(ILogger logger, HttpClient client, string url)
      {
         logger.LogWarning("Making request with url '{url}'", url);

         using (var response = await client.GetAsync("http://localhost:5000/api/" + url))
         {
            var content = await response.Content.ReadAsStringAsync();
            logger.LogWarning("Server responded with: {response}", content);
         }
      }

      private static Task StartServerAsync(ILoggerFactory loggerFactory)
      {
         var webHost = new WebHostBuilder()
                       .UseKestrel()
                       .ConfigureServices(collection =>
                                          {
                                             collection.AddSingleton(loggerFactory);
                                             collection.AddMvc()
                                                       .AddJsonOptions(options => options.SerializerSettings.Converters.Add(new EnumJsonConverter()));
                                          })
                       .Configure(builder => builder.UseMvc())
                       .Build();

         return webHost.RunAsync();
      }

      private static ILoggerFactory CreateLoggerFactory()
      {
         var serilog = new LoggerConfiguration()
                       .WriteTo.Console()
                       .MinimumLevel.Information()
                       .CreateLogger();

         var loggerFactory = new LoggerFactory();
         loggerFactory.AddSerilog(serilog);

         return loggerFactory;
      }
   }
}