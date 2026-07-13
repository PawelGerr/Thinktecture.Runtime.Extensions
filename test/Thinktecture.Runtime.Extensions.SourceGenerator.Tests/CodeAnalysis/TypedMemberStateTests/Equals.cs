using System.Linq;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.TypedMemberStateTests;

// These tests exercise the real TypedMemberState.Equals implementation from real compilations. The equality tests
// of the derived states (KeyMemberState, DefaultMemberState) use hand-written fakes and therefore never run this
// code. A distinct focus is IsGenericComparable, added in commit 495c5a82, which drives the generic vs non-generic
// IComparable cast in the generated CompareTo code.
public class Equals : CompilationTestBase
{
   [Fact]
   public void Should_return_true_when_built_from_the_same_type()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public int P1 { get; }
               public int P2 { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p1 = c.GetMembers("P1").OfType<IPropertySymbol>().Single();
      var p2 = c.GetMembers("P2").OfType<IPropertySymbol>().Single();

      // Act
      var state1 = new TypedMemberState(p1.Type);
      var state2 = new TypedMemberState(p2.Type);

      // Assert
      state1.Equals(state2).Should().BeTrue("both states are built from the same type (int)");
   }

   [Fact]
   public void Should_return_false_when_generic_and_non_generic_comparable_differ()
   {
      // Arrange
      // Both compilations declare the SAME type 'N.Value', so the two states share TypeFullyQualified and differ
      // only in whether the struct implements the generic System.IComparable<T>. Therefore Equals can return false
      // only because of IsGenericComparable; IsComparable is true in both cases.
      var nonGenericSource = """
         using System;

         namespace N
         {
            public struct Value : IComparable
            {
               public int CompareTo(object obj) => 0;
            }
         }
         """;

      var genericSource = """
         using System;

         namespace N
         {
            public struct Value : IComparable<Value>
            {
               public int CompareTo(Value other) => 0;
            }
         }
         """;

      var nonGenericType = GetTypeSymbol(CreateCompilation(nonGenericSource), "N.Value");
      var genericType = GetTypeSymbol(CreateCompilation(genericSource), "N.Value");

      // Act
      var nonGenericState = new TypedMemberState(nonGenericType);
      var genericState = new TypedMemberState(genericType);

      // Assert
      nonGenericState.IsComparable.Should().BeTrue("the struct implements the non-generic System.IComparable");
      nonGenericState.IsGenericComparable.Should().BeFalse("the struct does not implement System.IComparable<T>");
      genericState.IsComparable.Should().BeTrue("the struct implements System.IComparable<Value>");
      genericState.IsGenericComparable.Should().BeTrue("the struct implements the generic System.IComparable<T>");

      nonGenericState.Equals(genericState).Should().BeFalse("the states differ only in IsGenericComparable");
   }
}
