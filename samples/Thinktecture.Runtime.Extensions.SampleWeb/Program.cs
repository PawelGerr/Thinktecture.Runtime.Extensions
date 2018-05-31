using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;

namespace Thinktecture.Runtime.Extensions.SampleWeb
{
	public class Program
	{
		public static async Task Main()
		{
			var loggerFactory = CreateLoggerFactory();
			var server = StartServerAsync(loggerFactory);

			await DoHttpRequestAsync().ConfigureAwait(false); // calls http://localhost:5000/api/fruits

			await server.ConfigureAwait(false);
		}

		private static async Task DoHttpRequestAsync()
		{
			using (var client = new HttpClient())
			using (var response = await client.GetAsync("http://localhost:5000/api/fruits").ConfigureAwait(false))
			{
				var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				Console.WriteLine($"Server responded with: {content}");
			}
		}

		private static Task StartServerAsync(ILoggerFactory loggerFactory)
		{
			var webHost = new WebHostBuilder()
			              .UseKestrel()
			              .ConfigureServices(collection =>
			              {
				              collection.AddSingleton(loggerFactory);
				              collection.AddMvc();
			              })
			              .Configure(builder =>
			              {
				              builder.Use((context, func) =>
				              {
					              var id = Guid.NewGuid().ToString("N");

					              using (LogContext.PushProperty("RequestId", id))
					              {
						              context.Features.Get<IHttpRequestIdentifierFeature>().TraceIdentifier = id;
						              return func();
					              }
				              });

				              builder.UseMvc();
			              })
			              .Build();

			return webHost.RunAsync();
		}

		private static ILoggerFactory CreateLoggerFactory()
		{
			var serilog = new LoggerConfiguration()
			              .WriteTo.Console()
			              .MinimumLevel.Verbose()
			              .CreateLogger();

			var loggerFactory = new LoggerFactory();
			loggerFactory.AddSerilog(serilog);

			return loggerFactory;
		}
	}
}
