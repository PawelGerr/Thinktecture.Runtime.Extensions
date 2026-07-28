using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable once InconsistentNaming
public class SwitchMapWithCollidingItemNames
{
   [Fact]
   public void Should_call_item_action_when_item_name_collides_with_fixed_parameter_name()
   {
      SmartEnum_ItemsCollidingWithSwitchMapParameters calledActionOn = null;
      var fallbackCalled = false;

      // The fallback records something different from the item callback, so the assertions below show which of the
      // two callbacks ran instead of only showing that one of them ran.
      SmartEnum_ItemsCollidingWithSwitchMapParameters.Default.SwitchPartially(
         @default1: _ => fallbackCalled = true,
         @default: () => calledActionOn = SmartEnum_ItemsCollidingWithSwitchMapParameters.Default);

      calledActionOn.Should().Be(SmartEnum_ItemsCollidingWithSwitchMapParameters.Default);
      fallbackCalled.Should().BeFalse();
   }

   [Fact]
   public void Should_call_fallback_action_when_item_name_collides_with_fixed_parameter_name()
   {
      SmartEnum_ItemsCollidingWithSwitchMapParameters calledActionOn = null;

      SmartEnum_ItemsCollidingWithSwitchMapParameters.State.SwitchPartially(
         @default1: item => calledActionOn = item,
         @default: () => calledActionOn = SmartEnum_ItemsCollidingWithSwitchMapParameters.Default);

      calledActionOn.Should().Be(SmartEnum_ItemsCollidingWithSwitchMapParameters.State);
   }

   [Fact]
   public void Should_pass_state_to_callbacks_when_item_name_collides_with_state_parameter_name()
   {
      string passedState = null;

      SmartEnum_ItemsCollidingWithSwitchMapParameters.State.Switch(
         @state1: "state",
         @default: _ => throw new Exception("Wrong item."),
         @state: state => passedState = state);

      passedState.Should().Be("state");
   }

   [Fact]
   public void Should_pass_state_to_fallback_when_item_name_collides_with_state_parameter_name()
   {
      string passedState = null;

      SmartEnum_ItemsCollidingWithSwitchMapParameters.State.SwitchPartially(
         @state1: "state",
         @default1: (state, _) => passedState = state,
         @default: _ => throw new Exception("Wrong item."));

      passedState.Should().Be("state");
   }

   [Fact]
   public void Should_return_item_value_when_item_name_collides_with_fixed_parameter_name()
   {
      SmartEnum_ItemsCollidingWithSwitchMapParameters.Default
                                                     .MapPartially(@default1: 0, @default: 1, @state: 2)
                                                     .Should().Be(1);

      SmartEnum_ItemsCollidingWithSwitchMapParameters.State
                                                     .Map(@default: 1, @state: 2)
                                                     .Should().Be(2);
   }

   [Fact]
   public void Should_return_fallback_value_when_item_name_collides_with_fixed_parameter_name()
   {
      SmartEnum_ItemsCollidingWithSwitchMapParameters.State
                                                     .MapPartially(@default1: 0, @default: 1)
                                                     .Should().Be(0);
   }
}
