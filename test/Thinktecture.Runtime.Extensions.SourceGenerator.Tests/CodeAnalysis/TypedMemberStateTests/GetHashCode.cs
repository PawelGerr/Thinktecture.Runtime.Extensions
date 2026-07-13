using System.Linq;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.TypedMemberStateTests;

// These tests exercise the real TypedMemberState.GetHashCode implementation from real compilations, including the
// IsGenericComparable member (commit 495c5a82). The derived states' hash-code tests use hand-written fakes and
// therefore never run this code.
public class GetHashCode : CompilationTestBase
{
   [Fact]
   public void Should_return_same_hash_code_when_built_from_the_same_type()
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
      var hash1 = new TypedMemberState(p1.Type).GetHashCode();
      var hash2 = new TypedMemberState(p2.Type).GetHashCode();

      // Assert
      hash1.Should().Be(hash2, "both states are built from the same type (int)");
   }

   [Fact]
   public void Should_return_different_hash_code_when_generic_and_non_generic_comparable_differ()
   {
      // Arrange
      // Both compilations declare the SAME type 'N.Value', so the two states share TypeFullyQualified and differ
      // only in whether the struct implements the generic System.IComparable<T>. Therefore the hash codes can
      // differ only because of IsGenericComparable; IsComparable is true in both cases.
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
      var nonGenericHash = new TypedMemberState(nonGenericType).GetHashCode();
      var genericHash = new TypedMemberState(genericType).GetHashCode();

      // Assert
      nonGenericHash.Should().NotBe(genericHash);
   }
}
