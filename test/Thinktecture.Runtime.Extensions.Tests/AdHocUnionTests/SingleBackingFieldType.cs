#nullable enable
using System;
using System.Collections.Generic;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// ReSharper disable once InconsistentNaming
public class SingleBackingFieldType
{
   public class Value_returns_typed_base_for_ref_members
   {
      [Fact]
      public void Should_expose_Value_typed_as_interface_base()
      {
         var foo1 = new Foo1 { ExtraInt = 7 };
         var union = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(foo1);

         IFoo value = union.Value;
         value.Should().BeSameAs(foo1);
      }

      [Fact]
      public void Should_allow_direct_access_to_shared_property_via_Value()
      {
         // The motivating use case: union.Value.Bar without Switch/Map boilerplate.
         var union1 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo1());
         var union2 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo2());

         union1.Value.Bar.Should().Be("foo1");
         union2.Value.Bar.Should().Be("foo2");
      }

      [Fact]
      public void Should_expose_Value_typed_as_IComparable_for_value_type_members()
      {
         var unionInt = new TestUnion_struct_int_double_SingleBackingFieldType_IComparable(42);
         var unionDouble = new TestUnion_struct_int_double_SingleBackingFieldType_IComparable(3.14);

         unionInt.Value.Should().BeOfType<int>().Which.Should().Be(42);
         unionDouble.Value.Should().BeOfType<double>().Which.Should().Be(3.14);
      }
   }

   public class Value_returns_default_boxed_struct_for_stateless_struct_member
   {
      [Fact]
      public void Should_return_non_null_default_for_stateless_struct()
      {
         var union = new TestUnion_with_StatelessStruct_member(new EmptyFooStruct());

         union.Value.Should().NotBeNull();
         union.Value.Bar.Should().Be("empty-struct");
      }

      [Fact]
      public void Should_share_cached_boxed_default_across_instances()
      {
         var union1 = new TestUnion_with_StatelessStruct_member(new EmptyFooStruct());
         var union2 = new TestUnion_with_StatelessStruct_member(new EmptyFooStruct());

         // Both instances reference the same static cached boxed default.
         union1.Value.Should().BeSameAs(union2.Value);
      }
   }

   public class Value_returns_null_for_stateless_ref_type_member
   {
      [Fact]
      public void Should_return_null_when_constructed_with_stateless_reference_type()
      {
         var union = new TestUnion_with_StatelessRefType_member((NullFoo?)null);

         union.Value.Should().BeNull();
      }

      [Fact]
      public void Should_return_non_null_for_non_stateless_member_in_same_union()
      {
         var foo1 = new Foo1();
         var union = new TestUnion_with_StatelessRefType_member(foo1);

         union.Value.Should().BeSameAs(foo1);
      }
   }

   public class AsTx_round_trips_correctly_after_typed_storage
   {
      [Fact]
      public void Should_cast_back_to_concrete_type()
      {
         var foo1 = new Foo1 { ExtraInt = 42 };
         var union = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(foo1);

         var asFoo1 = union.AsFoo1;
         asFoo1.Should().BeSameAs(foo1);
         asFoo1.ExtraInt.Should().Be(42);
      }

      [Fact]
      public void Should_throw_when_accessing_wrong_AsTx()
      {
         var union = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo1());

         Action act = () => _ = union.AsFoo2;
         act.Should().Throw<InvalidOperationException>();
      }

      [Fact]
      public void Should_round_trip_value_types_through_IComparable_backing_field()
      {
         var union = new TestUnion_struct_int_double_SingleBackingFieldType_IComparable(123);

         union.AsInt32.Should().Be(123);
      }
   }

   public class IsTx_works_correctly_after_typed_storage
   {
      [Fact]
      public void Should_report_correct_discriminator()
      {
         var union1 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo1());
         var union2 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo2());

         union1.IsFoo1.Should().BeTrue();
         union1.IsFoo2.Should().BeFalse();

         union2.IsFoo1.Should().BeFalse();
         union2.IsFoo2.Should().BeTrue();
      }

      [Fact]
      public void Should_report_correct_discriminator_for_stateless_struct_member()
      {
         var unionStateless = new TestUnion_with_StatelessStruct_member(new EmptyFooStruct());
         var unionFoo1 = new TestUnion_with_StatelessStruct_member(new Foo1());

         unionStateless.IsEmptyFooStruct.Should().BeTrue();
         unionStateless.IsFoo1.Should().BeFalse();

         unionFoo1.IsEmptyFooStruct.Should().BeFalse();
         unionFoo1.IsFoo1.Should().BeTrue();
      }

      [Fact]
      public void Should_report_correct_discriminator_for_stateless_ref_type_member()
      {
         var unionStateless = new TestUnion_with_StatelessRefType_member((NullFoo?)null);
         var unionFoo1 = new TestUnion_with_StatelessRefType_member(new Foo1());

         unionStateless.IsNullFoo.Should().BeTrue();
         unionStateless.IsFoo1.Should().BeFalse();

         unionFoo1.IsNullFoo.Should().BeFalse();
         unionFoo1.IsFoo1.Should().BeTrue();
      }
   }

   public class Switch_and_Map_still_work_with_typed_backing_field
   {
      [Fact]
      public void Should_invoke_correct_Switch_branch()
      {
         var union1 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo1());
         var union2 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo2());

         string? result1 = null;
         union1.Switch(
            foo1: f => result1 = $"f1:{f.Bar}",
            foo2: f => result1 = $"f2:{f.Bar}");

         string? result2 = null;
         union2.Switch(
            foo1: f => result2 = $"f1:{f.Bar}",
            foo2: f => result2 = $"f2:{f.Bar}");

         result1.Should().Be("f1:foo1");
         result2.Should().Be("f2:foo2");
      }

      [Fact]
      public void Should_invoke_correct_Map_branch()
      {
         var union1 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo1());
         var union2 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo2());

         var r1 = union1.Map(foo1: "one", foo2: "two");
         var r2 = union2.Map(foo1: "one", foo2: "two");

         r1.Should().Be("one");
         r2.Should().Be("two");
      }
   }

   public class Equality_unaffected_by_typed_backing_field
   {
      [Fact]
      public void Should_be_equal_when_same_discriminator_and_same_value()
      {
         var foo1 = new Foo1 { ExtraInt = 5 };
         var u1 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(foo1);
         var u2 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(foo1);

         u1.Equals(u2).Should().BeTrue();
         (u1 == u2).Should().BeTrue();
         (u1 != u2).Should().BeFalse();
      }

      [Fact]
      public void Should_not_be_equal_when_same_discriminator_but_different_value()
      {
         var u1 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo1 { ExtraInt = 1 });
         var u2 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo1 { ExtraInt = 2 });

         u1.Equals(u2).Should().BeFalse();
         (u1 == u2).Should().BeFalse();
         (u1 != u2).Should().BeTrue();
      }

      [Fact]
      public void Should_not_be_equal_when_different_discriminator()
      {
         var u1 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo1());
         var u2 = new TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo2());

         u1.Equals(u2).Should().BeFalse();
         (u1 == u2).Should().BeFalse();
         (u1 != u2).Should().BeTrue();
      }
   }

   public class Value_typed_as_resolved_generic_when_TypeParamRef_used
   {
      [Fact]
      public void Should_resolve_TypeParamRef1_to_concrete_type_in_Value()
      {
         var list = new List<int> { 1, 2, 3 };
         var union = new TestUnion_generic_with_TypeParamRef<int>(list);

         IEnumerable<int> value = union.Value;
         value.Should().BeEquivalentTo(new[] { 1, 2, 3 });
      }

      [Fact]
      public void Should_round_trip_array_member_through_typed_backing()
      {
         var arr = new[] { "a", "b" };
         var union = new TestUnion_generic_with_TypeParamRef<string>(arr);

         union.AsTArray.Should().BeSameAs(arr);
         union.Value.Should().BeEquivalentTo(arr);
      }
   }

   public class Value_returns_nullable_struct_when_SingleBackingFieldType_is_nullable_struct
   {
      [Fact]
      public void Should_expose_Value_typed_as_nullable_int()
      {
         var union = new TestUnion_int_short_SingleBackingFieldType_NullableInt(42);

         int? value = union.Value;
         value.Should().Be(42);
      }

      [Fact]
      public void Should_round_trip_short_member_through_nullable_int_backing()
      {
         var union = new TestUnion_int_short_SingleBackingFieldType_NullableInt((short)7);

         union.IsInt16.Should().BeTrue();
         union.Value.Should().Be(7);
      }

      [Fact]
      public void Equality_works_for_two_unions_with_same_discriminator()
      {
         var union1 = new TestUnion_int_short_SingleBackingFieldType_NullableInt(42);
         var union2 = new TestUnion_int_short_SingleBackingFieldType_NullableInt(42);
         var union3 = new TestUnion_int_short_SingleBackingFieldType_NullableInt(99);

         union1.Equals(union2).Should().BeTrue();
         union1.Equals(union3).Should().BeFalse();
      }
   }

   public class Default_struct_union_with_SingleBackingFieldType
   {
      // The contract for struct unions is documented on the generated Value getter:
      //   <exception cref="System.InvalidOperationException">If the union (struct) is not
      //   initialized or initialized with default value.</exception>
      // The generator emits the discriminator switch with `0 => throw new InvalidOperationException(...)`
      // for Value, AsTx, Switch, Map, ToString, GetHashCode and Equals(Self) on struct unions --
      // including those configured with SingleBackingFieldType. IsTx does not throw: it simply
      // returns false when the discriminator is 0.

      [Fact]
      public void Should_throw_InvalidOperationException_from_Value_for_default_struct_union()
      {
         var union = default(TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo);

         Action act = () => _ = union.Value;
         act.Should().Throw<InvalidOperationException>()
            .WithMessage("This struct of type 'TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo' is not initialized. Make sure all fields, properties and variables are initialized with non-default values.");
      }

      [Fact]
      public void Should_throw_InvalidOperationException_from_AsTx_for_default_struct_union()
      {
         var union = default(TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo);

         Action actFoo1 = () => _ = union.AsFoo1;
         actFoo1.Should().Throw<InvalidOperationException>();

         Action actFoo2 = () => _ = union.AsFoo2;
         actFoo2.Should().Throw<InvalidOperationException>();
      }

      [Fact]
      public void Should_return_false_from_IsTx_for_default_struct_union()
      {
         var union = default(TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo);

         // IsTx does not throw: discriminator == 0 != membership index, so both return false.
         union.IsFoo1.Should().BeFalse();
         union.IsFoo2.Should().BeFalse();
      }

      [Fact]
      public void Should_throw_InvalidOperationException_from_Switch_for_default_struct_union()
      {
         var union = default(TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo);

         Action act = () => union.Switch(
            foo1: _ => { },
            foo2: _ => { });
         act.Should().Throw<InvalidOperationException>();
      }

      [Fact]
      public void Should_throw_InvalidOperationException_from_Map_for_default_struct_union()
      {
         var union = default(TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo);

         Action act = () => _ = union.Map(foo1: "one", foo2: "two");
         act.Should().Throw<InvalidOperationException>();
      }

      [Fact]
      public void Should_throw_InvalidOperationException_from_Equals_when_comparing_default_to_default()
      {
         var u1 = default(TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo);
         var u2 = default(TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo);

         // valueIndex == valueIndex == 0 -> switch arm 0 -> throw (matches existing struct-union behavior).
         Action act = () => u1.Equals(u2);
         act.Should().Throw<InvalidOperationException>();
      }

      [Fact]
      public void Should_return_false_from_Equals_when_comparing_default_to_initialized()
      {
         var u1 = default(TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo);
         var u2 = new TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo(new Foo1());

         // valueIndex 0 != valueIndex 1 -> early return false (no throw).
         u1.Equals(u2).Should().BeFalse();
         u2.Equals(u1).Should().BeFalse();
      }

      [Fact]
      public void Should_throw_InvalidOperationException_from_GetHashCode_for_default_struct_union()
      {
         var union = default(TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo);

         Action act = () => _ = union.GetHashCode();
         act.Should().Throw<InvalidOperationException>();
      }

      [Fact]
      public void Should_return_Foo1_from_Value_when_initialized_with_Foo1()
      {
         var foo1 = new Foo1 { ExtraInt = 9 };
         var union = new TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo(foo1);

         IFoo value = union.Value;
         value.Should().BeSameAs(foo1);
         value.Bar.Should().Be("foo1");
      }

      [Fact]
      public void Should_round_trip_struct_union_through_typed_Value_for_Foo2()
      {
         var foo2 = new Foo2 { ExtraString = "abc" };
         var union = new TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo(foo2);

         union.IsFoo2.Should().BeTrue();
         union.Value.Should().BeSameAs(foo2);
         union.AsFoo2.Should().BeSameAs(foo2);
      }
   }

   public class Default_struct_union_with_stateless_struct_member
   {
      [Fact]
      public void Should_throw_InvalidOperationException_from_Value_for_default_struct_union_with_stateless_member()
      {
         var union = default(TestUnion_struct_with_StatelessStruct_member_SingleBackingFieldType);

         Action act = () => _ = union.Value;
         act.Should().Throw<InvalidOperationException>()
            .WithMessage("This struct of type 'TestUnion_struct_with_StatelessStruct_member_SingleBackingFieldType' is not initialized. Make sure all fields, properties and variables are initialized with non-default values.");
      }

      [Fact]
      public void Should_return_cached_boxed_default_from_Value_when_initialized_with_stateless_struct_member()
      {
         var union1 = new TestUnion_struct_with_StatelessStruct_member_SingleBackingFieldType(new EmptyFooStruct());
         var union2 = new TestUnion_struct_with_StatelessStruct_member_SingleBackingFieldType(new EmptyFooStruct());

         union1.Value.Should().NotBeNull();
         union1.Value.Bar.Should().Be("empty-struct");

         // Both instances must reference the SAME boxed default; the static field is the cache.
         // ReferenceEquals proves the boxing happened once at type init and is shared across instances.
         ReferenceEquals(union1.Value, union2.Value).Should().BeTrue();
      }

      [Fact]
      public void Should_return_Foo1_from_Value_when_initialized_with_non_stateless_member()
      {
         var foo1 = new Foo1 { ExtraInt = 3 };
         var union = new TestUnion_struct_with_StatelessStruct_member_SingleBackingFieldType(foo1);

         union.IsFoo1.Should().BeTrue();
         union.Value.Should().BeSameAs(foo1);
      }
   }
}
