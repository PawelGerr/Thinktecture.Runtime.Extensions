#nullable enable
using System;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

public class IsValue
{
   [Fact]
   public void Should_use_correct_index_having_2_types()
   {
      new TestUnion_class_string_int("text").IsString.Should().BeTrue();
      new TestUnion_class_string_int("text").IsInt32.Should().BeFalse();
      new TestUnion_class_string_int(1).IsString.Should().BeFalse();
      new TestUnion_class_string_int(1).IsInt32.Should().BeTrue();

      new TestUnion_class_nullable_string_int(@string: null).IsString.Should().BeTrue();
      new TestUnion_class_nullable_string_int(@string: null).IsInt32.Should().BeFalse();
      new TestUnion_class_nullable_string_int("text").IsString.Should().BeTrue();
      new TestUnion_class_nullable_string_int("text").IsInt32.Should().BeFalse();
      new TestUnion_class_nullable_string_int(1).IsString.Should().BeFalse();
      new TestUnion_class_nullable_string_int(1).IsInt32.Should().BeTrue();

      new TestUnion_class_nullable_string_nullable_int(@string: null).IsString.Should().BeTrue();
      new TestUnion_class_nullable_string_nullable_int(@string: null).IsNullableInt32.Should().BeFalse();
      new TestUnion_class_nullable_string_nullable_int("text").IsString.Should().BeTrue();
      new TestUnion_class_nullable_string_nullable_int("text").IsNullableInt32.Should().BeFalse();
      new TestUnion_class_nullable_string_nullable_int(1).IsString.Should().BeFalse();
      new TestUnion_class_nullable_string_nullable_int(1).IsNullableInt32.Should().BeTrue();
      new TestUnion_class_nullable_string_nullable_int(nullableInt32: null).IsString.Should().BeFalse();
      new TestUnion_class_nullable_string_nullable_int(nullableInt32: null).IsNullableInt32.Should().BeTrue();

      new TestUnion_struct_string_int("text").IsString.Should().BeTrue();
      new TestUnion_struct_string_int("text").IsInt32.Should().BeFalse();
      new TestUnion_struct_string_int(1).IsString.Should().BeFalse();
      new TestUnion_struct_string_int(1).IsInt32.Should().BeTrue();

      new TestUnion_class_with_array(["text"]).IsStringArray.Should().BeTrue();
      new TestUnion_class_with_array(["text"]).IsInt32.Should().BeFalse();
      new TestUnion_class_with_array(1).IsStringArray.Should().BeFalse();
      new TestUnion_class_with_array(1).IsInt32.Should().BeTrue();
   }

   [Fact]
   public void Should_use_correct_index_having_3_types()
   {
      new TestUnion_class_string_int_bool("text").IsString.Should().BeTrue();
      new TestUnion_class_string_int_bool("text").IsInt32.Should().BeFalse();
      new TestUnion_class_string_int_bool("text").IsBoolean.Should().BeFalse();

      new TestUnion_class_string_int_bool(1).IsString.Should().BeFalse();
      new TestUnion_class_string_int_bool(1).IsInt32.Should().BeTrue();
      new TestUnion_class_string_int_bool(1).IsBoolean.Should().BeFalse();

      new TestUnion_class_string_int_bool(true).IsString.Should().BeFalse();
      new TestUnion_class_string_int_bool(true).IsInt32.Should().BeFalse();
      new TestUnion_class_string_int_bool(true).IsBoolean.Should().BeTrue();
   }

   [Fact]
   public void Should_use_correct_index_having_4_types()
   {
      new TestUnion_class_string_int_bool_guid("text").IsString.Should().BeTrue();
      new TestUnion_class_string_int_bool_guid("text").IsInt32.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid("text").IsBoolean.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid("text").IsGuid.Should().BeFalse();

      new TestUnion_class_string_int_bool_guid(1).IsString.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid(1).IsInt32.Should().BeTrue();
      new TestUnion_class_string_int_bool_guid(1).IsBoolean.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid(1).IsGuid.Should().BeFalse();

      new TestUnion_class_string_int_bool_guid(true).IsString.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid(true).IsInt32.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid(true).IsBoolean.Should().BeTrue();
      new TestUnion_class_string_int_bool_guid(true).IsGuid.Should().BeFalse();

      new TestUnion_class_string_int_bool_guid(new Guid("4161A501-D501-462E-85B5-034960B133D6")).IsString.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid(new Guid("4161A501-D501-462E-85B5-034960B133D6")).IsInt32.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid(new Guid("4161A501-D501-462E-85B5-034960B133D6")).IsBoolean.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid(new Guid("4161A501-D501-462E-85B5-034960B133D6")).IsGuid.Should().BeTrue();
   }

   [Fact]
   public void Should_use_correct_index_having_5_types()
   {
      new TestUnion_class_string_int_bool_guid_char("text").IsString.Should().BeTrue();
      new TestUnion_class_string_int_bool_guid_char("text").IsInt32.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char("text").IsBoolean.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char("text").IsGuid.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char("text").IsChar.Should().BeFalse();

      new TestUnion_class_string_int_bool_guid_char(1).IsString.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char(1).IsInt32.Should().BeTrue();
      new TestUnion_class_string_int_bool_guid_char(1).IsBoolean.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char(1).IsGuid.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char(1).IsChar.Should().BeFalse();

      new TestUnion_class_string_int_bool_guid_char(true).IsString.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char(true).IsInt32.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char(true).IsBoolean.Should().BeTrue();
      new TestUnion_class_string_int_bool_guid_char(true).IsGuid.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char(true).IsChar.Should().BeFalse();

      new TestUnion_class_string_int_bool_guid_char(new Guid("4161A501-D501-462E-85B5-034960B133D6")).IsString.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char(new Guid("4161A501-D501-462E-85B5-034960B133D6")).IsInt32.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char(new Guid("4161A501-D501-462E-85B5-034960B133D6")).IsBoolean.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char(new Guid("4161A501-D501-462E-85B5-034960B133D6")).IsGuid.Should().BeTrue();
      new TestUnion_class_string_int_bool_guid_char(new Guid("4161A501-D501-462E-85B5-034960B133D6")).IsChar.Should().BeFalse();

      new TestUnion_class_string_int_bool_guid_char('A').IsString.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char('A').IsInt32.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char('A').IsBoolean.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char('A').IsGuid.Should().BeFalse();
      new TestUnion_class_string_int_bool_guid_char('A').IsChar.Should().BeTrue();
   }

   [Fact]
   public void Should_use_correct_index_having_5_types_with_duplicates()
   {
      TestUnion_class_with_same_types.CreateText("text").IsText.Should().BeTrue();
      TestUnion_class_with_same_types.CreateText("text").IsInt32.Should().BeFalse();
      TestUnion_class_with_same_types.CreateText("text").IsString2.Should().BeFalse();
      TestUnion_class_with_same_types.CreateText("text").IsString3.Should().BeFalse();
      TestUnion_class_with_same_types.CreateText("text").IsNullableInt32.Should().BeFalse();

      new TestUnion_class_with_same_types(1).IsText.Should().BeFalse();
      new TestUnion_class_with_same_types(1).IsInt32.Should().BeTrue();
      new TestUnion_class_with_same_types(1).IsString2.Should().BeFalse();
      new TestUnion_class_with_same_types(1).IsString3.Should().BeFalse();
      new TestUnion_class_with_same_types(1).IsNullableInt32.Should().BeFalse();

      TestUnion_class_with_same_types.CreateString2("text").IsText.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString2("text").IsInt32.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString2("text").IsString2.Should().BeTrue();
      TestUnion_class_with_same_types.CreateString2("text").IsString3.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString2("text").IsNullableInt32.Should().BeFalse();

      TestUnion_class_with_same_types.CreateString3("text").IsText.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString3("text").IsInt32.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString3("text").IsString2.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString3("text").IsString3.Should().BeTrue();
      TestUnion_class_with_same_types.CreateString3("text").IsNullableInt32.Should().BeFalse();

      new TestUnion_class_with_same_types((int?)1).IsText.Should().BeFalse();
      new TestUnion_class_with_same_types((int?)1).IsInt32.Should().BeFalse();
      new TestUnion_class_with_same_types((int?)1).IsString2.Should().BeFalse();
      new TestUnion_class_with_same_types((int?)1).IsString3.Should().BeFalse();
      new TestUnion_class_with_same_types((int?)1).IsNullableInt32.Should().BeTrue();
   }
}
