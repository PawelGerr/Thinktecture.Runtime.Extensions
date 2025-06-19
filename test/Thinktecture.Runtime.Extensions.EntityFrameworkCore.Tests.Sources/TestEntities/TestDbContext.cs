using Microsoft.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.TestEntities;

// ReSharper disable InconsistentNaming
public class TestDbContext : DbContext
{
   private readonly ValueConverterRegistration _valueConverterRegistration;

   public DbSet<TestEntity_with_OwnedTypes> TestEntities_with_OwnedTypes { get; set; }
   public DbSet<TestEntity_with_Enum_and_ValueObjects> TestEntities_with_Enum_and_ValueObjects { get; set; }
   public DbSet<TestEntity_with_Types_having_ObjectFactories> TestEntities_with_Types_having_ObjectFactories { get; set; }

#if COMPLEX_TYPES
   public DbSet<TestEntityWithComplexType> TestEntities_with_ComplexType { get; set; }
   public DbSet<ComplexValueObjectWithComplexType> ComplexValueObject_with_ComplexType { get; set; }
   public DbSet<TestEntityWithComplexValueObjectAsComplexType> TestEntities_with_ComplexValueObjectAsComplexType { get; set; }
#endif

   public TestDbContext(
      DbContextOptions<TestDbContext> options,
      ValueConverterRegistration valueConverterRegistration = ValueConverterRegistration.None)
      : base(options)
   {
      _valueConverterRegistration = valueConverterRegistration;
   }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
      base.OnConfiguring(optionsBuilder);

      optionsBuilder.UseSqlite("DataSource=:memory:");
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      TestEntity_with_OwnedTypes.Configure(modelBuilder, _valueConverterRegistration);
      TestEntity_with_Enum_and_ValueObjects.Configure(modelBuilder, _valueConverterRegistration);
      TestEntity_with_Types_having_ObjectFactories.Configure(modelBuilder, _valueConverterRegistration);

#if COMPLEX_TYPES
      TestEntityWithComplexType.Configure(modelBuilder, _valueConverterRegistration);
      ComplexValueObjectWithComplexType.Configure(modelBuilder, _valueConverterRegistration);
      TestEntityWithComplexValueObjectAsComplexType.Configure(modelBuilder, _valueConverterRegistration);
#endif

      if (_valueConverterRegistration == ValueConverterRegistration.OnModelCreating)
         modelBuilder.AddThinktectureValueConverters();
   }
}
