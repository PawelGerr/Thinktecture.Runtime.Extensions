using System.Collections.Immutable;

namespace Thinktecture.Runtime.Tests.ImmutableArrayExtensionsTests;

public class FirstOrDefault
{
   [Fact]
   public void Should_return_default_value_if_array_is_default()
   {
      ImmutableArray<int> array = default;

      array.FirstOrDefault((i, arg) => true, 42)
           .Should().Be(0);
   }

   [Fact]
   public void Should_return_default_value_if_array_is_empty()
   {
      var array = ImmutableArray<int>.Empty;

      array.FirstOrDefault((i, arg) => true, 42)
           .Should().Be(0);
   }

   [Fact]
   public void Should_return_default_value_if_no_match()
   {
      var array = ImmutableArray.CreateRange(new[] { 1, 2, 3 });

      array.FirstOrDefault((i, arg) => false, 42)
           .Should().Be(0);
   }

   [Theory]
   [InlineData(1)]
   [InlineData(2)]
   [InlineData(3)]
   public void Should_return_default_value_matched_value(int matchedValue)
   {
      var array = ImmutableArray.CreateRange(new[] { 1, 2, 3 });

      array.FirstOrDefault((i, arg) => i == matchedValue && arg == 42, 42)
           .Should().Be(matchedValue);
   }
}
