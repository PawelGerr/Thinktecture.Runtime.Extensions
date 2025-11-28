using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Thinktecture.Database;

namespace Thinktecture.Benchmarks;

/*

29.04.2025

Runtime=.NET 10.0

| Method                         | Mean     | Error   | StdDev  | Gen0      | Gen1      | Allocated |
|------------------------------- |---------:|--------:|--------:|----------:|----------:|----------:|
| Entity_without_ValueObjects    | 111.6 ms | 2.21 ms | 3.44 ms | 3000.0000 | 2000.0000 |  79.21 MB |
| Entity_with_StructValueObjects | 115.5 ms | 2.09 ms | 4.58 ms | 3000.0000 | 2000.0000 |  87.27 MB |
| Entity_with_ClassValueObjects  | 150.4 ms | 2.93 ms | 4.56 ms | 4000.0000 | 3000.0000 |  83.79 MB |

 */

// ReSharper disable InconsistentNaming
public class LoadingValueObjects
{
   private BenchmarkContext? _benchmarkContext;
   private IServiceScope? _scope;
   private BenchmarkDbContext? _dbContext;

   private const int _NUMBER_OF_ENTITIES = 100_000;

   private readonly Entity_with_ValueObjects[] _Entity_with_ValueObjects = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_with_ValueObjects(i)
                                                                                                                                {
                                                                                                                                   Name = Name.Create("Name"),
                                                                                                                                   Description = Description.Create("Description"),
                                                                                                                                }).ToArray();
   private readonly Entity_without_ValueObjects[] _Entity_without_ValueObjects = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_without_ValueObjects(i)
                                                                                                                                      {
                                                                                                                                         Name = "Name",
                                                                                                                                         Description = "Description",
                                                                                                                                      }).ToArray();
   private readonly Entity_with_StructValueObjects[] _Entity_with_StructValueObjects = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_with_StructValueObjects(i)
                                                                                                                                            {
                                                                                                                                               Name = NameStruct.Create("Name"),
                                                                                                                                               Description = DescriptionStruct.Create("Description"),
                                                                                                                                            }).ToArray();

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
   public async Task Entity_without_ValueObjects()
   {
      await _dbContext!.Entity_without_ValueObjects.ToListAsync();
   }

   [Benchmark]
   public async Task Entity_with_StructValueObjects()
   {
      await _dbContext!.Entity_with_StructValueObjects.ToListAsync();
   }

   [Benchmark]
   public async Task Entity_with_ClassValueObjects()
   {
      await _dbContext!.Entity_with_ValueObjects.ToListAsync();
   }
}
