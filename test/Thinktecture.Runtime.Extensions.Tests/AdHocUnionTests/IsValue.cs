#nullable enable
using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

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
      new TestUnion_class_nullable_string_nullable_int(@string: null).IsNullableOfInt32.Should().BeFalse();
      new TestUnion_class_nullable_string_nullable_int("text").IsString.Should().BeTrue();
      new TestUnion_class_nullable_string_nullable_int("text").IsNullableOfInt32.Should().BeFalse();
      new TestUnion_class_nullable_string_nullable_int(1).IsString.Should().BeFalse();
      new TestUnion_class_nullable_string_nullable_int(1).IsNullableOfInt32.Should().BeTrue();
      new TestUnion_class_nullable_string_nullable_int(nullableOfInt32: null).IsString.Should().BeFalse();
      new TestUnion_class_nullable_string_nullable_int(nullableOfInt32: null).IsNullableOfInt32.Should().BeTrue();

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
      TestUnion_class_with_same_types.CreateText("text").IsNullableOfInt32.Should().BeFalse();

      new TestUnion_class_with_same_types(1).IsText.Should().BeFalse();
      new TestUnion_class_with_same_types(1).IsInt32.Should().BeTrue();
      new TestUnion_class_with_same_types(1).IsString2.Should().BeFalse();
      new TestUnion_class_with_same_types(1).IsString3.Should().BeFalse();
      new TestUnion_class_with_same_types(1).IsNullableOfInt32.Should().BeFalse();

      TestUnion_class_with_same_types.CreateString2("text").IsText.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString2("text").IsInt32.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString2("text").IsString2.Should().BeTrue();
      TestUnion_class_with_same_types.CreateString2("text").IsString3.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString2("text").IsNullableOfInt32.Should().BeFalse();

      TestUnion_class_with_same_types.CreateString3("text").IsText.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString3("text").IsInt32.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString3("text").IsString2.Should().BeFalse();
      TestUnion_class_with_same_types.CreateString3("text").IsString3.Should().BeTrue();
      TestUnion_class_with_same_types.CreateString3("text").IsNullableOfInt32.Should().BeFalse();

      new TestUnion_class_with_same_types((int?)1).IsText.Should().BeFalse();
      new TestUnion_class_with_same_types((int?)1).IsInt32.Should().BeFalse();
      new TestUnion_class_with_same_types((int?)1).IsString2.Should().BeFalse();
      new TestUnion_class_with_same_types((int?)1).IsString3.Should().BeFalse();
      new TestUnion_class_with_same_types((int?)1).IsNullableOfInt32.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_stateless_type_T1()
   {
      // Marker type NullValue
      var union1 = new TestUnion_class_stateless_nullvaluestruct_string(new NullValueStruct());
      union1.IsNullValueStruct.Should().BeTrue();
      union1.IsString.Should().BeFalse();

      // Regular type string
      var union2 = new TestUnion_class_stateless_nullvaluestruct_string("text");
      union2.IsNullValueStruct.Should().BeFalse();
      union2.IsString.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_stateless_type_T2()
   {
      // Regular type string
      var union1 = new TestUnion_class_string_stateless_emptystatestruct("text");
      union1.IsString.Should().BeTrue();
      union1.IsEmptyStateStruct.Should().BeFalse();

      // Marker type EmptyState
      var union2 = new TestUnion_class_string_stateless_emptystatestruct(new EmptyStateStruct());
      union2.IsString.Should().BeFalse();
      union2.IsEmptyStateStruct.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_multiple_stateless_types()
   {
      // Marker type NullValue
      var union1 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new NullValueStruct());
      union1.IsNullValueStruct.Should().BeTrue();
      union1.IsEmptyStateStruct.Should().BeFalse();
      union1.IsString.Should().BeFalse();

      // Marker type EmptyState
      var union2 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new EmptyStateStruct());
      union2.IsNullValueStruct.Should().BeFalse();
      union2.IsEmptyStateStruct.Should().BeTrue();
      union2.IsString.Should().BeFalse();

      // Regular type string
      var union3 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string("text");
      union3.IsNullValueStruct.Should().BeFalse();
      union3.IsEmptyStateStruct.Should().BeFalse();
      union3.IsString.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_stateless_type_in_struct_union()
   {
      // Marker type NullValue
      var union1 = new TestUnion_struct_stateless_nullvaluestruct_int(new NullValueStruct());
      union1.IsNullValueStruct.Should().BeTrue();
      union1.IsInt32.Should().BeFalse();

      // Regular type int
      var union2 = new TestUnion_struct_stateless_nullvaluestruct_int(42);
      union2.IsNullValueStruct.Should().BeFalse();
      union2.IsInt32.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_nullable_struct_marker()
   {
      var union1 = new TestUnion_class_string_stateless_emptystatestruct_nullable("text");
      union1.IsString.Should().BeTrue();
      union1.IsNullableOfEmptyStateStruct.Should().BeFalse();

      var union2 = new TestUnion_class_string_stateless_emptystatestruct_nullable((EmptyStateStruct?)null);
      union2.IsString.Should().BeFalse();
      union2.IsNullableOfEmptyStateStruct.Should().BeTrue();

      var union3 = new TestUnion_class_string_stateless_emptystatestruct_nullable((EmptyStateStruct?)new EmptyStateStruct());
      union3.IsString.Should().BeFalse();
      union3.IsNullableOfEmptyStateStruct.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_reference_type_marker_T1()
   {
      var union1 = new TestUnion_class_stateless_nullvalueclass_string(new NullValueClass());
      union1.IsNullValueClass.Should().BeTrue();
      union1.IsString.Should().BeFalse();

      var union2 = new TestUnion_class_stateless_nullvalueclass_string("text");
      union2.IsNullValueClass.Should().BeFalse();
      union2.IsString.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_reference_type_marker_T2()
   {
      var union1 = new TestUnion_class_string_stateless_emptystateclass("text");
      union1.IsString.Should().BeTrue();
      union1.IsEmptyStateClass.Should().BeFalse();

      var union2 = new TestUnion_class_string_stateless_emptystateclass(new EmptyStateClass());
      union2.IsString.Should().BeFalse();
      union2.IsEmptyStateClass.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_multiple_reference_type_stateless()
   {
      var union1 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string(new NullValueClass());
      union1.IsNullValueClass.Should().BeTrue();
      union1.IsEmptyStateClass.Should().BeFalse();
      union1.IsString.Should().BeFalse();

      var union2 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string(new EmptyStateClass());
      union2.IsNullValueClass.Should().BeFalse();
      union2.IsEmptyStateClass.Should().BeTrue();
      union2.IsString.Should().BeFalse();

      var union3 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string("text");
      union3.IsNullValueClass.Should().BeFalse();
      union3.IsEmptyStateClass.Should().BeFalse();
      union3.IsString.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_duplicate_value_struct_stateless()
   {
      // First marker
      var union1 = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue1(default);
      union1.IsNullValue1.Should().BeTrue();
      union1.IsNullValue2.Should().BeFalse();
      union1.IsString.Should().BeFalse();

      // Second marker (same type as first)
      var union2 = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue2(default);
      union2.IsNullValue1.Should().BeFalse();
      union2.IsNullValue2.Should().BeTrue();
      union2.IsString.Should().BeFalse();

      // Regular type
      var union3 = new TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string("text");
      union3.IsNullValue1.Should().BeFalse();
      union3.IsNullValue2.Should().BeFalse();
      union3.IsString.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_duplicate_value_struct_stateless_T2_and_T3()
   {
      // Regular type
      var union1 = new TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct("text");
      union1.IsString.Should().BeTrue();
      union1.IsEmptyState1.Should().BeFalse();
      union1.IsEmptyState2.Should().BeFalse();

      // First marker
      var union2 = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState1(default);
      union2.IsString.Should().BeFalse();
      union2.IsEmptyState1.Should().BeTrue();
      union2.IsEmptyState2.Should().BeFalse();

      // Second marker (same type as first)
      var union3 = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState2(default);
      union3.IsString.Should().BeFalse();
      union3.IsEmptyState1.Should().BeFalse();
      union3.IsEmptyState2.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_duplicate_reference_type_stateless()
   {
      // First marker
      var union1 = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass1(null);
      union1.IsNullValueClass1.Should().BeTrue();
      union1.IsNullValueClass2.Should().BeFalse();
      union1.IsInt32.Should().BeFalse();

      // Second marker (same type as first)
      var union2 = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass2(null);
      union2.IsNullValueClass1.Should().BeFalse();
      union2.IsNullValueClass2.Should().BeTrue();
      union2.IsInt32.Should().BeFalse();

      // Regular type
      var union3 = new TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int(42);
      union3.IsNullValueClass1.Should().BeFalse();
      union3.IsNullValueClass2.Should().BeFalse();
      union3.IsInt32.Should().BeTrue();
   }

   [Fact]
   public void Should_correctly_identify_duplicate_markers_in_struct_union()
   {
      // First marker
      var union1 = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue1(default);
      union1.IsNullValue1.Should().BeTrue();
      union1.IsNullValue2.Should().BeFalse();
      union1.IsInt32.Should().BeFalse();

      // Second marker
      var union2 = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue2(default);
      union2.IsNullValue1.Should().BeFalse();
      union2.IsNullValue2.Should().BeTrue();
      union2.IsInt32.Should().BeFalse();

      // Regular type
      var union3 = new TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int(42);
      union3.IsNullValue1.Should().BeFalse();
      union3.IsNullValue2.Should().BeFalse();
      union3.IsInt32.Should().BeTrue();
   }
}
