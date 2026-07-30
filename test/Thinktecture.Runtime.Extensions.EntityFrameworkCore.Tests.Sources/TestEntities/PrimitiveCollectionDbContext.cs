using Microsoft.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.TestEntities;

public class PrimitiveCollectionDbContext(
   DbContextOptions<PrimitiveCollectionDbContext> options,
   bool addValueConvertersOnEntity = false)
   : DbContext(options)
{
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      // The primitive collection must be declared explicitly, because EF Core does not map a collection of a type
      // without a type mapping by convention.
      modelBuilder.Entity<EntityWithStringSmartEnumCollection>(builder =>
      {
         builder.PrimitiveCollection(e => e.Items);

         if (addValueConvertersOnEntity)
            builder.AddThinktectureValueConverters();
      });
   }
}
