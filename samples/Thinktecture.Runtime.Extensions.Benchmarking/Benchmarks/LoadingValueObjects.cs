using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Thinktecture.Database;

namespace Thinktecture.Benchmarks;

/*

22.01.2025

.NET 9.0.1

| Method                         | Mean     | Error   | StdDev   | Median   | Gen0      | Gen1      | Allocated |
|------------------------------- |---------:|--------:|---------:|---------:|----------:|----------:|----------:|
| Entity_with_ValueObjects       | 178.4 ms | 7.83 ms | 22.73 ms | 172.4 ms | 4000.0000 | 3000.0000 |  89.13 MB |
| Entity_without_ValueObjects    | 175.6 ms | 8.77 ms | 25.84 ms | 166.2 ms | 4000.0000 | 3000.0000 |  84.55 MB |
| Entity_with_StructValueObjects | 164.7 ms | 8.33 ms | 24.30 ms | 157.5 ms | 4000.0000 | 3000.0000 |  92.61 MB |

 */

// ReSharper disable InconsistentNaming
public class LoadingValueObjects
{
   private BenchmarkContext? _benchmarkContext;
   private IServiceScope? _scope;
   private BenchmarkDbContext? _dbContext;

   private const int _NUMBER_OF_ENTITIES = 100_000;

   private readonly Entity_with_ValueObjects[] _Entity_with_ValueObjects = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_with_ValueObjects(i)).ToArray();
   private readonly Entity_without_ValueObjects[] _Entity_without_ValueObjects = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_without_ValueObjects(i)).ToArray();
   private readonly Entity_with_StructValueObjects[] _Entity_with_StructValueObjects = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_with_StructValueObjects(i)).ToArray();

   [GlobalSetup]
   public void Initialize()
   {
      _benchmarkContext = new BenchmarkContext();
      _scope = _benchmarkContext.RootServiceProvider.CreateScope();
      _dbContext = _scope.ServiceProvider.GetRequiredService<BenchmarkDbContext>();

      _dbContext.Database.OpenConnection();
      _dbContext.Database.EnsureCreated();

      _dbContext.RemoveRange(_dbContext.Entity_with_ValueObjects);
      _dbContext.Entity_with_ValueObjects.AddRange(_Entity_with_ValueObjects);

      _dbContext.RemoveRange(_dbContext.Entity_without_ValueObjects);
      _dbContext.Entity_without_ValueObjects.AddRange(_Entity_without_ValueObjects);

      _dbContext.RemoveRange(_dbContext.Entity_with_StructValueObjects);
      _dbContext.Entity_with_StructValueObjects.AddRange(_Entity_with_StructValueObjects);

      _dbContext.SaveChanges();
   }

   [IterationSetup]
   public void IterationSetup()
   {
      _dbContext!.ChangeTracker.Clear();
   }

   [GlobalCleanup]
   public void Dispose()
   {
      _scope?.Dispose();
      _benchmarkContext?.Dispose();
   }

   [Benchmark]
   public async Task Entity_with_ValueObjects()
   {
      await _dbContext!.Entity_with_ValueObjects.ToListAsync();
   }

   [Benchmark]
   public async Task Entity_without_ValueObjects()
   {
      await _dbContext!.Entity_without_ValueObjects.ToListAsync();
   }

   [Benchmark]
   public async Task Entity_with_StructValueObjects()
   {
      await _dbContext!.Entity_with_StructValueObjects.ToListAsync();
   }
}
