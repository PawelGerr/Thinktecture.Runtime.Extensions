using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.Collections;

public class SingleItemReadOnlyDictionaryTests
{
   private readonly IReadOnlyDictionary<int, int> _sut = SingleItem.Dictionary(42, 43);

   [Fact]
   public void Should_have_count_of_1()
   {
      _sut.Count.Should().Be(1);
   }

   [Fact]
   public void Should_have_correct_indexer_behavior()
   {
      _sut.Invoking(sut => sut[-1]).Should().Throw<KeyNotFoundException>().WithMessage("The given key '-1' was not present in the dictionary.");
      _sut.Invoking(sut => sut[1]).Should().Throw<KeyNotFoundException>().WithMessage("The given key '1' was not present in the dictionary.");

      _sut[42].Should().Be(43);
   }

   [Fact]
   public void Should_have_ContainsKey()
   {
      _sut.ContainsKey(0).Should().BeFalse();
      _sut.ContainsKey(42).Should().BeTrue();
      _sut.ContainsKey(43).Should().BeFalse();
   }

   [Fact]
   public void Should_have_TryGetValue()
   {
      _sut.TryGetValue(0, out var value).Should().BeFalse();
      value.Should().Be(0);

      _sut.TryGetValue(42, out value).Should().BeTrue();
      value.Should().Be(43);

      _sut.TryGetValue(43, out value).Should().BeFalse();
      value.Should().Be(0);
   }

   [Fact]
   public void Should_have_correct_enumerator_behavior()
   {
      using var enumerator = _sut.GetEnumerator();
      var defaultKvp = default(KeyValuePair<int, int>);

      enumerator.Current.Should().Be(defaultKvp);

      enumerator.MoveNext().Should().BeTrue();
      enumerator.Current.Should().Be(new KeyValuePair<int, int>(42, 43));

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(defaultKvp);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(defaultKvp);

      enumerator.Reset();

      enumerator.Current.Should().Be(defaultKvp);

      enumerator.MoveNext().Should().BeTrue();
      enumerator.Current.Should().Be(new KeyValuePair<int, int>(42, 43));

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(defaultKvp);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(defaultKvp);
   }

   [Fact]
   public void Should_have_correct_keys_enumerator_behavior()
   {
      using var enumerator = _sut.Keys.GetEnumerator();

      enumerator.Current.Should().Be(42);

      enumerator.MoveNext().Should().BeTrue();
      enumerator.Current.Should().Be(42);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(42);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(42);

      enumerator.Reset();

      enumerator.Current.Should().Be(42);

      enumerator.MoveNext().Should().BeTrue();
      enumerator.Current.Should().Be(42);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(42);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(42);
   }

   [Fact]
   public void Should_have_correct_values_enumerator_behavior()
   {
      using var enumerator = _sut.Values.GetEnumerator();

      enumerator.Current.Should().Be(43);

      enumerator.MoveNext().Should().BeTrue();
      enumerator.Current.Should().Be(43);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(43);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(43);

      enumerator.Reset();

      enumerator.Current.Should().Be(43);

      enumerator.MoveNext().Should().BeTrue();
      enumerator.Current.Should().Be(43);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(43);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(43);
   }
}
