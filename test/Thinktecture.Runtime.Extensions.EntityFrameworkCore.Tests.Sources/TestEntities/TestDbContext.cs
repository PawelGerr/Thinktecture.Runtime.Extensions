using Microsoft.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.TestEntities;

// ReSharper disable InconsistentNaming
public class TestDbContext : DbContext
{
   private readonly bool _registerConverters;
   public DbSet<TestEntity_with_OwnedTypes> TestEntities_with_OwnedTypes { get; set; }
   public DbSet<TestEntity_with_Enum_and_ValueObjects> TestEntities_with_Enum_and_ValueObjects { get; set; }

#if COMPLEX_TYPES
   public DbSet<TestEntityWithComplexType> TestEntities_with_ComplexType { get; set; }
#endif

   public TestDbContext(DbContextOptions<TestDbContext> options, bool registerConverters = false)
      : base(options)
   {
      _registerConverters = registerConverters;
   }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
      base.OnConfiguring(optionsBuilder);

      optionsBuilder.UseSqlite("DataSource=:memory:");
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      TestEntity_with_OwnedTypes.Configure(modelBuilder);
      TestEntity_with_Enum_and_ValueObjects.Configure(modelBuilder);

#if COMPLEX_TYPES
      TestEntityWithComplexType.Configure(modelBuilder);
#endif

      if (_registerConverters)
         modelBuilder.AddValueObjectConverters(true);
   }
}
