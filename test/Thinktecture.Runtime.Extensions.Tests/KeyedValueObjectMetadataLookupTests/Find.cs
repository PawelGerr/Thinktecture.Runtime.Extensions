using Thinktecture.Internal;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.KeyedValueObjectMetadataLookupTests;

public class Find
{
   [Fact]
   public void Should_find_metadata_of_enum_and_derived_types()
   {
      var metadata = KeyedValueObjectMetadataLookup.Find(EnumWithDerivedType.Item1.GetType());
      var derivedTypeMetadata = KeyedValueObjectMetadataLookup.Find(EnumWithDerivedType.ItemOfDerivedType.GetType());
      var innerTypeMetadata = KeyedValueObjectMetadataLookup.Find(EnumWithDerivedType.ItemOfInnerType.GetType());
      var genericDecimalMetadata = KeyedValueObjectMetadataLookup.Find(EnumWithDerivedType.GenericItemDecimal.GetType());

      var openGenericMetadata = KeyedValueObjectMetadataLookup.Find(EnumWithDerivedType.GenericItemDecimal.GetType().GetGenericTypeDefinition());

      metadata.Should().BeSameAs(derivedTypeMetadata);
      metadata.Should().BeSameAs(innerTypeMetadata);
      metadata.Should().BeSameAs(genericDecimalMetadata);
      metadata.Should().BeSameAs(openGenericMetadata);
   }

   [Fact]
   public void Should_not_find_metadata_of_keyless_enum()
   {
      var keylessMetadata = KeyedValueObjectMetadataLookup.Find(KeylessTestEnum.Item1.GetType());

      keylessMetadata.Should().BeNull();
   }
}
