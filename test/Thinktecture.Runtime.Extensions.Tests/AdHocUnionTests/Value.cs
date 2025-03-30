#nullable enable
using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

public class Value
{
   [Fact]
   public void Should_return_correct_value_having_2_types()
   {
      new TestUnion_class_string_int("text").Value.Should().Be("text");
      new TestUnion_class_string_int(1).Value.Should().Be(1);

      new TestUnion_class_nullable_string_int(@string: null).Value.Should().BeNull();
      new TestUnion_class_nullable_string_int("text").Value.Should().Be("text");
      new TestUnion_class_nullable_string_int(1).Value.Should().Be(1);

      new TestUnion_class_nullable_string_nullable_int(@string: null).Value.Should().BeNull();
      new TestUnion_class_nullable_string_nullable_int("text").Value.Should().Be("text");
      new TestUnion_class_nullable_string_nullable_int(1).Value.Should().Be(1);
      new TestUnion_class_nullable_string_nullable_int(nullableOfInt32: null).Value.Should().BeNull();

      new TestUnion_struct_string_int("text").Value.Should().Be("text");
      new TestUnion_struct_string_int(1).Value.Should().Be(1);

      new TestUnion_class_with_array(["text"]).Value.Should().BeEquivalentTo(new[] { "text" });
      new TestUnion_class_with_array(1).Value.Should().Be(1);
   }

   [Fact]
   public void Should_return_correct_value_having_3_types()
   {
      new TestUnion_class_string_int_bool("text").Value.Should().Be("text");
      new TestUnion_class_string_int_bool(1).Value.Should().Be(1);
      new TestUnion_class_string_int_bool(true).Value.Should().Be(true);
   }

   [Fact]
   public void Should_return_correct_value_having_4_types()
   {
      new TestUnion_class_string_int_bool_guid("text").Value.Should().Be("text");
      new TestUnion_class_string_int_bool_guid(1).Value.Should().Be(1);
      new TestUnion_class_string_int_bool_guid(true).Value.Should().Be(true);
      new TestUnion_class_string_int_bool_guid(new Guid("04F2DA71-1E0F-4AA4-AD1E-CE56BEDED52B")).Value.Should().Be(new Guid("04F2DA71-1E0F-4AA4-AD1E-CE56BEDED52B"));
   }

   [Fact]
   public void Should_return_correct_value_having_5_types()
   {
      new TestUnion_class_string_int_bool_guid_char("text").Value.Should().Be("text");
      new TestUnion_class_string_int_bool_guid_char(1).Value.Should().Be(1);
      new TestUnion_class_string_int_bool_guid_char(true).Value.Should().Be(true);
      new TestUnion_class_string_int_bool_guid_char(new Guid("04F2DA71-1E0F-4AA4-AD1E-CE56BEDED52B")).Value.Should().Be(new Guid("04F2DA71-1E0F-4AA4-AD1E-CE56BEDED52B"));
      new TestUnion_class_string_int_bool_guid_char('A').Value.Should().Be('A');
   }

   [Fact]
   public void Should_return_correct_value_having_5_types_with_duplicates()
   {
      TestUnion_class_with_same_types.CreateText("text").Value.Should().Be("text");
      new TestUnion_class_with_same_types(1).Value.Should().Be(1);
      TestUnion_class_with_same_types.CreateString2("text2").Value.Should().Be("text2");
      TestUnion_class_with_same_types.CreateString3("text3").Value.Should().Be("text3");
      new TestUnion_class_with_same_types((int?)2).Value.Should().Be((int?)2);
   }
}
