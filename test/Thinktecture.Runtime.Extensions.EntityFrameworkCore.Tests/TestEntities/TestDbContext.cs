using Microsoft.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.TestEntities
{
   public class TestDbContext : DbContext
   {
      public DbSet<TestEntity_with_OwnedTypes> TestEntities_with_OwnedTypes { get; set; }
      public DbSet<TestEntity_with_Enum_and_ValueTypes> TestEntities_with_Enum_and_ValueTypes { get; set; }

      public TestDbContext()
      {
      }

      public TestDbContext(DbContextOptions<TestDbContext> options)
         : base(options)
      {
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
         TestEntity_with_Enum_and_ValueTypes.Configure(modelBuilder);

         modelBuilder.AddEnumAndValueTypeConverters(true);
      }
   }
}
