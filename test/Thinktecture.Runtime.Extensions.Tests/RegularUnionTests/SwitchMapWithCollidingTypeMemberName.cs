using Thinktecture.Runtime.Tests.TestRegularUnions;

namespace Thinktecture.Runtime.Tests.RegularUnionTests;

// ReSharper disable once InconsistentNaming
public class SwitchMapWithCollidingTypeMemberName
{
   [Fact]
   public void Should_call_type_specific_action_when_type_member_name_collides_with_pattern_local()
   {
      TestUnionWithTypeMemberNamedValue calledActionOn = null;
      var union = (TestUnionWithTypeMemberNamedValue)new TestUnionWithTypeMemberNamedValue.Value("v");

      union.Switch(@value: v => calledActionOn = v,
                   other: o => calledActionOn = o);

      calledActionOn.Should().Be(new TestUnionWithTypeMemberNamedValue.Value("v"));
   }

   [Fact]
   public void Should_call_fallback_action_when_type_member_name_collides_with_pattern_local()
   {
      TestUnionWithTypeMemberNamedValue calledActionOn = null;
      var union = (TestUnionWithTypeMemberNamedValue)new TestUnionWithTypeMemberNamedValue.Value("v");

      union.SwitchPartially(@default: u => calledActionOn = u,
                            other: o => calledActionOn = o);

      calledActionOn.Should().Be(new TestUnionWithTypeMemberNamedValue.Value("v"));
   }

   [Fact]
   public void Should_return_type_specific_value_when_type_member_name_collides_with_pattern_local()
   {
      var union = (TestUnionWithTypeMemberNamedValue)new TestUnionWithTypeMemberNamedValue.Value("v");

      union.Map(@value: "v", other: "o").Should().Be("v");
      union.MapPartially(@default: "default", @value: "v").Should().Be("v");
   }

   [Fact]
   public void Should_return_fallback_value_when_type_member_name_collides_with_pattern_local()
   {
      var union = (TestUnionWithTypeMemberNamedValue)new TestUnionWithTypeMemberNamedValue.Value("v");

      union.MapPartially(@default: "default", other: "o").Should().Be("default");
   }
}
