using Thinktecture.Runtime.Tests.TestAdHocUnions;

#nullable enable

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

public class ConversionFromValue
{
   [Fact]
   public void Should_have_implicit_conversion_from_nullable_struct_to_nullable_class_union()
   {
      var nullableInt = (int?)42;
      var union = (TestUnion_class_string_int?)nullableInt;

      union.Should().NotBeNull();
      union.IsInt32.Should().BeTrue();
      union.AsInt32.Should().Be(42);
   }

   [Fact]
   public void Should_return_null_when_converting_null_nullable_struct_to_nullable_class_union()
   {
      var nullableInt = (int?)null;
      var union = (TestUnion_class_string_int?)nullableInt;

      union.Should().BeNull();
   }

   [Fact]
   public void Should_have_implicit_conversion_from_nullable_reference_type_to_nullable_class_union()
   {
      var nullableString = (string?)"value";
      var union = (TestUnion_class_string_int?)nullableString!;

      union.Should().NotBeNull();
      union.IsString.Should().BeTrue();
      union.AsString.Should().Be("value");
   }

   [Fact]
   public void Should_return_null_when_converting_null_to_nullable_class_union()
   {
      string? nullableString = null;
      var union = (TestUnion_class_string_int?)nullableString!;

      union.Should().NotBeNull();
      union.AsString.Should().BeNull();
   }

   [Fact]
   public void Should_have_implicit_conversion_from_nullable_struct_to_nullable_struct_union()
   {
      int? nullableInt = 42;
      var union = (TestUnion_struct_string_int?)nullableInt;

      union.Should().NotBeNull();
      union.Value.IsInt32.Should().BeTrue();
      union.Value.AsInt32.Should().Be(42);
   }

   [Fact]
   public void Should_return_null_when_converting_null_nullable_struct_to_nullable_struct_union()
   {
      int? nullableInt = null;
      var union = (TestUnion_struct_string_int?)nullableInt;

      union.Should().BeNull();
   }
}
