using System;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

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
}
