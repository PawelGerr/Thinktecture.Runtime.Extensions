using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

public class ImplicitCasts
{
   [Fact]
   public void Should_have_implicit_casts_from_value_having_2_values()
   {
      TestUnion_class_string_int stringUnion = "text";
      stringUnion.Value.Should().Be("text");

      TestUnion_class_string_int intUnion = 42;
      intUnion.Value.Should().Be(42);

      TestUnion_struct_string_int stringStructUnion = "text";
      stringStructUnion.Value.Should().Be("text");

      TestUnion_struct_string_int intStructUnion = 42;
      intStructUnion.Value.Should().Be(42);

      TestUnion_class_with_array arrayClassUnion = new[] { "text" };
      arrayClassUnion.Value.Should().BeEquivalentTo(new[] { "text" });
   }

   [Fact]
   public void Should_have_implicit_casts_from_value_having_3_values()
   {
      TestUnion_class_string_int_bool stringUnion = "text";
      stringUnion.Value.Should().Be("text");

      TestUnion_class_string_int_bool intUnion = 42;
      intUnion.Value.Should().Be(42);

      TestUnion_class_string_int_bool boolUnion = true;
      boolUnion.Value.Should().Be(true);
   }

   [Fact]
   public void Should_have_implicit_casts_from_value_having_4_values()
   {
      TestUnion_class_string_int_bool_guid stringUnion = "text";
      stringUnion.Value.Should().Be("text");

      TestUnion_class_string_int_bool_guid intUnion = 42;
      intUnion.Value.Should().Be(42);

      TestUnion_class_string_int_bool_guid boolUnion = true;
      boolUnion.Value.Should().Be(true);

      TestUnion_class_string_int_bool_guid guidUnion = new Guid("2FCC1FE5-5A0D-4FE7-ADBB-356CCABEDAC2");
      guidUnion.Value.Should().Be(new Guid("2FCC1FE5-5A0D-4FE7-ADBB-356CCABEDAC2"));
   }

   [Fact]
   public void Should_have_implicit_casts_from_value_having_5_values()
   {
      TestUnion_class_string_int_bool_guid_char stringUnion = "text";
      stringUnion.Value.Should().Be("text");

      TestUnion_class_string_int_bool_guid_char intUnion = 42;
      intUnion.Value.Should().Be(42);

      TestUnion_class_string_int_bool_guid_char boolUnion = true;
      boolUnion.Value.Should().Be(true);

      TestUnion_class_string_int_bool_guid_char guidUnion = new Guid("2FCC1FE5-5A0D-4FE7-ADBB-356CCABEDAC2");
      guidUnion.Value.Should().Be(new Guid("2FCC1FE5-5A0D-4FE7-ADBB-356CCABEDAC2"));

      TestUnion_class_string_int_bool_guid_char charUnion = 'A';
      charUnion.Value.Should().Be('A');
   }

   [Fact]
   public void Should_have_implicit_casts_from_value_having_5_values_with_duplicates()
   {
      TestUnion_class_with_same_types intUnion = 42;
      intUnion.Value.Should().Be(42);

      TestUnion_class_with_same_types nullableIntUnion = (int?)42;
      nullableIntUnion.Value.Should().Be(42);
   }

   [Fact]
   public void Should_support_implicit_conversion_from_stateless_type()
   {
      TestUnion_class_stateless_nullvaluestruct_string union = new NullValueStruct();
      union.IsNullValueStruct.Should().BeTrue();
      union.AsNullValueStruct.Should().Be(default(NullValueStruct));
   }

   [Fact]
   public void Should_support_implicit_conversion_from_regular_type_with_marker()
   {
      TestUnion_class_stateless_nullvaluestruct_string union = "text";
      union.IsString.Should().BeTrue();
      union.AsString.Should().Be("text");
   }

   [Fact]
   public void Should_support_conversion_with_stateless_type_T2()
   {
      TestUnion_class_string_stateless_emptystatestruct union1 = "text";
      union1.IsString.Should().BeTrue();

      TestUnion_class_string_stateless_emptystatestruct union2 = new EmptyStateStruct();
      union2.IsEmptyStateStruct.Should().BeTrue();
   }

   [Fact]
   public void Should_support_conversion_with_multiple_stateless_types()
   {
      TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string union1 = new NullValueStruct();
      union1.IsNullValueStruct.Should().BeTrue();

      TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string union2 = new EmptyStateStruct();
      union2.IsEmptyStateStruct.Should().BeTrue();

      TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string union3 = "text";
      union3.IsString.Should().BeTrue();
   }

   [Fact]
   public void Should_support_conversion_in_struct_union()
   {
      TestUnion_struct_stateless_nullvaluestruct_int union1 = new NullValueStruct();
      union1.IsNullValueStruct.Should().BeTrue();

      TestUnion_struct_stateless_nullvaluestruct_int union2 = 42;
      union2.IsInt32.Should().BeTrue();
   }

   [Fact]
   public void Should_support_conversion_from_nullable_struct_marker()
   {
      TestUnion_class_string_stateless_emptystatestruct_nullable union1 = (EmptyStateStruct?)null;
      union1.IsNullableOfEmptyStateStruct.Should().BeTrue();

      TestUnion_class_string_stateless_emptystatestruct_nullable union2 = (EmptyStateStruct?)new EmptyStateStruct();
      union2.IsNullableOfEmptyStateStruct.Should().BeTrue();

      TestUnion_class_string_stateless_emptystatestruct_nullable union3 = "text";
      union3.IsString.Should().BeTrue();
   }

   [Fact]
   public void Should_support_conversion_from_reference_type_marker()
   {
      TestUnion_class_stateless_nullvalueclass_string union = new NullValueClass();
      union.IsNullValueClass.Should().BeTrue();
      union.AsNullValueClass.Should().BeNull();
   }

   [Fact]
   public void Should_support_conversion_from_regular_type_with_reference_marker()
   {
      TestUnion_class_stateless_nullvalueclass_string union = "text";
      union.IsString.Should().BeTrue();
      union.AsString.Should().Be("text");
   }
}
