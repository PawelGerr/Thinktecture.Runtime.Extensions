#nullable enable
using System;
using System.Collections.Generic;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

public class Value
{
   [Fact]
   public void Should_return_correct_value_having_2_types()
   {
      new TestUnion_class_string_int("text").Value.Should().Be("text");
      new TestUnion_class_string_int(1).Value.Should().Be(1);

      new TestUnion_class_nullable_string_int(@string: null).Value.Should().BeNull();
      new TestUnion_class_nullable_string_int("text").Value.Should().Be("text");
      new TestUnion_class_nullable_string_int(1).Value.Should().Be(1);

      new TestUnion_class_nullable_string_nullable_int(@string: null).Value.Should().BeNull();
      new TestUnion_class_nullable_string_nullable_int("text").Value.Should().Be("text");
      new TestUnion_class_nullable_string_nullable_int(1).Value.Should().Be(1);
      new TestUnion_class_nullable_string_nullable_int(nullableOfInt32: null).Value.Should().BeNull();

      new TestUnion_struct_string_int("text").Value.Should().Be("text");
      new TestUnion_struct_string_int(1).Value.Should().Be(1);

      new TestUnion_class_with_array(["text"]).Value.Should().BeEquivalentTo(new[] { "text" });
      new TestUnion_class_with_array(1).Value.Should().Be(1);
   }

   [Fact]
   public void Should_return_correct_value_having_3_types()
   {
      new TestUnion_class_string_int_bool("text").Value.Should().Be("text");
      new TestUnion_class_string_int_bool(1).Value.Should().Be(1);
      new TestUnion_class_string_int_bool(true).Value.Should().Be(true);
   }

   [Fact]
   public void Should_return_correct_value_having_4_types()
   {
      new TestUnion_class_string_int_bool_guid("text").Value.Should().Be("text");
      new TestUnion_class_string_int_bool_guid(1).Value.Should().Be(1);
      new TestUnion_class_string_int_bool_guid(true).Value.Should().Be(true);
      new TestUnion_class_string_int_bool_guid(new Guid("04F2DA71-1E0F-4AA4-AD1E-CE56BEDED52B")).Value.Should().Be(new Guid("04F2DA71-1E0F-4AA4-AD1E-CE56BEDED52B"));
   }

   [Fact]
   public void Should_return_correct_value_having_5_types()
   {
      new TestUnion_class_string_int_bool_guid_char("text").Value.Should().Be("text");
      new TestUnion_class_string_int_bool_guid_char(1).Value.Should().Be(1);
      new TestUnion_class_string_int_bool_guid_char(true).Value.Should().Be(true);
      new TestUnion_class_string_int_bool_guid_char(new Guid("04F2DA71-1E0F-4AA4-AD1E-CE56BEDED52B")).Value.Should().Be(new Guid("04F2DA71-1E0F-4AA4-AD1E-CE56BEDED52B"));
      new TestUnion_class_string_int_bool_guid_char('A').Value.Should().Be('A');
   }

   [Fact]
   public void Should_return_correct_value_having_5_types_with_duplicates()
   {
      TestUnion_class_with_same_types.CreateText("text").Value.Should().Be("text");
      new TestUnion_class_with_same_types(1).Value.Should().Be(1);
      TestUnion_class_with_same_types.CreateString2("text2").Value.Should().Be("text2");
      TestUnion_class_with_same_types.CreateString3("text3").Value.Should().Be("text3");
      new TestUnion_class_with_same_types((int?)2).Value.Should().Be((int?)2);
   }

   [Fact]
   public void Should_return_correct_value_when_all_members_share_the_backing_field()
   {
      new TestUnion_class_string_listOfInts("text").Value.Should().Be("text");
      new TestUnion_class_string_listOfInts(new List<int> { 1, 2 }).Value.Should().BeEquivalentTo(new[] { 1, 2 });

      new TestUnion_struct_string_listOfInts("text").Value.Should().Be("text");
      new TestUnion_struct_string_listOfInts(new List<int> { 1, 2 }).Value.Should().BeEquivalentTo(new[] { 1, 2 });
   }

   [Fact]
   public void Should_throw_when_struct_union_sharing_the_backing_field_is_not_initialized()
   {
      var union = default(TestUnion_struct_string_listOfInts);

      var action = () => union.Value;

      action.Should().Throw<InvalidOperationException>()
            .WithMessage("*is not initialized*");
   }

   [Fact]
   public void Should_return_null_for_stateless_reference_type_member()
   {
      new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass(new NullValueClass()).Value.Should().BeNull();
      new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass(new EmptyStateClass()).Value.Should().BeNull();

      new TestUnion_class_stateless_nullvalueclass_string_listOfInts(new NullValueClass()).Value.Should().BeNull();
      new TestUnion_class_stateless_nullvalueclass_string_listOfInts("text").Value.Should().Be("text");
      new TestUnion_class_stateless_nullvalueclass_string_listOfInts(new List<int> { 1 }).Value.Should().BeEquivalentTo(new[] { 1 });
   }

   [Fact]
   public void Should_return_default_of_stateless_value_type_member_when_UseSingleBackingField_is_set()
   {
      // The stateless struct member has no assignment of its own, so the shared field stays null.
      // Value must still return the boxed default of the member type, not null.
      new TestUnion_class_string_stateless_nullvaluestruct_UseSingleBackingField("text").Value.Should().Be("text");
      new TestUnion_class_string_stateless_nullvaluestruct_UseSingleBackingField(new NullValueStruct()).Value.Should().Be(new NullValueStruct());
   }

   [Fact]
   public void Should_return_null_when_all_members_are_stateless_and_UseSingleBackingField_is_set()
   {
      new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_UseSingleBackingField(new NullValueClass()).Value.Should().BeNull();
      new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_UseSingleBackingField(new EmptyStateClass()).Value.Should().BeNull();

      new TestUnion_struct_stateless_nullvalueclass_stateless_emptystateclass_UseSingleBackingField(new NullValueClass()).Value.Should().BeNull();
      new TestUnion_struct_stateless_nullvalueclass_stateless_emptystateclass_UseSingleBackingField(new EmptyStateClass()).Value.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_for_stateless_type_parameter_member_constrained_to_a_reference_type()
   {
      // A stateless type parameter gets no assignment, so the shared field stays null. For a
      // reference type that is exactly what default(T) is, therefore reading the field is correct.
      new TestUnion_generic_class_stateless_classconstrained_TypeParamRef1_string_UseSingleBackingField<Foo1>(new Foo1()).Value.Should().BeNull();
      new TestUnion_generic_class_stateless_classconstrained_TypeParamRef1_string_UseSingleBackingField<Foo1>("text").Value.Should().Be("text");

      new TestUnion_generic_class_stateless_classconstrained_TypeParamRef1_Foo1_SingleBackingFieldType<Foo2>(new Foo2()).Value.Should().BeNull();

      var foo1 = new Foo1();
      new TestUnion_generic_class_stateless_classconstrained_TypeParamRef1_Foo1_SingleBackingFieldType<Foo2>(foo1).Value.Should().Be(foo1);
   }

   [Fact]
   public void Should_return_boxed_default_for_stateless_type_parameter_member_constrained_to_a_value_type()
   {
      // The shared field stays null for a stateless type parameter, but default(T) of a struct is a
      // boxed zero value. Value must return that boxed value instead of reading the field.
      new TestUnion_generic_class_stateless_structconstrained_TypeParamRef1_string_UseSingleBackingField<int>(0).Value.Should().Be(0);
      new TestUnion_generic_class_stateless_structconstrained_TypeParamRef1_string_UseSingleBackingField<int>("text").Value.Should().Be("text");

      new TestUnion_generic_class_stateless_structconstrained_TypeParamRef1_Foo1_SingleBackingFieldType<FooStruct>(new FooStruct()).Value.Should().Be(new FooStruct());

      var foo1 = new Foo1();
      new TestUnion_generic_class_stateless_structconstrained_TypeParamRef1_Foo1_SingleBackingFieldType<FooStruct>(foo1).Value.Should().Be(foo1);
   }
}
