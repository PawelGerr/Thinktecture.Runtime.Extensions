using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Thinktecture.Database;

namespace Thinktecture.Benchmarks;

/*

15.04.2025

.NET 9.0.4

| Method                       | Mean     | Error    | StdDev    | Median    | Allocated |
|----------------------------- |---------:|---------:|----------:|----------:|----------:|
| Real_Enum_StringConverter    | 11.34 ms | 1.382 ms |  3.876 ms |  9.640 ms |   7.16 MB |
| SmartEnum_Struct_StringBased | 11.63 ms | 1.485 ms |  4.214 ms | 10.541 ms |   8.51 MB |
| SmartEnum_Class_StringBased  | 15.93 ms | 2.599 ms |  7.416 ms | 13.270 ms |   8.21 MB |
| Real_Enum_IntBased           | 15.07 ms | 2.923 ms |  8.574 ms | 11.595 ms |   6.67 MB |
| SmartEnum_Struct_IntBased    | 16.91 ms | 4.670 ms | 13.399 ms | 10.216 ms |   8.02 MB |
| SmartEnum_Class_IntBased     | 20.11 ms | 5.255 ms | 15.078 ms | 12.856 ms |   7.72 MB |


 */

// ReSharper disable InconsistentNaming
public class LoadingSmartEnums
{
   private BenchmarkContext? _benchmarkContext;
   private IServiceScope? _scope;
   private BenchmarkDbContext? _dbContext;

   private const int _NUMBER_OF_ENTITIES = 10_000;
   private static readonly RealEnum[] _enums = Enum.GetValues<RealEnum>();

   private readonly Entity_Enum_StringConverter[] _Entity_Enum_StringConverter
      = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_Enum_StringConverter(i, _enums[i % _enums.Length]) { Enum = RealEnum.Value1 }).ToArray();
   private readonly Entity_Enum_IntBased[] _Entity_Enum_IntBased
      = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_Enum_IntBased(i, _enums[i % _enums.Length]) { Enum = RealEnum.Value1 }).ToArray();
   private readonly Entity_SmartEnum_Class_StringBased[] _Entity_SmartEnum_Class_StringBased
      = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_SmartEnum_Class_StringBased(i, TestSmartEnum_Class_StringBased.Items[i % _enums.Length]) { Enum = TestSmartEnum_Class_StringBased.Value1 }).ToArray();
   private readonly Entity_SmartEnum_Class_IntBased[] _Entity_SmartEnum_Class_IntBased
      = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_SmartEnum_Class_IntBased(i, TestSmartEnum_Class_IntBased.Items[i % _enums.Length]) { Enum = TestSmartEnum_Class_IntBased.Value1 }).ToArray();

   [GlobalSetup]
   public void Initialize()
   {
      _benchmarkContext = new BenchmarkContext();
      _scope = _benchmarkContext.RootServiceProvider.CreateScope();
      _dbContext = _scope.ServiceProvider.GetRequiredService<BenchmarkDbContext>();

      _dbContext.Database.OpenConnection();
      _dbContext.Database.EnsureCreated();

      _dbContext.RemoveRange(_dbContext.Entity_Enum_StringConverter);
      _dbContext.Entity_Enum_StringConverter.AddRange(_Entity_Enum_StringConverter);

      _dbContext.RemoveRange(_dbContext.Entity_Enum_IntBased);
      _dbContext.Entity_Enum_IntBased.AddRange(_Entity_Enum_IntBased);

      _dbContext.RemoveRange(_dbContext.Entity_SmartEnum_Class_StringBased);
      _dbContext.Entity_SmartEnum_Class_StringBased.AddRange(_Entity_SmartEnum_Class_StringBased);

      _dbContext.RemoveRange(_dbContext.Entity_SmartEnum_Class_IntBased);
      _dbContext.Entity_SmartEnum_Class_IntBased.AddRange(_Entity_SmartEnum_Class_IntBased);

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
   public async Task Real_Enum_StringConverter()
   {
      await _dbContext!.Entity_Enum_StringConverter.ToListAsync();
   }

   [Benchmark]
   public async Task SmartEnum_Class_StringBased()
   {
      await _dbContext!.Entity_SmartEnum_Class_StringBased.ToListAsync();
   }

   [Benchmark]
   public async Task Real_Enum_IntBased()
   {
      await _dbContext!.Entity_Enum_IntBased.ToListAsync();
   }

   [Benchmark]
   public async Task SmartEnum_Class_IntBased()
   {
      await _dbContext!.Entity_SmartEnum_Class_IntBased.ToListAsync();
   }
}
