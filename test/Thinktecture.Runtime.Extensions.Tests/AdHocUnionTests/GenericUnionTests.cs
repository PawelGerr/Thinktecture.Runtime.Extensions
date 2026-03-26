using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// ReSharper disable once InconsistentNaming
public class GenericUnionTests
{
   public class DuplicateTypeParamRef
   {
      [Fact]
      public void Should_create_via_factory_methods()
      {
         var v1 = TestUnion_generic_struct_duplicate_TypeParamRef1<string>.CreateT1("hello");
         var v2 = TestUnion_generic_struct_duplicate_TypeParamRef1<string>.CreateT2("world");

         v1.IsT1.Should().BeTrue();
         v1.IsT2.Should().BeFalse();
         v1.AsT1.Should().Be("hello");

         v2.IsT1.Should().BeFalse();
         v2.IsT2.Should().BeTrue();
         v2.AsT2.Should().Be("world");
      }

      [Fact]
      public void Should_switch_to_correct_branch()
      {
         var v1 = TestUnion_generic_struct_duplicate_TypeParamRef1<int>.CreateT1(1);
         var v2 = TestUnion_generic_struct_duplicate_TypeParamRef1<int>.CreateT2(2);

         var result1 = v1.Switch(t1: v => $"t1:{v}", t2: v => $"t2:{v}");
         var result2 = v2.Switch(t1: v => $"t1:{v}", t2: v => $"t2:{v}");

         result1.Should().Be("t1:1");
         result2.Should().Be("t2:2");
      }

      [Fact]
      public void Should_not_be_equal_when_same_value_in_different_slots()
      {
         var v1 = TestUnion_generic_struct_duplicate_TypeParamRef1<string>.CreateT1("same");
         var v2 = TestUnion_generic_struct_duplicate_TypeParamRef1<string>.CreateT2("same");

         v1.Equals(v2).Should().BeFalse();
         (v1 == v2).Should().BeFalse();
         (v1 != v2).Should().BeTrue();
      }

      [Fact]
      public void Should_be_equal_when_same_value_in_same_slot()
      {
         var v1 = TestUnion_generic_struct_duplicate_TypeParamRef1<string>.CreateT1("same");
         var v2 = TestUnion_generic_struct_duplicate_TypeParamRef1<string>.CreateT1("same");

         v1.Equals(v2).Should().BeTrue();
         (v1 == v2).Should().BeTrue();
      }

      [Fact]
      public void Should_return_correct_value_from_Value_property()
      {
         var v1 = TestUnion_generic_struct_duplicate_TypeParamRef1<int>.CreateT1(42);
         var v2 = TestUnion_generic_struct_duplicate_TypeParamRef1<int>.CreateT2(99);

         v1.Value.Should().Be(42);
         v2.Value.Should().Be(99);
      }

      [Fact]
      public void Should_map_to_correct_result()
      {
         var v1 = TestUnion_generic_struct_duplicate_TypeParamRef1<string>.CreateT1("hello");
         var v2 = TestUnion_generic_struct_duplicate_TypeParamRef1<string>.CreateT2("world");

         v1.Map(t1: "first", t2: "second").Should().Be("first");
         v2.Map(t1: "first", t2: "second").Should().Be("second");
      }

      [Fact]
      public void Should_produce_correct_ToString()
      {
         var v = TestUnion_generic_struct_duplicate_TypeParamRef1<int>.CreateT1(42);

         v.ToString().Should().Be("42");
      }
   }

   public class TwoTypeParamsWithSameConcreteType
   {
      [Fact]
      public void Should_require_factory_methods_when_both_type_params_are_string()
      {
         // When T1 and T2 are both string, constructors are ambiguous (CS0121).
         // Factory methods must be used instead.
         var v1 = TestUnion_generic_class_TypeParamRef1_TypeParamRef2<string, string>.CreateT1("hello");
         var v2 = TestUnion_generic_class_TypeParamRef1_TypeParamRef2<string, string>.CreateT2("world");

         v1.IsT1.Should().BeTrue();
         v1.AsT1.Should().Be("hello");

         v2.IsT2.Should().BeTrue();
         v2.AsT2.Should().Be("world");
      }

      [Fact]
      public void Should_switch_correctly_when_both_type_params_are_string()
      {
         var v = TestUnion_generic_class_TypeParamRef1_TypeParamRef2<string, string>.CreateT1("first");

         var result = v.Switch(t1: v1 => $"t1:{v1}", t2: v2 => $"t2:{v2}");

         result.Should().Be("t1:first");
      }

      [Fact]
      public void Should_not_be_equal_when_same_value_in_different_slots()
      {
         var v1 = TestUnion_generic_class_TypeParamRef1_TypeParamRef2<string, string>.CreateT1("same");
         var v2 = TestUnion_generic_class_TypeParamRef1_TypeParamRef2<string, string>.CreateT2("same");

         v1.Equals(v2).Should().BeFalse();
      }
   }

   public class TwoTypeParamsWithTwoRefTypes
   {
      [Fact]
      public void Should_create_all_four_variants_when_both_type_params_are_string()
      {
         var vT1 = TestUnion_generic_class_two_TypeParamRefs_and_two_ref_types<string, string>.CreateT1("fromT1");
         var vT2 = TestUnion_generic_class_two_TypeParamRefs_and_two_ref_types<string, string>.CreateT2("fromT2");
         var vStr = new TestUnion_generic_class_two_TypeParamRefs_and_two_ref_types<string, string>("literal");
         var vInt = new TestUnion_generic_class_two_TypeParamRefs_and_two_ref_types<string, string>(42);

         vT1.IsT1.Should().BeTrue();
         vT1.AsT1.Should().Be("fromT1");

         vT2.IsT2.Should().BeTrue();
         vT2.AsT2.Should().Be("fromT2");

         vStr.IsString.Should().BeTrue();
         vStr.AsString.Should().Be("literal");

         vInt.IsInt32.Should().BeTrue();
         vInt.AsInt32.Should().Be(42);
      }

      [Fact]
      public void Should_switch_to_correct_branch_when_both_type_params_are_string()
      {
         var vT1 = TestUnion_generic_class_two_TypeParamRefs_and_two_ref_types<string, string>.CreateT1("fromT1");
         var vStr = new TestUnion_generic_class_two_TypeParamRefs_and_two_ref_types<string, string>("literal");

         var resultT1 = vT1.Switch(t1: v => "t1", t2: v => "t2", @string: v => "string", int32: v => "int32");
         var resultStr = vStr.Switch(t1: v => "t1", t2: v => "t2", @string: v => "string", int32: v => "int32");

         resultT1.Should().Be("t1");
         resultStr.Should().Be("string");
      }

      [Fact]
      public void Should_not_be_equal_across_slots_when_both_type_params_are_string()
      {
         var vT1 = TestUnion_generic_class_two_TypeParamRefs_and_two_ref_types<string, string>.CreateT1("same");
         var vT2 = TestUnion_generic_class_two_TypeParamRefs_and_two_ref_types<string, string>.CreateT2("same");
         var vStr = new TestUnion_generic_class_two_TypeParamRefs_and_two_ref_types<string, string>("same");

         vT1.Equals(vT2).Should().BeFalse();
         vT1.Equals(vStr).Should().BeFalse();
         vT2.Equals(vStr).Should().BeFalse();
      }
   }
}
