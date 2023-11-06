using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Thinktecture.Database;

namespace Thinktecture.Benchmarks;

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
