using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable EqualExpressionComparison
// ReSharper disable ConditionIsAlwaysTrueOrFalse
public class Equals
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
      where T : IEquatable<T>
      where T2 : IEquatable<T2>
   {
      var obj = stringFactory("text");

      obj.Equals((TestUnion_class_string_int)null).Should().BeFalse();
      obj.Equals((object)null).Should().BeFalse();

      obj.Equals(obj).Should().BeTrue();
      obj.Equals((object)obj).Should().BeTrue();

      obj.Equals(stringFactory("text")).Should().BeTrue();
      obj.Equals((object)stringFactory("text")).Should().BeTrue();

      obj.Equals(stringFactory("TEXT")).Should().BeTrue();
      obj.Equals((object)stringFactory("TEXT")).Should().BeTrue();

      caseSensitiveFactory("text").Equals(caseSensitiveFactory("TEXT")).Should().BeFalse();
      caseSensitiveFactory("text").Equals((object)caseSensitiveFactory("TEXT")).Should().BeFalse();

      obj.Equals(stringFactory("other text")).Should().BeFalse();
      obj.Equals((object)stringFactory("other text")).Should().BeFalse();

      stringFactory("42").Equals(intFactory(42)).Should().BeFalse();
      stringFactory("42").Equals((object)intFactory(42)).Should().BeFalse();

      stringFactory("42").Equals((object)"42").Should().BeFalse();
      stringFactory("42").Equals((object)42).Should().BeFalse();
      stringFactory("42").Equals(caseSensitiveFactory("42")).Should().BeFalse();
      stringFactory("42").Equals((object)caseSensitiveFactory("42")).Should().BeFalse();
   }

   [Fact]
   public void Should_consider_equal_when_both_are_same_stateless_type()
   {
      var union1 = new TestUnion_class_stateless_nullvaluestruct_string(new NullValueStruct());
      var union2 = new TestUnion_class_stateless_nullvaluestruct_string(new NullValueStruct());

      union1.Equals(union2).Should().BeTrue();
      (union1 == union2).Should().BeTrue();
      (union1 != union2).Should().BeFalse();
      union1.GetHashCode().Should().Be(union2.GetHashCode());
   }

   [Fact]
   public void Should_consider_not_equal_when_different_types()
   {
      var union1 = new TestUnion_class_stateless_nullvaluestruct_string(new NullValueStruct());
      var union2 = new TestUnion_class_stateless_nullvaluestruct_string("text");

      union1.Equals(union2).Should().BeFalse();
      (union1 == union2).Should().BeFalse();
      (union1 != union2).Should().BeTrue();
   }

   [Fact]
   public void Should_consider_equal_when_same_regular_type_value()
   {
      var union1 = new TestUnion_class_stateless_nullvaluestruct_string("text");
      var union2 = new TestUnion_class_stateless_nullvaluestruct_string("text");

      union1.Equals(union2).Should().BeTrue();
      (union1 == union2).Should().BeTrue();
      (union1 != union2).Should().BeFalse();
      union1.GetHashCode().Should().Be(union2.GetHashCode());
   }

   [Fact]
   public void Should_handle_equality_with_multiple_stateless_types()
   {
      var union1 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new NullValueStruct());
      var union2 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new NullValueStruct());
      union1.Equals(union2).Should().BeTrue();

      var union3 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new EmptyStateStruct());
      var union4 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new EmptyStateStruct());
      union3.Equals(union4).Should().BeTrue();

      union1.Equals(union3).Should().BeFalse();
   }

   [Fact]
   public void Should_handle_equality_in_struct_union()
   {
      var union1 = new TestUnion_struct_stateless_nullvaluestruct_int(new NullValueStruct());
      var union2 = new TestUnion_struct_stateless_nullvaluestruct_int(new NullValueStruct());

      union1.Equals(union2).Should().BeTrue();
      (union1 == union2).Should().BeTrue();
      (union1 != union2).Should().BeFalse();
      union1.GetHashCode().Should().Be(union2.GetHashCode());
   }

   [Fact]
   public void Should_handle_equality_with_nullable_struct_marker()
   {
      var union1 = new TestUnion_class_string_stateless_emptystatestruct_nullable((EmptyStateStruct?)null);
      var union2 = new TestUnion_class_string_stateless_emptystatestruct_nullable((EmptyStateStruct?)new EmptyStateStruct());

      // Both are markers, so they should be equal
      union1.Equals(union2).Should().BeTrue();
      (union1 == union2).Should().BeTrue();
      union1.GetHashCode().Should().Be(union2.GetHashCode());
   }

   [Fact]
   public void Should_not_equal_when_marker_vs_regular_type_with_nullable_struct()
   {
      var union1 = new TestUnion_class_string_stateless_emptystatestruct_nullable((EmptyStateStruct?)null);
      var union2 = new TestUnion_class_string_stateless_emptystatestruct_nullable("text");

      union1.Equals(union2).Should().BeFalse();
      (union1 == union2).Should().BeFalse();
      (union1 != union2).Should().BeTrue();
   }

   [Fact]
   public void Should_handle_equality_with_reference_type_marker()
   {
      var union1 = new TestUnion_class_stateless_nullvalueclass_string(new NullValueClass());
      var union2 = new TestUnion_class_stateless_nullvalueclass_string(new NullValueClass());

      // Both are markers, so they should be equal (both return null)
      union1.Equals(union2).Should().BeTrue();
      (union1 == union2).Should().BeTrue();
      union1.GetHashCode().Should().Be(union2.GetHashCode());
   }

   [Fact]
   public void Should_not_equal_when_reference_marker_vs_regular_type()
   {
      var union1 = new TestUnion_class_stateless_nullvalueclass_string(new NullValueClass());
      var union2 = new TestUnion_class_stateless_nullvalueclass_string("text");

      union1.Equals(union2).Should().BeFalse();
      (union1 == union2).Should().BeFalse();
      (union1 != union2).Should().BeTrue();
   }

   [Fact]
   public void Should_handle_equality_with_duplicate_value_struct_stateless()
   {
      var union1a = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue1(default);
      var union1b = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue1(default);

      // Same marker index should be equal
      union1a.Equals(union1b).Should().BeTrue();

      var union2a = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue2(default);
      var union2b = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue2(default);

      // Same marker index should be equal
      union2a.Equals(union2b).Should().BeTrue();

      // Different marker indices should NOT be equal (even though same underlying type)
      union1a.Equals(union2a).Should().BeFalse();
   }

   [Fact]
   public void Should_handle_equality_with_duplicate_value_struct_stateless_T2_and_T3()
   {
      var union1a = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState1(default);
      var union1b = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState1(default);

      union1a.Equals(union1b).Should().BeTrue();

      var union2a = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState2(default);
      var union2b = TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct.CreateEmptyState2(default);

      union2a.Equals(union2b).Should().BeTrue();

      // Different marker indices should NOT be equal
      union1a.Equals(union2a).Should().BeFalse();
   }

   [Fact]
   public void Should_handle_equality_with_duplicate_reference_type_stateless()
   {
      var union1a = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass1(null);
      var union1b = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass1(null);

      // Same marker index should be equal (both return null)
      union1a.Equals(union1b).Should().BeTrue();

      var union2a = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass2(null);
      var union2b = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass2(null);

      // Same marker index should be equal (both return null)
      union2a.Equals(union2b).Should().BeTrue();

      // Different marker indices should NOT be equal (even though both return null)
      union1a.Equals(union2a).Should().BeFalse();
   }

   [Fact]
   public void Should_handle_equality_with_duplicate_markers_in_struct_union()
   {
      var union1a = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue1(default);
      var union1b = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue1(default);

      union1a.Equals(union1b).Should().BeTrue();

      var union2a = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue2(default);
      var union2b = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue2(default);

      union2a.Equals(union2b).Should().BeTrue();

      // Different marker indices should NOT be equal
      union1a.Equals(union2a).Should().BeFalse();
   }
}
