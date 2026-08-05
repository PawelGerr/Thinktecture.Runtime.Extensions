using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// Regression tests for a duplicate-member union whose first duplicate is stateless while a later
// duplicate of the same type is stateful. The stateful duplicate must round-trip through its own
// backing field instead of silently reading the stateless member's storage.
public class FirstStatelessDuplicate
{
   [Fact]
   public void Should_report_correct_discriminator_for_struct_union()
   {
      var first = TestUnion_struct_stateless_emptystatestruct_emptystatestruct.CreateFirst();
      var second = TestUnion_struct_stateless_emptystatestruct_emptystatestruct.CreateSecond(new EmptyStateStruct());

      first.IsFirst.Should().BeTrue();
      first.IsSecond.Should().BeFalse();

      second.IsSecond.Should().BeTrue();
      second.IsFirst.Should().BeFalse();
   }

   [Fact]
   public void Should_round_trip_stateful_duplicate_of_struct_union()
   {
      var second = TestUnion_struct_stateless_emptystatestruct_emptystatestruct.CreateSecond(new EmptyStateStruct());

      second.AsSecond.Should().Be(new EmptyStateStruct());
      second.Value.Should().Be(new EmptyStateStruct());
   }

   [Fact]
   public void Should_treat_stateless_and_stateful_duplicate_of_struct_union_as_unequal()
   {
      var first = TestUnion_struct_stateless_emptystatestruct_emptystatestruct.CreateFirst();
      var second = TestUnion_struct_stateless_emptystatestruct_emptystatestruct.CreateSecond(new EmptyStateStruct());

      first.Equals(second).Should().BeFalse();
      (first == second).Should().BeFalse();

      // Two stateful instances carry the same singleton struct value, so they are equal.
      var otherSecond = TestUnion_struct_stateless_emptystatestruct_emptystatestruct.CreateSecond(new EmptyStateStruct());
      second.Equals(otherSecond).Should().BeTrue();
      second.GetHashCode().Should().Be(otherSecond.GetHashCode());
   }

   [Fact]
   public void Should_dispatch_switch_and_map_of_struct_union_to_the_matching_arm()
   {
      var first = TestUnion_struct_stateless_emptystatestruct_emptystatestruct.CreateFirst();
      var second = TestUnion_struct_stateless_emptystatestruct_emptystatestruct.CreateSecond(new EmptyStateStruct());

      first.Map(first: 1, second: 2).Should().Be(1);
      second.Map(first: 1, second: 2).Should().Be(2);

      var firstCalls = 0;
      var secondCalls = 0;
      first.Switch(first: _ => firstCalls++, second: _ => secondCalls++);
      second.Switch(first: _ => firstCalls++, second: _ => secondCalls++);

      firstCalls.Should().Be(1);
      secondCalls.Should().Be(1);
   }

   [Fact]
   public void Should_report_correct_discriminator_for_class_union()
   {
      var first = TestUnion_class_stateless_emptystateclass_emptystateclass.CreateFirst();
      var second = TestUnion_class_stateless_emptystateclass_emptystateclass.CreateSecond(new EmptyStateClass());

      first.IsFirst.Should().BeTrue();
      first.IsSecond.Should().BeFalse();

      second.IsSecond.Should().BeTrue();
      second.IsFirst.Should().BeFalse();
   }

   [Fact]
   public void Should_round_trip_stateful_duplicate_of_class_union()
   {
      var instance = new EmptyStateClass();
      var second = TestUnion_class_stateless_emptystateclass_emptystateclass.CreateSecond(instance);

      second.AsSecond.Should().BeSameAs(instance);
      second.Value.Should().BeSameAs(instance);
   }

   [Fact]
   public void Should_treat_stateless_and_stateful_duplicate_of_class_union_as_unequal()
   {
      var first = TestUnion_class_stateless_emptystateclass_emptystateclass.CreateFirst();
      var second = TestUnion_class_stateless_emptystateclass_emptystateclass.CreateSecond(new EmptyStateClass());

      first.Equals(second).Should().BeFalse();
      (first == second).Should().BeFalse();

      var instance = new EmptyStateClass();
      var secondA = TestUnion_class_stateless_emptystateclass_emptystateclass.CreateSecond(instance);
      var secondB = TestUnion_class_stateless_emptystateclass_emptystateclass.CreateSecond(instance);
      secondA.Equals(secondB).Should().BeTrue();
      secondA.GetHashCode().Should().Be(secondB.GetHashCode());
   }

   [Fact]
   public void Should_dispatch_switch_and_map_of_class_union_to_the_matching_arm()
   {
      var first = TestUnion_class_stateless_emptystateclass_emptystateclass.CreateFirst();
      var second = TestUnion_class_stateless_emptystateclass_emptystateclass.CreateSecond(new EmptyStateClass());

      first.Map(first: 1, second: 2).Should().Be(1);
      second.Map(first: 1, second: 2).Should().Be(2);

      var firstCalls = 0;
      var secondCalls = 0;
      first.Switch(first: _ => firstCalls++, second: _ => secondCalls++);
      second.Switch(first: _ => firstCalls++, second: _ => secondCalls++);

      firstCalls.Should().Be(1);
      secondCalls.Should().Be(1);
   }

   // Stateful-first mixed group with a single backing field: the later stateless duplicate must store the
   // cached boxed default so that Value returns the boxed default(T) instead of null, and the stateful
   // duplicate must round-trip through the shared field.
   [Fact]
   public void Should_return_boxed_default_for_stateless_index_of_single_backing_field_union()
   {
      var second = TestUnion_struct_emptystatestruct_stateless_emptystatestruct_UseSingleBackingField.CreateSecond();

      second.IsSecond.Should().BeTrue();
      second.Value.Should().NotBeNull();
      second.Value.Should().Be(default(EmptyStateStruct));
   }

   [Fact]
   public void Should_round_trip_stateful_first_of_single_backing_field_union()
   {
      var first = TestUnion_struct_emptystatestruct_stateless_emptystatestruct_UseSingleBackingField.CreateFirst(new EmptyStateStruct());

      first.IsFirst.Should().BeTrue();
      first.AsFirst.Should().Be(new EmptyStateStruct());
      first.Value.Should().Be(new EmptyStateStruct());
   }
}
