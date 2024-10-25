using System;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

public class ToString
{
   [Fact]
   public void Should_return_string_representation_of_the_inner_value_having_2_types()
   {
      new TestUnion_class_string_int("text").ToString().Should().Be("text");
      new TestUnion_class_string_int(42).ToString().Should().Be("42");

      new TestUnion_struct_string_int("text").ToString().Should().Be("text");
      new TestUnion_struct_string_int(42).ToString().Should().Be("42");

      new TestUnion_class_with_array(["text"]).ToString().Should().Be("System.String[]");
      new TestUnion_class_with_array(42).ToString().Should().Be("42");
   }

   [Fact]
   public void Should_return_string_representation_of_the_inner_value_having_3_types()
   {
      new TestUnion_class_string_int_bool("text").ToString().Should().Be("text");
      new TestUnion_class_string_int_bool(42).ToString().Should().Be("42");
      new TestUnion_class_string_int_bool(true).ToString().Should().Be("True");
   }

   [Fact]
   public void Should_return_string_representation_of_the_inner_value_having_4_types()
   {
      new TestUnion_class_string_int_bool_guid("text").ToString().Should().Be("text");
      new TestUnion_class_string_int_bool_guid(42).ToString().Should().Be("42");
      new TestUnion_class_string_int_bool_guid(true).ToString().Should().Be("True");
      new TestUnion_class_string_int_bool_guid(new Guid("ED91613B-C9A5-4762-A5A7-A3F615F81CA6")).ToString().Should().Be("ed91613b-c9a5-4762-a5a7-a3f615f81ca6");
   }

   [Fact]
   public void Should_return_string_representation_of_the_inner_value_having_5_types()
   {
      new TestUnion_class_string_int_bool_guid_char("text").ToString().Should().Be("text");
      new TestUnion_class_string_int_bool_guid_char(42).ToString().Should().Be("42");
      new TestUnion_class_string_int_bool_guid_char(true).ToString().Should().Be("True");
      new TestUnion_class_string_int_bool_guid_char(new Guid("ED91613B-C9A5-4762-A5A7-A3F615F81CA6")).ToString().Should().Be("ed91613b-c9a5-4762-a5a7-a3f615f81ca6");
      new TestUnion_class_string_int_bool_guid_char('A').ToString().Should().Be("A");
   }
}
