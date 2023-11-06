using System;
using Microsoft.EntityFrameworkCore;
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
      var services = new ServiceCollection()
                     .AddLogging(builder => builder.AddConsole())
                     .AddDbContext<BenchmarkDbContext>(builder => builder
                                                                  .UseSqlServer("Server=localhost;Database=TT_RE_Benchmarking;Integrated Security=true;TrustServerCertificate=true;")
                                                                  .UseLoggerFactory(NullLoggerFactory.Instance)
                                                                  .UseValueObjectValueConverter());

      RootServiceProvider = services.BuildServiceProvider();
   }

   public void Dispose()
   {
      RootServiceProvider.Dispose();
   }
}
