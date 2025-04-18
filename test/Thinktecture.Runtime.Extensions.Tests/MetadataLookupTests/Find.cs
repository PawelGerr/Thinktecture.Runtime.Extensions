using Thinktecture.Internal;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.MetadataLookupTests;

public class Find
{
   [Fact]
   public void Should_find_metadata_of_enum_and_derived_types()
   {
      var metadata = MetadataLookup.Find(EnumWithDerivedType.Item1.GetType());
      var derivedTypeMetadata = MetadataLookup.Find(EnumWithDerivedType.ItemOfDerivedType.GetType());
      var innerTypeMetadata = MetadataLookup.Find(EnumWithDerivedType.ItemOfInnerType.GetType());
      var genericDecimalMetadata = MetadataLookup.Find(EnumWithDerivedType.GenericItemDecimal.GetType());

      var openGenericMetadata = MetadataLookup.Find(EnumWithDerivedType.GenericItemDecimal.GetType().GetGenericTypeDefinition());

      metadata.Should().BeSameAs(derivedTypeMetadata);
      metadata.Should().BeSameAs(innerTypeMetadata);
      metadata.Should().BeSameAs(genericDecimalMetadata);
      metadata.Should().BeSameAs(openGenericMetadata);
   }

   [Fact]
   public void Should_not_find_metadata_of_keyless_enum()
   {
      var keylessMetadata = MetadataLookup.Find(KeylessTestEnum.Item1.GetType());

      keylessMetadata.Should().BeNull();
   }
}
