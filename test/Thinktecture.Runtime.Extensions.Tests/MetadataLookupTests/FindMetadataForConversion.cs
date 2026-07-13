using System;
using Thinktecture.Internal;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.MetadataLookupTests;

// ReSharper disable once InconsistentNaming
public class FindMetadataForConversion
{
   private static readonly Func<ObjectFactoryMetadata, bool> _allFactories = _ => true;
   private static readonly Func<ObjectFactoryMetadata, bool> _noFactories = _ => false;
   private static readonly Func<Metadata.Keyed, bool> _allMetadata = _ => true;

   [Fact]
   public void Should_return_key_based_metadata_for_keyed_smart_enum()
   {
      var metadata = MetadataLookup.FindMetadataForConversion(typeof(SmartEnum_IntBased), _noFactories, _allMetadata);

      metadata.Should().NotBeNull();
      metadata!.Value.Type.Should().Be(typeof(SmartEnum_IntBased));
      metadata.Value.KeyType.Should().Be(typeof(int));
   }

   [Fact]
   public void Should_prefer_object_factory_over_key_based_metadata()
   {
      // IntBasedReferenceValueObject_With_StringBasedObjectFactory has key type int and a string-based object factory.
      var withFactory = MetadataLookup.FindMetadataForConversion(typeof(IntBasedReferenceValueObject_With_StringBasedObjectFactory), _allFactories, _allMetadata);
      var withoutFactory = MetadataLookup.FindMetadataForConversion(typeof(IntBasedReferenceValueObject_With_StringBasedObjectFactory), _noFactories, _allMetadata);

      // Object factories have priority over key-based metadata, so the factory's value type (string) wins.
      withFactory.Should().NotBeNull();
      withFactory!.Value.KeyType.Should().Be(typeof(string));

      // Without a matching factory the key-based metadata (int) is used.
      withoutFactory.Should().NotBeNull();
      withoutFactory!.Value.KeyType.Should().Be(typeof(int));
   }

   [Fact]
   public void Should_return_base_metadata_for_item_of_derived_smart_enum_type()
   {
      // Regression: FindMetadataForConversion returned null for runtime types that are derived (nested) classes,
      // because the exact-type comparison failed and the derived type has no own reflectable metadata property.
      var runtimeType = SmartEnum_DerivedTypes.ItemOfDerivedType.GetType();
      runtimeType.Should().NotBe(typeof(SmartEnum_DerivedTypes));

      var metadata = MetadataLookup.FindMetadataForConversion(runtimeType, _noFactories, _allMetadata);

      metadata.Should().NotBeNull();
      metadata!.Value.Type.Should().Be(typeof(SmartEnum_DerivedTypes));
      metadata.Value.KeyType.Should().Be(typeof(int));
   }

   [Fact]
   public void Should_return_null_for_type_without_metadata()
   {
      var metadata = MetadataLookup.FindMetadataForConversion(typeof(string), _allFactories, _allMetadata);

      metadata.Should().BeNull();
   }

   [Fact]
   public void Should_return_object_factory_metadata_for_derived_type_of_standalone_object_factory_class()
   {
      // Regression: FindObjectFactoryMetadata queried only the provided type for the private static ObjectFactories
      // property, which reflection never returns for a base type. A derived class of a standalone [ObjectFactory] plain
      // class therefore lost its object factories. FindObjectFactoryMetadata must walk the base types, as the metadata
      // path does via SearchBaseTypesForMetadata.
      typeof(DerivedClassWithStringObjectFactory).Should().NotBe(typeof(ClassWithStringObjectFactory));

      var metadata = MetadataLookup.FindMetadataForConversion(typeof(DerivedClassWithStringObjectFactory), _allFactories, _allMetadata);

      metadata.Should().NotBeNull();
      // The conversion metadata must report the type that owns the object factories (the annotated base type), not the
      // requested derived type. The generated IObjectFactory<T, ...> interface exists only for the base type, so building
      // a converter with the derived type would violate the invariant constraint of the generic converter.
      metadata!.Value.Type.Should().Be(typeof(ClassWithStringObjectFactory));
      metadata.Value.KeyType.Should().Be(typeof(string));
   }
}
