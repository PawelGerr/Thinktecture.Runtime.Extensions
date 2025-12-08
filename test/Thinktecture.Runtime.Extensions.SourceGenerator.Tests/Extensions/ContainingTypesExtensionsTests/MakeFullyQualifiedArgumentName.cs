using System.Collections.Immutable;
using System.Text;
using Thinktecture.CodeAnalysis;
using System.Linq;

namespace Thinktecture.Runtime.Tests.ContainingTypesExtensionsTests;

public class MakeFullyQualifiedArgumentName
{
   [Theory]
   [InlineData(new string[0], "Foo", false, "foo")]
   [InlineData(new[] { "Outer" }, "Foo", false, "outerFoo")]
   [InlineData(new[] { "Outer", "Inner" }, "Foo", false, "outerInnerFoo")]
   [InlineData(new[] { "Outer", "Inner" }, "Foo", true, "innerFoo")]
   [InlineData(new string[0], "_Name", false, "_name")]
   [InlineData(new string[0], "ABC", false, "abc")]
   [InlineData(new string[0], "_ABC", false, "_abc")]
   [InlineData(new string[0], "_9Value", false, "_9value")] // note: first visible char '9' stays; underscore kept
   [InlineData(new string[0], "", false, "")]
   [InlineData(new string[0], "URLValue", false, "urlValue")]
   [InlineData(new string[0], "IPAddress", false, "ipAddress")]
   [InlineData(new string[0], "ICode", false, "iCode")]
   [InlineData(new[] { "Outer", "Inner" }, "Value", false, "outerInnerValue")]
   [InlineData(new[] { "Outer" }, "Value", true, "value")]
   public void MakeFullyQualifiedArgumentName_Works(string[] types, string member, bool skipRoot, string expected)
   {
      var list = types.Select(n => new ContainingTypeState(n, true, false, [])).ToImmutableArray();
      var sb = new StringBuilder("prefix"); // ensure restoration works
      var result = ContainingTypesExtensions.MakeFullyQualifiedArgumentName(list, member, skipLevels: skipRoot ? 1 : 0, sb);

      result.Should().Be(expected);
      sb.Length.Should().Be("prefix".Length); // sb is restored
   }
}
