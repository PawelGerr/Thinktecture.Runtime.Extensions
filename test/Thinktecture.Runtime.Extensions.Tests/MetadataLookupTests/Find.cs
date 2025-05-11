using Thinktecture.Internal;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.MetadataLookupTests;

public class Find
{
   [Fact]
   public void Should_find_metadata_of_enum_and_derived_types()
   {
      var metadata = MetadataLookup.Find(SmartEnum_DerivedTypes.Item1.GetType());
      var derivedTypeMetadata = MetadataLookup.Find(SmartEnum_DerivedTypes.ItemOfDerivedType.GetType());
      var innerTypeMetadata = MetadataLookup.Find(SmartEnum_DerivedTypes.ItemOfInnerType.GetType());
      var genericDecimalMetadata = MetadataLookup.Find(SmartEnum_DerivedTypes.GenericItemDecimal.GetType());

      var openGenericMetadata = MetadataLookup.Find(SmartEnum_DerivedTypes.GenericItemDecimal.GetType().GetGenericTypeDefinition());

      metadata.Should().BeSameAs(derivedTypeMetadata);
      metadata.Should().BeSameAs(innerTypeMetadata);
      metadata.Should().BeSameAs(genericDecimalMetadata);
      metadata.Should().BeSameAs(openGenericMetadata);
   }

   [Fact]
   public void Should_not_find_metadata_of_keyless_enum()
   {
      var keylessMetadata = MetadataLookup.Find(SmartEnum_Keyless.Item1.GetType());

      keylessMetadata.Should().BeNull();
   }
}
