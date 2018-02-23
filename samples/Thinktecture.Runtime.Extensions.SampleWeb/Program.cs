using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Thinktecture.Runtime.Extensions.SampleWeb
{
	public class Program
	{
		public static async Task Main()
		{
			var server = StartServerAsync();

			await DoHttpRequestAsync(); // calls http://localhost:5000/api/fruits

			await server;
		}

		private static async Task DoHttpRequestAsync()
		{
			using (var client = new HttpClient())
			using (var response = await client.GetAsync("http://localhost:5000/api/fruits"))
			{
				var content = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Server responded with: {content}");
			}
		}

		private static Task StartServerAsync()
		{
			return new WebHostBuilder()
			       .UseKestrel()
			       .ConfigureServices(collection => collection.AddMvc())
			       .Configure(builder => builder.UseMvc())
			       .Build()
			       .RunAsync();
		}
	}
}
