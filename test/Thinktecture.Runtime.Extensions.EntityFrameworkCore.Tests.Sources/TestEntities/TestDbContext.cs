using Microsoft.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.TestEntities;

// ReSharper disable InconsistentNaming
public class TestDbContext : DbContext
{
   private readonly ValueConverterRegistration _valueConverterRegistration;

   public DbSet<TestEntity_with_OwnedTypes> TestEntities_with_OwnedTypes { get; set; }
   public DbSet<TestEntity_with_Enum_and_ValueObjects> TestEntities_with_Enum_and_ValueObjects { get; set; }

#if COMPLEX_TYPES
   public DbSet<TestEntityWithComplexType> TestEntities_with_ComplexType { get; set; }
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

      var configureOnEntityTypeLevel = _valueConverterRegistration is ValueConverterRegistration.EntityConfiguration or ValueConverterRegistration.ComplexTypeConfiguration;

      TestEntity_with_OwnedTypes.Configure(modelBuilder, configureOnEntityTypeLevel);
      TestEntity_with_Enum_and_ValueObjects.Configure(modelBuilder, configureOnEntityTypeLevel);

#if COMPLEX_TYPES
      TestEntityWithComplexType.Configure(modelBuilder, _valueConverterRegistration);
#endif

      if (_valueConverterRegistration == ValueConverterRegistration.OnModelCreating)
         modelBuilder.AddValueObjectConverters(true);
   }
}
