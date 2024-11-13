using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

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
              TestUnion_class_with_same_types_case_sensitive.CreateText);
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
}
