#nullable enable
using System;
using System.Reflection;

namespace Thinktecture.Runtime.Tests.AdHocUnions;

// Local test unions so the test is self-contained and independent of shared-type churn.
[Union<int, string>(ValueMemberAccessModifier = AccessModifier.Private)]
public partial class PrivateValueUnion;

[Union<int, string>(ValueMemberAccessModifier = AccessModifier.Internal)]
public partial class InternalValueUnion;

[Union<int, string>]
public partial class PublicValueUnion;

public class AdHocUnionValueMemberAccessModifierTests
{
   private static PropertyInfo GetValueProperty(Type type)
      => type.GetProperty("Value", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;

   [Fact]
   public void Should_render_Value_as_non_public_when_ValueMemberAccessModifier_is_Private()
   {
      var property = GetValueProperty(typeof(PrivateValueUnion));

      property.Should().NotBeNull();
      property.GetMethod!.IsPublic.Should().BeFalse();
      property.GetMethod!.IsPrivate.Should().BeTrue();
   }

   [Fact]
   public void Should_render_Value_as_non_public_when_ValueMemberAccessModifier_is_Internal()
   {
      var property = GetValueProperty(typeof(InternalValueUnion));

      property.Should().NotBeNull();
      property.GetMethod!.IsPublic.Should().BeFalse();
      property.GetMethod!.IsAssembly.Should().BeTrue();
   }

   [Fact]
   public void Should_render_Value_as_public_when_ValueMemberAccessModifier_is_default()
   {
      var property = GetValueProperty(typeof(PublicValueUnion));

      property.Should().NotBeNull();
      property.GetMethod!.IsPublic.Should().BeTrue();
   }
}
