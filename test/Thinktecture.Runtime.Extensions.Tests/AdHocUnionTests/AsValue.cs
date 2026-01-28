#nullable enable
using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

public class AsValue
{
   [Fact]
   public void Should_return_correct_value_or_throw_exception_having_2_types()
   {
      new TestUnion_class_string_int("text").AsString.Should().Be("text");
      new TestUnion_class_string_int("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int' is not of type 'int'.");
      new TestUnion_class_string_int(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int' is not of type 'string'.");
      new TestUnion_class_string_int(1).AsInt32.Should().Be(1);

      new TestUnion_class_nullable_string_int(@string: null).AsString.Should().BeNull();
      new TestUnion_class_nullable_string_int(@string: null).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_int' is not of type 'int'.");
      new TestUnion_class_nullable_string_int("text").AsString.Should().Be("text");
      new TestUnion_class_nullable_string_int("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_int' is not of type 'int'.");
      new TestUnion_class_nullable_string_int(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_int' is not of type 'string?'.");
      new TestUnion_class_nullable_string_int(1).AsInt32.Should().Be(1);

      new TestUnion_class_nullable_string_nullable_int(@string: null).AsString.Should().BeNull();
      new TestUnion_class_nullable_string_nullable_int(@string: null).Invoking(u => u.AsNullableOfInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_nullable_int' is not of type 'int?'.");
      new TestUnion_class_nullable_string_nullable_int("text").AsString.Should().Be("text");
      new TestUnion_class_nullable_string_nullable_int("text").Invoking(u => u.AsNullableOfInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_nullable_int' is not of type 'int?'.");
      new TestUnion_class_nullable_string_nullable_int(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_nullable_int' is not of type 'string?'.");
      new TestUnion_class_nullable_string_nullable_int(1).AsNullableOfInt32.Should().Be(1);
      new TestUnion_class_nullable_string_nullable_int(nullableOfInt32: null).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_nullable_int' is not of type 'string?'.");
      new TestUnion_class_nullable_string_nullable_int(nullableOfInt32: null).AsNullableOfInt32.Should().BeNull();

      new TestUnion_struct_string_int("text").AsString.Should().Be("text");
      new TestUnion_struct_string_int("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_struct_string_int' is not of type 'int'.");
      new TestUnion_struct_string_int(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_struct_string_int' is not of type 'string'.");
      new TestUnion_struct_string_int(1).AsInt32.Should().Be(1);

      new TestUnion_class_with_array(["text"]).AsStringArray.Should().BeEquivalentTo(["text"]);
      new TestUnion_class_with_array(["text"]).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_array' is not of type 'int'.");
      new TestUnion_class_with_array(1).Invoking(u => u.AsStringArray.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_array' is not of type 'string[]'.");
      new TestUnion_class_with_array(1).AsInt32.Should().Be(1);
   }

   [Fact]
   public void Should_return_correct_value_or_throw_exception_having_3_types()
   {
      new TestUnion_class_string_int_bool("text").AsString.Should().Be("text");
      new TestUnion_class_string_int_bool("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'int'.");
      new TestUnion_class_string_int_bool("text").Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'bool'.");

      new TestUnion_class_string_int_bool(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'string'.");
      new TestUnion_class_string_int_bool(1).AsInt32.Should().Be(1);
      new TestUnion_class_string_int_bool(1).Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'bool'.");

      new TestUnion_class_string_int_bool(true).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'string'.");
      new TestUnion_class_string_int_bool(true).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'int'.");
      new TestUnion_class_string_int_bool(true).AsBoolean.Should().Be(true);
   }

   [Fact]
   public void Should_return_correct_value_or_throw_exception_having_4_types()
   {
      new TestUnion_class_string_int_bool_guid("text").AsString.Should().Be("text");
      new TestUnion_class_string_int_bool_guid("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid("text").Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid("text").Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'Guid'.");

      new TestUnion_class_string_int_bool_guid(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid(1).AsInt32.Should().Be(1);
      new TestUnion_class_string_int_bool_guid(1).Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid(1).Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'Guid'.");

      new TestUnion_class_string_int_bool_guid(true).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid(true).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid(true).AsBoolean.Should().Be(true);
      new TestUnion_class_string_int_bool_guid(true).Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'Guid'.");

      new TestUnion_class_string_int_bool_guid(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).AsGuid.Should().Be(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C"));
   }

   [Fact]
   public void Should_return_correct_value_or_throw_exception_having_5_types()
   {
      new TestUnion_class_string_int_bool_guid_char("text").AsString.Should().Be("text");
      new TestUnion_class_string_int_bool_guid_char("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid_char("text").Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid_char("text").Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid'.");
      new TestUnion_class_string_int_bool_guid_char("text").Invoking(u => u.AsChar.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char'.");

      new TestUnion_class_string_int_bool_guid_char(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid_char(1).AsInt32.Should().Be(1);
      new TestUnion_class_string_int_bool_guid_char(1).Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid_char(1).Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid'.");
      new TestUnion_class_string_int_bool_guid_char(1).Invoking(u => u.AsChar.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char'.");

      new TestUnion_class_string_int_bool_guid_char(true).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid_char(true).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid_char(true).AsBoolean.Should().Be(true);
      new TestUnion_class_string_int_bool_guid_char(true).Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid'.");
      new TestUnion_class_string_int_bool_guid_char(true).Invoking(u => u.AsChar.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char'.");

      new TestUnion_class_string_int_bool_guid_char(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid_char(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid_char(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid_char(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).AsGuid.Should().Be(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C"));
      new TestUnion_class_string_int_bool_guid_char(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsChar.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char'.");

      new TestUnion_class_string_int_bool_guid_char('A').Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid_char('A').Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid_char('A').Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid_char('A').Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid'.");
      new TestUnion_class_string_int_bool_guid_char('A').AsChar.Should().Be('A');
   }

   [Fact]
   public void Should_return_correct_value_or_throw_exception_having_5_types_with_duplicates()
   {
      TestUnion_class_with_same_types.CreateText("text").AsText.Should().Be("text");
      TestUnion_class_with_same_types.CreateText("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int'.");
      TestUnion_class_with_same_types.CreateText("text").Invoking(u => u.AsString2.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      TestUnion_class_with_same_types.CreateText("text").Invoking(u => u.AsString3.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string?'.");
      TestUnion_class_with_same_types.CreateText("text").Invoking(u => u.AsNullableOfInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int?'.");

      new TestUnion_class_with_same_types(1).Invoking(u => u.AsText.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      new TestUnion_class_with_same_types(1).AsInt32.Should().Be(1);
      new TestUnion_class_with_same_types(1).Invoking(u => u.AsString2.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      new TestUnion_class_with_same_types(1).Invoking(u => u.AsString3.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string?'.");
      new TestUnion_class_with_same_types(1).Invoking(u => u.AsNullableOfInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int?'.");

      TestUnion_class_with_same_types.CreateString2("text").Invoking(u => u.AsText.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      TestUnion_class_with_same_types.CreateString2("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int'.");
      TestUnion_class_with_same_types.CreateString2("text").AsString2.Should().Be("text");
      TestUnion_class_with_same_types.CreateString2("text").Invoking(u => u.AsString3.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string?'.");
      TestUnion_class_with_same_types.CreateString2("text").Invoking(u => u.AsNullableOfInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int?'.");

      TestUnion_class_with_same_types.CreateString3("text").Invoking(u => u.AsText.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      TestUnion_class_with_same_types.CreateString3("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int'.");
      TestUnion_class_with_same_types.CreateString3("text").Invoking(u => u.AsString2.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      TestUnion_class_with_same_types.CreateString3("text").AsString3.Should().Be("text");
      TestUnion_class_with_same_types.CreateString3("text").Invoking(u => u.AsNullableOfInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int?'.");

      new TestUnion_class_with_same_types((int?)1).Invoking(u => u.AsText.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      new TestUnion_class_with_same_types((int?)1).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int'.");
      new TestUnion_class_with_same_types((int?)1).Invoking(u => u.AsString2.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      new TestUnion_class_with_same_types((int?)1).Invoking(u => u.AsString3.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string?'.");
      new TestUnion_class_with_same_types((int?)1).AsNullableOfInt32.Should().Be(1);
   }

   [Fact]
   public void Should_return_default_for_stateless_type_T1()
   {
      // Marker type should return default(NullValueStruct)
      var union = new TestUnion_class_stateless_nullvaluestruct_string(new NullValueStruct());
      union.AsNullValueStruct.Should().Be(default(NullValueStruct));

      // Accessing wrong type should throw
      union.Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>()
           .WithMessage("'TestUnion_class_stateless_nullvaluestruct_string' is not of type 'string'.");
   }

   [Fact]
   public void Should_return_value_for_non_stateless_type()
   {
      var union = new TestUnion_class_stateless_nullvaluestruct_string("text");
      union.AsString.Should().Be("text");

      // Accessing marker type should throw
      union.Invoking(u => u.AsNullValueStruct.Should()).Should().Throw<InvalidOperationException>()
           .WithMessage("'TestUnion_class_stateless_nullvaluestruct_string' is not of type 'NullValueStruct'.");
   }

   [Fact]
   public void Should_return_default_for_stateless_type_T2()
   {
      // Regular type should return its value
      var union1 = new TestUnion_class_string_stateless_emptystatestruct("text");
      union1.AsString.Should().Be("text");

      // Marker type should return default(EmptyStateStruct)
      var union2 = new TestUnion_class_string_stateless_emptystatestruct(new EmptyStateStruct());
      union2.AsEmptyStateStruct.Should().Be(default(EmptyStateStruct));
   }

   [Fact]
   public void Should_return_default_for_multiple_stateless_types()
   {
      var union1 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new NullValueStruct());
      union1.AsNullValueStruct.Should().Be(default(NullValueStruct));

      var union2 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new EmptyStateStruct());
      union2.AsEmptyStateStruct.Should().Be(default(EmptyStateStruct));

      var union3 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string("text");
      union3.AsString.Should().Be("text");
   }

   [Fact]
   public void Should_return_default_for_stateless_type_in_struct_union()
   {
      var union1 = new TestUnion_struct_stateless_nullvaluestruct_int(new NullValueStruct());
      union1.AsNullValueStruct.Should().Be(default(NullValueStruct));

      var union2 = new TestUnion_struct_stateless_nullvaluestruct_int(42);
      union2.AsInt32.Should().Be(42);
   }

   [Fact]
   public void Should_return_default_for_nullable_struct_marker()
   {
      // Nullable struct marker should return null (default for nullable structs)
      var union1 = new TestUnion_class_string_stateless_emptystatestruct_nullable((EmptyStateStruct?)null);
      union1.AsNullableOfEmptyStateStruct.Should().BeNull();

      var union2 = new TestUnion_class_string_stateless_emptystatestruct_nullable((EmptyStateStruct?)new EmptyStateStruct());
      union2.AsNullableOfEmptyStateStruct.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_for_reference_type_marker_T1()
   {
      // Reference type marker should return null (default for reference types)
      var union = new TestUnion_class_stateless_nullvalueclass_string(new NullValueClass());
      union.AsNullValueClass.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_for_reference_type_marker_T2()
   {
      var union = new TestUnion_class_string_stateless_emptystateclass(new EmptyStateClass());
      union.AsEmptyStateClass.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_for_multiple_reference_type_stateless_1()
   {
      var union1 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string(new NullValueClass());
      union1.AsNullValueClass.Should().BeNull();

      var union2 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string(new EmptyStateClass());
      union2.AsEmptyStateClass.Should().BeNull();

      var union3 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string("text");
      union3.AsString.Should().Be("text");
   }

   [Fact]
   public void Should_return_default_for_duplicate_value_struct_stateless()
   {
      // Both stateless should return default(NullValueStruct)
      var union1 = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue1(default);
      union1.AsNullValue1.Should().Be(default(NullValueStruct));

      var union2 = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue2(default);
      union2.AsNullValue2.Should().Be(default(NullValueStruct));

      // Regular type should return its value
      var union3 = new TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string("text");
      union3.AsString.Should().Be("text");
   }

   [Fact]
   public void Should_return_default_for_duplicate_value_struct_markers_T2_and_T3()
   {
      var union1 = new TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct("text");
      union1.AsString.Should().Be("text");

      var union2 = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState1(default);
      union2.AsEmptyState1.Should().Be(default(EmptyStateStruct));

      var union3 = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState2(default);
      union3.AsEmptyState2.Should().Be(default(EmptyStateStruct));
   }

   [Fact]
   public void Should_return_null_for_multiple_reference_type_stateless()
   {
      // Both markers should return null
      var union1 = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass1(null);
      union1.AsNullValueClass1.Should().BeNull();

      var union2 = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass2(null);
      union2.AsNullValueClass2.Should().BeNull();

      // Regular type should return its value
      var union3 = new TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int(42);
      union3.AsInt32.Should().Be(42);
   }

   [Fact]
   public void Should_return_default_for_duplicate_markers_in_struct_union()
   {
      var union1 = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue1(default);
      union1.AsNullValue1.Should().Be(default(NullValueStruct));

      var union2 = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue2(default);
      union2.AsNullValue2.Should().Be(default(NullValueStruct));

      var union3 = new TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int(42);
      union3.AsInt32.Should().Be(42);
   }
}
