using System;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_hashcode_of_the_type_plus_inner_value()
   {
      ComputeHashCode(new TestUnion_class_string_int("text"), "text");
      ComputeHashCode(new TestUnion_class_string_int("text"), "TEXT");
      ComputeHashCode(new TestUnion_class_string_int(42), 42);

      ComputeHashCode(new TestUnion_class_string_int_bool("text"), "text");
      ComputeHashCode(new TestUnion_class_string_int_bool("text"), "TEXT");
      ComputeHashCode(new TestUnion_class_string_int_bool(42), 42);
      ComputeHashCode(new TestUnion_class_string_int_bool(true), true);

      ComputeHashCode(new TestUnion_class_string_int_bool_guid("text"), "text");
      ComputeHashCode(new TestUnion_class_string_int_bool_guid("text"), "TEXT");
      ComputeHashCode(new TestUnion_class_string_int_bool_guid(42), 42);
      ComputeHashCode(new TestUnion_class_string_int_bool_guid(true), true);
      ComputeHashCode(new TestUnion_class_string_int_bool_guid(new Guid("15A033FD-5887-465C-97E9-72DBE78AD02C")), new Guid("15A033FD-5887-465C-97E9-72DBE78AD02C"));

      ComputeHashCode(new TestUnion_class_string_int_bool_guid_char("text"), "text");
      ComputeHashCode(new TestUnion_class_string_int_bool_guid_char("text"), "TEXT");
      ComputeHashCode(new TestUnion_class_string_int_bool_guid_char(42), 42);
      ComputeHashCode(new TestUnion_class_string_int_bool_guid_char(true), true);
      ComputeHashCode(new TestUnion_class_string_int_bool_guid_char(new Guid("15A033FD-5887-465C-97E9-72DBE78AD02C")), new Guid("15A033FD-5887-465C-97E9-72DBE78AD02C"));
      ComputeHashCode(new TestUnion_class_string_int_bool_guid_char('A'), 'A');

      ComputeHashCode(new TestUnion_struct_string_int("text"), "text");
      ComputeHashCode(new TestUnion_struct_string_int("text"), "TEXT");
      ComputeHashCode(new TestUnion_struct_string_int(42), 42);

      ComputeHashCode(TestUnion_class_with_same_types.CreateText("text"), "text");
      ComputeHashCode(new TestUnion_class_with_same_types(42), 42);
      ComputeHashCode(TestUnion_class_with_same_types.CreateString2("text2"), "TEXT2");
      ComputeHashCode(TestUnion_class_with_same_types.CreateString3("text3"), "text3");
      ComputeHashCode(new TestUnion_class_with_same_types((int?)1), (int?)1);
   }

   private static void ComputeHashCode<T, T2>(T union, T2 value)
   {
      var expected = HashCode.Combine(typeof(T), value is string s ? StringComparer.OrdinalIgnoreCase.GetHashCode(s) : value?.GetHashCode());
      union.GetHashCode().Should().Be(expected);
   }

   [Fact]
   public void Should_return_hashcode_of_case_sensitive_union()
   {
      ComputeHashCodeOrdinal(new TestUnion_class_string_int_case_sensitive("text"), "text", true);
      ComputeHashCodeOrdinal(new TestUnion_class_string_int_case_sensitive("text"), "TEXT", false);

      ComputeHashCodeOrdinal(new TestUnion_class_string_int_bool_case_sensitive("text"), "text", true);
      ComputeHashCodeOrdinal(new TestUnion_class_string_int_bool_case_sensitive("text"), "TEXT", false);

      ComputeHashCodeOrdinal(new TestUnion_class_string_int_bool_guid_case_sensitive("text"), "text", true);
      ComputeHashCodeOrdinal(new TestUnion_class_string_int_bool_guid_case_sensitive("text"), "TEXT", false);

      ComputeHashCodeOrdinal(new TestUnion_class_string_int_bool_guid_char_case_sensitive("text"), "text", true);
      ComputeHashCodeOrdinal(new TestUnion_class_string_int_bool_guid_char_case_sensitive("text"), "TEXT", false);

      ComputeHashCodeOrdinal(TestUnion_class_with_same_types_case_sensitive.CreateText("text"), "text", true);
      ComputeHashCodeOrdinal(TestUnion_class_with_same_types_case_sensitive.CreateText("text"), "tExt", false);

      ComputeHashCodeOrdinal(TestUnion_class_with_same_types_case_sensitive.CreateString2("text2"), "text2", true);
      ComputeHashCodeOrdinal(TestUnion_class_with_same_types_case_sensitive.CreateString2("text2"), "TEXT2", false);

      ComputeHashCodeOrdinal(TestUnion_class_with_same_types_case_sensitive.CreateString3("text3"), "text3", true);
      ComputeHashCodeOrdinal(TestUnion_class_with_same_types_case_sensitive.CreateString3("text3"), "Text3", false);
   }

   private static void ComputeHashCodeOrdinal<T>(T union, string value, bool equal)
   {
      var expected = HashCode.Combine(typeof(T), StringComparer.Ordinal.GetHashCode(value));

      if (equal)
      {
         union.GetHashCode().Should().Be(expected);
      }
      else
      {
         union.GetHashCode().Should().NotBe(expected);
      }
   }
}
