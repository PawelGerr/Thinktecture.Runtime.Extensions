using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable EqualExpressionComparison
// ReSharper disable ConditionIsAlwaysTrueOrFalse
public class EqualityOperator
{
   [Fact]
   public void Should_compare_unions_with_2_types()
   {
      Compare(s => new TestUnion_class_string_int(s),
              n => new TestUnion_class_string_int(n),
              s => new TestUnion_class_string_int_case_sensitive(s));

      Compare(s => new TestUnion_struct_string_int(s),
              n => new TestUnion_struct_string_int(n),
              s => new TestUnion_struct_string_int_case_sensitive(s));
   }

   [Fact]
   public void Should_compare_unions_with_3_types()
   {
      Compare(s => new TestUnion_class_string_int_bool(s),
              n => new TestUnion_class_string_int_bool(n),
              s => new TestUnion_class_string_int_bool_case_sensitive(s));
   }

   [Fact]
   public void Should_compare_unions_with_4_types()
   {
      Compare(s => new TestUnion_class_string_int_bool_guid(s),
              n => new TestUnion_class_string_int_bool_guid(n),
              s => new TestUnion_class_string_int_bool_guid_case_sensitive(s));
   }

   [Fact]
   public void Should_compare_unions_with_5_types()
   {
      Compare(s => new TestUnion_class_string_int_bool_guid_char(s),
              n => new TestUnion_class_string_int_bool_guid_char(n),
              s => new TestUnion_class_string_int_bool_guid_char_case_sensitive(s));
   }

   [Fact]
   public void Should_compare_unions_with_5_types_with_duplicates()
   {
      Compare(TestUnion_class_with_same_types.CreateText,
              n => new TestUnion_class_with_same_types(n),
              TestAdHocUnions.NonGeneric.TestUnion_class_with_same_types_case_sensitive.CreateText);
   }

   private static void Compare<T, T2>(
      Func<string, T> stringFactory,
      Func<int, T> intFactory,
      Func<string, T2> caseSensitiveFactory)
      where T : System.Numerics.IEqualityOperators<T, T, bool>
      where T2 : System.Numerics.IEqualityOperators<T2, T2, bool>
   {
      var obj = stringFactory("text");

      (obj == null).Should().BeFalse();
      (obj == obj).Should().BeTrue();
      (obj == stringFactory("text")).Should().BeTrue();
      (obj == stringFactory("TEXT")).Should().BeTrue();
      (caseSensitiveFactory("text") == caseSensitiveFactory("TEXT")).Should().BeFalse();
      (obj == stringFactory("other text")).Should().BeFalse();
      (stringFactory("42") == intFactory(42)).Should().BeFalse();
   }

   [Fact]
   public void Should_use_equality_operator_with_duplicate_value_struct_stateless()
   {
      var union1a = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue1(default);
      var union1b = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue1(default);

      // Same marker index should be equal
      (union1a == union1b).Should().BeTrue();

      var union2a = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue2(default);
      var union2b = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue2(default);

      // Same marker index should be equal
      (union2a == union2b).Should().BeTrue();

      // Different marker indices should NOT be equal
      (union1a == union2a).Should().BeFalse();
      (union1a != union2a).Should().BeTrue();
   }

   [Fact]
   public void Should_use_equality_operator_with_duplicate_value_struct_stateless_T2_and_T3()
   {
      var union1a = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState1(default);
      var union1b = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState1(default);

      (union1a == union1b).Should().BeTrue();

      var union2a = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState2(default);
      var union2b = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState2(default);

      (union2a == union2b).Should().BeTrue();

      // Different marker indices should NOT be equal
      (union1a == union2a).Should().BeFalse();
      (union1a != union2a).Should().BeTrue();
   }

   [Fact]
   public void Should_use_equality_operator_with_duplicate_reference_type_stateless()
   {
      var union1a = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass1(null);
      var union1b = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass1(null);

      // Same marker index should be equal
      (union1a == union1b).Should().BeTrue();

      var union2a = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass2(null);
      var union2b = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass2(null);

      // Same marker index should be equal
      (union2a == union2b).Should().BeTrue();

      // Different marker indices should NOT be equal
      (union1a == union2a).Should().BeFalse();
      (union1a != union2a).Should().BeTrue();
   }

   [Fact]
   public void Should_use_equality_operator_with_duplicate_markers_in_struct_union()
   {
      var union1a = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue1(default);
      var union1b = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue1(default);

      (union1a == union1b).Should().BeTrue();

      var union2a = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue2(default);
      var union2b = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue2(default);

      (union2a == union2b).Should().BeTrue();

      // Different marker indices should NOT be equal
      (union1a == union2a).Should().BeFalse();
      (union1a != union2a).Should().BeTrue();
   }
}
