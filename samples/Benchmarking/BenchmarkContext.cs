using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Thinktecture.Database;

namespace Thinktecture;

public class BenchmarkContext : IDisposable
{
   public ServiceProvider RootServiceProvider { get; }

   public BenchmarkContext()
   {
      var configuration = GetConfiguration();
      var services = new ServiceCollection()
                     .AddLogging(builder => builder.AddConsole())
                     .AddDbContext<BenchmarkDbContext>(builder => builder
                                                                  .UseSqlServer(configuration.GetConnectionString("default"))
                                                                  .UseLoggerFactory(NullLoggerFactory.Instance)
                                                                  .UseThinktectureValueConverters());

      RootServiceProvider = services.BuildServiceProvider();
   }

   private static IConfiguration GetConfiguration()
   {
      return new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .AddUserSecrets<Program>()
             .Build();
   }

   public void Dispose()
   {
      RootServiceProvider.Dispose();
   }
}
