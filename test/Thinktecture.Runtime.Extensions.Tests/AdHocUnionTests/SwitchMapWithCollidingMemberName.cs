using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// ReSharper disable once InconsistentNaming
public class SwitchMapWithCollidingMemberName
{
   [Fact]
   public void Should_call_member_action_when_member_type_name_collides_with_fixed_parameter_name()
   {
      var member = new CollidingMemberTypes.Default();
      var union = new TestUnion_class_memberNamedDefault_int(member);
      object calledActionOn = null;
      var fallbackCalled = false;

      // The fallback records something different from the member callback, so the assertions below show which of
      // the two callbacks ran instead of only showing that one of them ran.
      union.SwitchPartially(@default1: _ => fallbackCalled = true,
                            @default: v => calledActionOn = v);

      calledActionOn.Should().Be(member);
      fallbackCalled.Should().BeFalse();
   }

   [Fact]
   public void Should_call_fallback_action_when_member_type_name_collides_with_fixed_parameter_name()
   {
      var union = new TestUnion_class_memberNamedDefault_int(42);
      object calledActionOn = null;

      union.SwitchPartially(@default1: v => calledActionOn = v,
                            @default: v => calledActionOn = v);

      calledActionOn.Should().Be(42);
   }

   [Fact]
   public void Should_return_member_value_when_member_type_name_collides_with_fixed_parameter_name()
   {
      var union = new TestUnion_class_memberNamedDefault_int(42);

      union.MapPartially(@default1: "fallback", int32: "int").Should().Be("int");
   }

   [Fact]
   public void Should_return_fallback_value_when_member_type_name_collides_with_fixed_parameter_name()
   {
      var union = new TestUnion_class_memberNamedDefault_int(42);

      union.MapPartially(@default1: "fallback", @default: "default").Should().Be("fallback");
   }
}
