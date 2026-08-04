#nullable enable
using System.Reflection;

namespace Thinktecture.Runtime.Tests.AdHocUnions;

// Local test unions so the test is self-contained and independent of shared-type churn.
[Union<int, string>(ValueMemberName = "RawValue")]
public partial class RenamedValueUnion;

// The generated member is renamed, which frees the Value identifier for the user's own property.
[Union<int, string>(ValueMemberName = "RawValue")]
public partial class RenamedValueUnionWithHandwrittenValue
{
   public string Value => RawValue?.ToString() ?? "";
}

public class AdHocUnionValueMemberNameTests
{
   [Fact]
   public void Should_generate_property_under_the_configured_name()
   {
      var property = typeof(RenamedValueUnion).GetProperty("RawValue", BindingFlags.Public | BindingFlags.Instance);

      property.Should().NotBeNull();
   }

   [Fact]
   public void Should_not_generate_a_property_named_Value_when_renamed()
   {
      var property = typeof(RenamedValueUnion).GetProperty("Value", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      property.Should().BeNull();
   }

   [Fact]
   public void Should_keep_the_hand_written_Value_property_alongside_the_renamed_member()
   {
      var type = typeof(RenamedValueUnionWithHandwrittenValue);

      type.GetProperty("RawValue", BindingFlags.Public | BindingFlags.Instance).Should().NotBeNull();

      var handWritten = type.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
      handWritten.Should().NotBeNull();
      handWritten!.PropertyType.Should().Be<string>();
   }
}
