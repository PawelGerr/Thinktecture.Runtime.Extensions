using System;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable EqualExpressionComparison
// ReSharper disable ConditionIsAlwaysTrueOrFalse
public class InequalityOperator
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
              TestUnion_class_with_same_types_case_sensitive.CreateText);
   }

   private static void Compare<T, T2>(
      Func<string, T> stringFactory,
      Func<int, T> intFactory,
      Func<string, T2> caseSensitiveFactory)
      where T : System.Numerics.IEqualityOperators<T, T, bool>
      where T2 : System.Numerics.IEqualityOperators<T2, T2, bool>
   {
      var obj = stringFactory("text");

      (obj != null).Should().BeTrue();
      (obj != obj).Should().BeFalse();
      (obj != stringFactory("text")).Should().BeFalse();
      (obj != stringFactory("TEXT")).Should().BeFalse();
      (caseSensitiveFactory("text") != caseSensitiveFactory("TEXT")).Should().BeTrue();
      (obj != stringFactory("other text")).Should().BeTrue();
      (stringFactory("42") != intFactory(42)).Should().BeTrue();
   }
}
