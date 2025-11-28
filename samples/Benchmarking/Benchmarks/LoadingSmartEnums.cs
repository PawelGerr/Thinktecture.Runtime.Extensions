using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Thinktecture.Database;

namespace Thinktecture.Benchmarks;

/*

28.11.2025

Runtime=.NET 10.0

| Method                      | Mean     | Error     | StdDev    | Allocated |
|---------------------------- |---------:|----------:|----------:|----------:|
| RegularEnum_StringConverter | 6.663 ms | 0.1269 ms | 0.3364 ms |   7.14 MB |
| SmartEnum_StringBased       | 6.859 ms | 0.2158 ms | 0.5570 ms |   8.19 MB |
| RegularEnum_IntBased        | 5.860 ms | 0.1823 ms | 0.4804 ms |   6.66 MB |
| SmartEnum_IntBased          | 6.377 ms | 0.1824 ms | 0.4838 ms |   7.71 MB |

 */

// ReSharper disable InconsistentNaming
public class LoadingSmartEnums
{
   private BenchmarkContext? _benchmarkContext;
   private IServiceScope? _scope;
   private BenchmarkDbContext? _dbContext;

   private const int _NUMBER_OF_ENTITIES = 10_000;
   private static readonly RealEnum[] _enums = Enum.GetValues<RealEnum>();

   private readonly Entity_RegularEnum_StringConverter[] _Entity_Enum_StringConverter
      = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_RegularEnum_StringConverter(i, _enums[i % _enums.Length]) { Enum = RealEnum.Value1 }).ToArray();
   private readonly Entity_RegularEnum_IntBased[] _Entity_Enum_IntBased
      = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_RegularEnum_IntBased(i, _enums[i % _enums.Length]) { Enum = RealEnum.Value1 }).ToArray();
   private readonly Entity_SmartEnum_StringBased[] _Entity_SmartEnum_Class_StringBased
      = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_SmartEnum_StringBased(i, TestSmartEnum_Class_StringBased.Items[i % _enums.Length]) { Enum = TestSmartEnum_Class_StringBased.Value1 }).ToArray();
   private readonly Entity_SmartEnum_IntBased[] _Entity_SmartEnum_Class_IntBased
      = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_SmartEnum_IntBased(i, TestSmartEnum_Class_IntBased.Items[i % _enums.Length]) { Enum = TestSmartEnum_Class_IntBased.Value1 }).ToArray();

   [GlobalSetup]
   public void Initialize()
   {
      _benchmarkContext = new BenchmarkContext();
      _scope = _benchmarkContext.RootServiceProvider.CreateScope();
      _dbContext = _scope.ServiceProvider.GetRequiredService<BenchmarkDbContext>();

      _dbContext.Database.OpenConnection();
      _dbContext.Database.EnsureCreated();

      _dbContext.RemoveRange(_dbContext.Entity_RegularEnum_StringConverter);
      _dbContext.Entity_RegularEnum_StringConverter.AddRange(_Entity_Enum_StringConverter);

      _dbContext.RemoveRange(_dbContext.Entity_RegularEnum_IntBased);
      _dbContext.Entity_RegularEnum_IntBased.AddRange(_Entity_Enum_IntBased);

      _dbContext.RemoveRange(_dbContext.Entity_SmartEnum_StringBased);
      _dbContext.Entity_SmartEnum_StringBased.AddRange(_Entity_SmartEnum_Class_StringBased);

      _dbContext.RemoveRange(_dbContext.Entity_SmartEnum_IntBased);
      _dbContext.Entity_SmartEnum_IntBased.AddRange(_Entity_SmartEnum_Class_IntBased);

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
   public async Task RegularEnum_StringConverter()
   {
      await _dbContext!.Entity_RegularEnum_StringConverter.ToListAsync();
   }

   [Benchmark]
   public async Task SmartEnum_StringBased()
   {
      await _dbContext!.Entity_SmartEnum_StringBased.ToListAsync();
   }

   [Benchmark]
   public async Task RegularEnum_IntBased()
   {
      await _dbContext!.Entity_RegularEnum_IntBased.ToListAsync();
   }

   [Benchmark]
   public async Task SmartEnum_IntBased()
   {
      await _dbContext!.Entity_SmartEnum_IntBased.ToListAsync();
   }
}
