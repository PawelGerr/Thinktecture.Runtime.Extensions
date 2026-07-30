using Microsoft.EntityFrameworkCore;
using Thinktecture.Runtime.Tests.TestEntities;
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
      var entityType = ctx.Model.FindEntityType(typeof(EntityWithStringSmartEnumCollection));
      var property = entityType.FindProperty(nameof(EntityWithStringSmartEnumCollection.Items));

      var elementType = property.GetElementType();

      // The element carries the max length ...
      elementType.GetMaxLength().Should().Be(10);

      // ... and the collection property must not be constrained to a single key length.
      property.GetMaxLength().Should().BeNull();

      // The element is also the item that gets the value converter.
      var valueConverter = elementType.GetValueConverter();
      valueConverter.Should().NotBeNull();
      valueConverter.ModelClrType.Should().Be(typeof(SmartEnum_StringBased));
      valueConverter.ProviderClrType.Should().Be(typeof(string));
   }
}
