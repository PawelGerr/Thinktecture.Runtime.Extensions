using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// ReSharper disable once InconsistentNaming
public class Map
{
   public class HavingClass
   {
      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      public void Should_use_correct_arg_having_2_values(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int("text"),
            2 => new TestUnion_class_string_int(42),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(@string: (object)"text",
                                        int32: 42);

         calledActionOn.Should().Be(expected);
      }

#if NET9_0_OR_GREATER
      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      public void Should_use_correct_arg_having_2_values_returning_ref_struct(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int("text"),
            2 => new TestUnion_class_string_int(42),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(@string: new TestRefStruct("text"),
                                        int32: new TestRefStruct(42));

         calledActionOn.Value.Should().Be(expected);
      }
#endif

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      public void Should_use_correct_arg_having_2_values_with_array(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_with_array(["text"]),
            2 => new TestUnion_class_with_array(42),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(stringArray: (object)"text",
                                        int32: 42);

         calledActionOn.Should().Be(expected);
      }

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      public void Should_use_correct_arg_having_3_values(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool("text"),
            2 => new TestUnion_class_string_int_bool(42),
            3 => new TestUnion_class_string_int_bool(true),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(@string: (object)"text",
                                        int32: 42,
                                        boolean: true);

         calledActionOn.Should().Be(expected);
      }

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      [InlineData(4, "4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B")]
      public void Should_use_correct_arg_having_4_values(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool_guid("text"),
            2 => new TestUnion_class_string_int_bool_guid(42),
            3 => new TestUnion_class_string_int_bool_guid(true),
            4 => new TestUnion_class_string_int_bool_guid(new Guid("4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B")),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(@string: (object)"text",
                                        int32: 42,
                                        boolean: true,
                                        guid: new Guid("4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B"));

         calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
      }

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      [InlineData(4, "4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B")]
      [InlineData(5, 'A')]
      public void Should_use_correct_arg_having_5_values(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool_guid_char("text"),
            2 => new TestUnion_class_string_int_bool_guid_char(42),
            3 => new TestUnion_class_string_int_bool_guid_char(true),
            4 => new TestUnion_class_string_int_bool_guid_char(new Guid("4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B")),
            5 => new TestUnion_class_string_int_bool_guid_char('A'),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(@string: (object)"text",
                                        int32: 42,
                                        boolean: true,
                                        guid: new Guid("4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B"),
                                        @char: 'A');

         calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
      }

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      [InlineData(3, "text2")]
      [InlineData(4, "text3")]
      [InlineData(5, 43)]
      public void Should_use_correct_arg_having_5_values_with_duplicates(int index, object expected)
      {
         var value = index switch
         {
            1 => TestUnion_class_with_same_types.CreateText("text"),
            2 => new TestUnion_class_with_same_types(42),
            3 => TestUnion_class_with_same_types.CreateString2("text2"),
            4 => TestUnion_class_with_same_types.CreateString3("text3"),
            5 => new TestUnion_class_with_same_types((int?)43),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(text: (object)"text",
                                        int32: 42,
                                        string2: "text2",
                                        string3: "text3",
                                        nullableOfInt32: 43);

         calledActionOn.Should().Be(expected);
      }

      [Fact]
      public void Should_map_correctly_with_stateless_type_T1()
      {
         var union1 = new TestUnion_class_stateless_nullvaluestruct_string(new NullValueStruct());
         var result1 = union1.Map(
            nullValueStruct: "marker",
            @string: "text");
         result1.Should().Be("marker");

         var union2 = new TestUnion_class_stateless_nullvaluestruct_string("actual_text");
         var result2 = union2.Map(
            nullValueStruct: "marker",
            @string: "text");
         result2.Should().Be("text");
      }

      [Fact]
      public void Should_map_correctly_with_stateless_type_T2()
      {
         var union1 = new TestUnion_class_string_stateless_emptystatestruct("actual_text");
         var result1 = union1.Map(
            @string: "text",
            emptyStateStruct: "marker");
         result1.Should().Be("text");

         var union2 = new TestUnion_class_string_stateless_emptystatestruct(new EmptyStateStruct());
         var result2 = union2.Map(
            @string: "text",
            emptyStateStruct: "marker");
         result2.Should().Be("marker");
      }

      [Fact]
      public void Should_map_correctly_with_multiple_stateless_types()
      {
         var union1 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new NullValueStruct());
         var result1 = union1.Map(
            nullValueStruct: "null",
            emptyStateStruct: "empty",
            @string: "text");
         result1.Should().Be("null");

         var union2 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new EmptyStateStruct());
         var result2 = union2.Map(
            nullValueStruct: "null",
            emptyStateStruct: "empty",
            @string: "text");
         result2.Should().Be("empty");

         var union3 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string("actual_text");
         var result3 = union3.Map(
            nullValueStruct: "null",
            emptyStateStruct: "empty",
            @string: "text");
         result3.Should().Be("text");
      }

      [Fact]
      public void Should_map_correctly_with_nullable_struct_marker()
      {
         var union1 = new TestUnion_class_string_stateless_emptystatestruct_nullable("actual");
         var result1 = union1.Map(
            @string: "text",
            nullableOfEmptyStateStruct: "marker");
         result1.Should().Be("text");

         var union2 = new TestUnion_class_string_stateless_emptystatestruct_nullable((EmptyStateStruct?)null);
         var result2 = union2.Map(
            @string: "text",
            nullableOfEmptyStateStruct: "marker");
         result2.Should().Be("marker");

         var union3 = new TestUnion_class_string_stateless_emptystatestruct_nullable((EmptyStateStruct?)new EmptyStateStruct());
         var result3 = union3.Map(
            @string: "text",
            nullableOfEmptyStateStruct: "marker");
         result3.Should().Be("marker");
      }

      [Fact]
      public void Should_map_correctly_with_reference_type_marker()
      {
         var union1 = new TestUnion_class_stateless_nullvalueclass_string(new NullValueClass());
         var result1 = union1.Map(
            nullValueClass: "marker",
            @string: "text");
         result1.Should().Be("marker");

         var union2 = new TestUnion_class_stateless_nullvalueclass_string("actual");
         var result2 = union2.Map(
            nullValueClass: "marker",
            @string: "text");
         result2.Should().Be("text");
      }

      [Fact]
      public void Should_map_correctly_with_multiple_reference_type_stateless()
      {
         var union1 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string(new NullValueClass());
         var result1 = union1.Map(
            nullValueClass: "null",
            emptyStateClass: "empty",
            @string: "text");
         result1.Should().Be("null");

         var union2 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string(new EmptyStateClass());
         var result2 = union2.Map(
            nullValueClass: "null",
            emptyStateClass: "empty",
            @string: "text");
         result2.Should().Be("empty");

         var union3 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string("text");
         var result3 = union3.Map(
            nullValueClass: "null",
            emptyStateClass: "empty",
            @string: "text");
         result3.Should().Be("text");
      }

      [Fact]
      public void Should_map_correctly_with_duplicate_value_struct_stateless()
      {
         var union1 = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue1(default);
         var result1 = union1.Map(
            nullValue1: "marker1",
            nullValue2: "marker2",
            @string: "text");
         result1.Should().Be("marker1");

         var union2 = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue2(default);
         var result2 = union2.Map(
            nullValue1: "marker1",
            nullValue2: "marker2",
            @string: "text");
         result2.Should().Be("marker2");

         var union3 = new TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string("actual");
         var result3 = union3.Map(
            nullValue1: "marker1",
            nullValue2: "marker2",
            @string: "text");
         result3.Should().Be("text");
      }

      [Fact]
      public void Should_map_correctly_with_duplicate_reference_type_stateless()
      {
         var union1 = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass1(null);
         var result1 = union1.Map<object>(
            nullValueClass1: "marker1",
            nullValueClass2: "marker2",
            int32: 100);
         result1.Should().Be("marker1");

         var union2 = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass2(null);
         var result2 = union2.Map<object>(
            nullValueClass1: "marker1",
            nullValueClass2: "marker2",
            int32: 100);
         result2.Should().Be("marker2");

         var union3 = new TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int(42);
         var result3 = union3.Map<object>(
            nullValueClass1: "marker1",
            nullValueClass2: "marker2",
            int32: 100);
         result3.Should().Be(100);
      }
   }

   public class HavingStruct
   {
      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      public void Should_use_correct_arg_having_2_values(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_struct_string_int("text"),
            2 => new TestUnion_struct_string_int(42),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(@string: (object)"text",
                                        int32: 42);

         calledActionOn.Should().Be(expected);
      }

      [Fact]
      public void Should_map_correctly_with_stateless_type_in_struct_union()
      {
         var union1 = new TestUnion_struct_stateless_nullvaluestruct_int(new NullValueStruct());
         var result1 = union1.Map(
            nullValueStruct: 0,
            int32: 100);
         result1.Should().Be(0);

         var union2 = new TestUnion_struct_stateless_nullvaluestruct_int(42);
         var result2 = union2.Map(
            nullValueStruct: 0,
            int32: 100);
         result2.Should().Be(100);
      }

      [Fact]
      public void Should_map_correctly_with_duplicate_markers_in_struct_union()
      {
         var union1 = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue1(default);
         var result1 = union1.Map(
            nullValue1: 0,
            nullValue2: -1,
            int32: 100);
         result1.Should().Be(0);

         var union2 = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue2(default);
         var result2 = union2.Map(
            nullValue1: 0,
            nullValue2: -1,
            int32: 100);
         result2.Should().Be(-1);

         var union3 = new TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int(42);
         var result3 = union3.Map(
            nullValue1: 0,
            nullValue2: -1,
            int32: 100);
         result3.Should().Be(100);
      }
   }
}
