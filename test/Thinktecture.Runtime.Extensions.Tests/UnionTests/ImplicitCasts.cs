using System;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

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
}
