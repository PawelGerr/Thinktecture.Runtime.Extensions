using System;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class Equals
{
   [Fact]
   public void Should_be_equal_for_same_value_and_type_argument_for_generic_int_based_value_objects()
   {
      var obj1 = ValueObject_Generic_IntBased<string>.Create(42);
      var obj2 = ValueObject_Generic_IntBased<string>.Create(42);

      obj1.Should().Be(obj2);
      (obj1 == obj2).Should().BeTrue();
      (obj1 != obj2).Should().BeFalse();
   }

   [Fact]
   public void Should_not_be_equal_for_different_values_for_generic_int_based_value_objects()
   {
      var obj1 = ValueObject_Generic_IntBased<string>.Create(42);
      var obj2 = ValueObject_Generic_IntBased<string>.Create(43);

      obj1.Should().NotBe(obj2);
      (obj1 == obj2).Should().BeFalse();
      (obj1 != obj2).Should().BeTrue();
   }

   [Fact]
   public void Should_have_consistent_hashcode_for_generic_int_based_value_objects()
   {
      var obj1 = ValueObject_Generic_IntBased<string>.Create(42);
      var obj2 = ValueObject_Generic_IntBased<string>.Create(42);

      obj1.GetHashCode().Should().Be(obj2.GetHashCode());
   }

   [Fact]
   public void Should_have_different_hashcode_for_different_values_for_generic_int_based_value_objects()
   {
      var obj1 = ValueObject_Generic_IntBased<string>.Create(42);
      var obj2 = ValueObject_Generic_IntBased<string>.Create(43);

      obj1.GetHashCode().Should().NotBe(obj2.GetHashCode());
   }

   [Fact]
   public void Should_be_equal_for_same_value_and_type_argument_for_generic_string_based_value_objects()
   {
      var obj1 = ValueObject_Generic_StringBased<object>.Create("test");
      var obj2 = ValueObject_Generic_StringBased<object>.Create("test");

      obj1.Should().Be(obj2);
      (obj1 == obj2).Should().BeTrue();
      (obj1 != obj2).Should().BeFalse();
   }

   [Fact]
   public void Should_not_be_equal_for_different_values_for_generic_string_based_value_objects()
   {
      var obj1 = ValueObject_Generic_StringBased<object>.Create("test1");
      var obj2 = ValueObject_Generic_StringBased<object>.Create("test2");

      obj1.Should().NotBe(obj2);
      (obj1 == obj2).Should().BeFalse();
      (obj1 != obj2).Should().BeTrue();
   }

   [Fact]
   public void Should_have_consistent_hashcode_for_generic_string_based_value_objects()
   {
      var obj1 = ValueObject_Generic_StringBased<object>.Create("test");
      var obj2 = ValueObject_Generic_StringBased<object>.Create("test");

      obj1.GetHashCode().Should().Be(obj2.GetHashCode());
   }

   [Fact]
   public void Should_be_equal_for_same_guid_and_type_argument_for_generic_guid_based_value_objects()
   {
      var guid = Guid.NewGuid();
      var obj1 = ValueObject_Generic_GuidBased<string>.Create(guid);
      var obj2 = ValueObject_Generic_GuidBased<string>.Create(guid);

      obj1.Should().Be(obj2);
      (obj1 == obj2).Should().BeTrue();
      (obj1 != obj2).Should().BeFalse();
   }

   [Fact]
   public void Should_not_be_equal_for_different_guids_for_generic_guid_based_value_objects()
   {
      var obj1 = ValueObject_Generic_GuidBased<string>.Create(Guid.NewGuid());
      var obj2 = ValueObject_Generic_GuidBased<string>.Create(Guid.NewGuid());

      obj1.Should().NotBe(obj2);
      (obj1 == obj2).Should().BeFalse();
      (obj1 != obj2).Should().BeTrue();
   }

   [Fact]
   public void Should_have_consistent_hashcode_for_generic_guid_based_value_objects()
   {
      var guid = Guid.NewGuid();
      var obj1 = ValueObject_Generic_GuidBased<string>.Create(guid);
      var obj2 = ValueObject_Generic_GuidBased<string>.Create(guid);

      obj1.GetHashCode().Should().Be(obj2.GetHashCode());
   }

   [Fact]
   public void Should_be_equal_for_same_value_and_type_argument_for_generic_struct_value_objects()
   {
      var obj1 = StructValueObject_Generic_IntBased<string>.Create(42);
      var obj2 = StructValueObject_Generic_IntBased<string>.Create(42);

      obj1.Should().Be(obj2);
      (obj1 == obj2).Should().BeTrue();
      (obj1 != obj2).Should().BeFalse();
   }

   [Fact]
   public void Should_not_be_equal_for_different_values_for_generic_struct_value_objects()
   {
      var obj1 = StructValueObject_Generic_IntBased<string>.Create(42);
      var obj2 = StructValueObject_Generic_IntBased<string>.Create(43);

      obj1.Should().NotBe(obj2);
      (obj1 == obj2).Should().BeFalse();
      (obj1 != obj2).Should().BeTrue();
   }
}

