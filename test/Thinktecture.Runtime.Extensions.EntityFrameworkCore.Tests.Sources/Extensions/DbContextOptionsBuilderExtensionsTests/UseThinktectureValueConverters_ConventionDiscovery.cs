using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Extensions.DbContextOptionsBuilderExtensionsTests;

// ReSharper disable InconsistentNaming
public class UseThinktectureValueConverters_ConventionDiscovery
{
   // Regression test for the conventions plugin applying the element max length of a primitive collection to the
   // collection property instead of the element.
   //
   // A primitive collection is persisted as a single JSON column. The default strategy computes the max length from
   // the longest key of the string-based Smart Enum (rounded up to the next multiple of 10, i.e. 10). Before the fix
   // the convention applied this length to the collection property, which would constrain the whole JSON array to the
   // length of one key. After the fix the length is applied to the element, and the collection property stays
   // unconstrained.
   [Fact]
   public void Should_apply_element_max_length_to_element_of_string_based_smart_enum_collection()
   {
      var options = new DbContextOptionsBuilder<PrimitiveCollectionDbContext>()
                    .UseSqlite("DataSource=:memory:")
                    .EnableServiceProviderCaching(false)
                    .UseThinktectureValueConverters()
                    .Options;

      using var ctx = new PrimitiveCollectionDbContext(options);
      var entityType = ctx.Model.FindEntityType(typeof(EntityWithStringSmartEnumCollection)) ?? throw new Exception("Entity not found");
      var property = entityType.FindProperty(nameof(EntityWithStringSmartEnumCollection.Items)) ?? throw new Exception("Collection property not found");

      var elementType = property.GetElementType() ?? throw new Exception("Element type not found");

      // The element carries the max length ...
      elementType.GetMaxLength().Should().Be(10);

      // ... and the collection property must not be constrained to a single key length.
      property.GetMaxLength().Should().BeNull();
   }

   private class PrimitiveCollectionDbContext(DbContextOptions<PrimitiveCollectionDbContext> options) : DbContext(options)
   {
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);

         // The primitive collection must be declared explicitly, because EF Core does not map a collection of a type
         // without a type mapping by convention.
         modelBuilder.Entity<EntityWithStringSmartEnumCollection>(builder => builder.PrimitiveCollection(e => e.Items));
      }
   }
}

public class EntityWithStringSmartEnumCollection
{
   public Guid Id { get; set; }
   public List<SmartEnum_StringBased> Items { get; set; } = [];
}
