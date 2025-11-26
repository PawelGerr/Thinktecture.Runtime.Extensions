using Microsoft.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.TestEntities;

// ReSharper disable InconsistentNaming
public class TestDbContext : DbContext
{
   private readonly ValueConverterRegistration _valueConverterRegistration;
   private readonly bool _setConnectionString;
   private readonly Configuration _configuration;

   public DbSet<TestEntity_with_OwnedTypes> TestEntities_with_OwnedTypes { get; set; }
   public DbSet<TestEntity_with_Enum_and_ValueObjects> TestEntities_with_Enum_and_ValueObjects { get; set; }
   public DbSet<TestEntity_with_Types_having_ObjectFactories> TestEntities_with_Types_having_ObjectFactories { get; set; }
   public DbSet<TestEntityWithComplexType> TestEntities_with_ComplexType { get; set; }
   public DbSet<ComplexValueObjectWithComplexType> ComplexValueObject_with_ComplexType { get; set; }
   public DbSet<TestEntityWithComplexValueObjectAsComplexType> TestEntities_with_ComplexValueObjectAsComplexType { get; set; }

   public TestDbContext(
      DbContextOptions<TestDbContext> options,
      ValueConverterRegistration valueConverterRegistration = ValueConverterRegistration.None,
      Configuration configuration = null,
      bool setConnectionString = true)
      : base(options)
   {
      _valueConverterRegistration = valueConverterRegistration;
      _setConnectionString = setConnectionString;
      _configuration = configuration ?? Configuration.Default;
   }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
      base.OnConfiguring(optionsBuilder);

      if (_setConnectionString)
         optionsBuilder.UseSqlite("DataSource=:memory:");
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      TestEntity_with_OwnedTypes.Configure(modelBuilder, _valueConverterRegistration, _configuration);
      TestEntity_with_Enum_and_ValueObjects.Configure(modelBuilder, _valueConverterRegistration, _configuration);
      TestEntity_with_Types_having_ObjectFactories.Configure(modelBuilder, _valueConverterRegistration, _configuration);
      TestEntityWithComplexType.Configure(modelBuilder, _valueConverterRegistration, _configuration);
      ComplexValueObjectWithComplexType.Configure(modelBuilder, _valueConverterRegistration, _configuration);
      TestEntityWithComplexValueObjectAsComplexType.Configure(modelBuilder, _valueConverterRegistration, _configuration);

      if (_valueConverterRegistration == ValueConverterRegistration.OnModelCreating)
         modelBuilder.AddThinktectureValueConverters(_configuration);
   }
}
