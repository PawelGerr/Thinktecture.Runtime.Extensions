using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Thinktecture.Database;

namespace Thinktecture.Benchmarks;

// ReSharper disable InconsistentNaming
[MemoryDiagnoser]
public class LoadingSmartEnums
{
   private BenchmarkContext? _benchmarkContext;
   private IServiceScope? _scope;
   private BenchmarkDbContext? _dbContext;

   private const int _NUMBER_OF_ENTITIES = 10_000;
   private static readonly RealEnum[] _enums = Enum.GetValues<RealEnum>();

   private readonly Entity_Enum_StringConverter[] _Entity_Enum_StringConverter = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_Enum_StringConverter(i, _enums[i % _enums.Length])).ToArray();
   private readonly Entity_Enum_IntBased[] _Entity_Enum_IntBased = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_Enum_IntBased(i, _enums[i % _enums.Length])).ToArray();
   private readonly Entity_SmartEnum_Class_StringBased[] _Entity_SmartEnum_Class_StringBased = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_SmartEnum_Class_StringBased(i, TestSmartEnum_Class_StringBased.Items[i % _enums.Length])).ToArray();
   private readonly Entity_SmartEnum_Struct_StringBased[] _Entity_SmartEnum_Struct_StringBased = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_SmartEnum_Struct_StringBased(i, TestSmartEnum_Struct_StringBased.Items[i % _enums.Length])).ToArray();
   private readonly Entity_SmartEnum_Class_IntBased[] _Entity_SmartEnum_Class_IntBased = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_SmartEnum_Class_IntBased(i, TestSmartEnum_Class_IntBased.Items[i % _enums.Length])).ToArray();
   private readonly Entity_SmartEnum_Struct_IntBased[] _Entity_SmartEnum_Struct_IntBased = Enumerable.Range(1, _NUMBER_OF_ENTITIES).Select(i => new Entity_SmartEnum_Struct_IntBased(i, TestSmartEnum_Struct_IntBased.Items[i % _enums.Length])).ToArray();

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

      _dbContext.RemoveRange(_dbContext.Entity_SmartEnum_Struct_StringBased);
      _dbContext.Entity_SmartEnum_Struct_StringBased.AddRange(_Entity_SmartEnum_Struct_StringBased);

      _dbContext.RemoveRange(_dbContext.Entity_SmartEnum_Class_IntBased);
      _dbContext.Entity_SmartEnum_Class_IntBased.AddRange(_Entity_SmartEnum_Class_IntBased);

      _dbContext.RemoveRange(_dbContext.Entity_SmartEnum_Struct_IntBased);
      _dbContext.Entity_SmartEnum_Struct_IntBased.AddRange(_Entity_SmartEnum_Struct_IntBased);

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
   public async Task SmartEnum_Struct_StringBased()
   {
      await _dbContext!.Entity_SmartEnum_Struct_StringBased.ToListAsync();
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
   public async Task SmartEnum_Struct_IntBased()
   {
      await _dbContext!.Entity_SmartEnum_Struct_IntBased.ToListAsync();
   }

   [Benchmark]
   public async Task SmartEnum_Class_IntBased()
   {
      await _dbContext!.Entity_SmartEnum_Class_IntBased.ToListAsync();
   }
}
