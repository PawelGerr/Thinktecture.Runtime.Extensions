using System;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ComparableCodeGeneratorTests;

public class GenerateImplementation
{
   [Fact]
   public void GenerateImplementation_WithEmptyStringBuilder_AppendsContent()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      sb.Length.Should().BeGreaterThan(0);
   }

   [Fact]
   public async Task WithDefaultGenerator_AndReferenceType_GeneratesNullCheckInCompareTo()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.MyClass", "MyClass")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("if(obj is null)");
      result.Should().Contain("return 1;");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithDefaultGenerator_AndValueType_DoesNotGenerateNullCheckInGenericCompareTo()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.MyStruct", "MyStruct")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      // Object CompareTo always has null check
      result.Should().Contain("public int CompareTo(object? obj)");
      result.Should().Contain("if(obj is null)"); // In object CompareTo

      // Extract the generic CompareTo method and verify it doesn't have null check for value types
      var genericCompareToIndex = result.IndexOf("public int CompareTo(global::Thinktecture.Tests.MyStruct obj)", StringComparison.Ordinal);
      genericCompareToIndex.Should().BeGreaterThan(0);

      var nextMethodIndex = result.IndexOf("public", genericCompareToIndex + 10, StringComparison.Ordinal);
      var genericCompareToEnd = nextMethodIndex > 0 ? nextMethodIndex : result.Length;
      var genericCompareTo = result.Substring(genericCompareToIndex, genericCompareToEnd - genericCompareToIndex);

      genericCompareTo.Should().NotContain("if(obj is null)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithDefaultGenerator_AndStringKey_UsesStringComparerOrdinalIgnoreCase()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductId", "ProductId")
                  .WithStringKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.StringComparer.OrdinalIgnoreCase.Compare");
      result.Should().Contain("this._value, obj._value");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithDefaultGenerator_AndNonStringKey_UsesIComparable()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Amount", "Amount")
                  .WithIntKeyMember("_value")
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("((global::System.IComparable<global::System.Int32>)this._value).CompareTo(obj._value)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithCustomComparerAccessor_UsesProvidedComparer()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.ProductGroup", "ProductGroup")
                  .WithStringKeyMember("_key")
                  .Build();

      var sb = new StringBuilder();
      var generator = new ComparableCodeGenerator("global::Thinktecture.Tests.ProductGroup.KeyMemberComparer");

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::Thinktecture.Tests.ProductGroup.KeyMemberComparer.Comparer.Compare");
      result.Should().Contain("this._key, obj._key");
      result.Should().NotContain("StringComparer");
      result.Should().NotContain("IComparable");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithCustomComparerAccessor_AndIntKey_UsesCustomComparer()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.CustomType", "CustomType")
                  .WithIntKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = new ComparableCodeGenerator("global::Thinktecture.Tests.CustomType.Comparer");

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::Thinktecture.Tests.CustomType.Comparer.Comparer.Compare");
      result.Should().Contain("this.Value, obj.Value");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ErrorMessage_ContainsTypeName()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.MyCustomType", "MyCustomType")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("Argument must be of type \\\"MyCustomType\\\"");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithComplexGenericTypeName_GeneratesCorrectly()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.Container<global::System.String>.Inner", "Inner")
                  .WithStringKeyMember("_id")
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task WithSpecialCharactersInTypeName_EscapesCorrectly()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.@Special.@Type", "Type")
                  .WithIntKeyMember("@value")
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      await Verifier.Verify(sb.ToString());
   }

   [Fact]
   public async Task WithGuidKey_UsesIComparable()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.OrderId", "OrderId")
                  .WithGuidKeyMember("_id")
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("((global::System.IComparable<global::System.Guid>)this._id).CompareTo(obj._id)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithDifferentKeyMemberNames_GeneratesCorrectly()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestType")
                  .WithIntKeyMember("CustomKeyName")
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("this.CustomKeyName").And.Contain("obj.CustomKeyName");
      result.Should().Contain("CompareTo(obj.CustomKeyName)");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithReferenceTypeAndCustomComparer_GeneratesNullCheck()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithReferenceType("global::Thinktecture.Tests.TestClass", "TestClass")
                  .WithStringKeyMember("Name")
                  .Build();

      var sb = new StringBuilder();
      var generator = new ComparableCodeGenerator("Comparer");

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("if(obj is null)");
      result.Should().Contain("Comparer.Comparer.Compare");
      await Verifier.Verify(result);
   }

   [Fact]
   public async Task WithValueTypeAndStringKey_GeneratesStringComparer()
   {
      // Arrange
      var state = new ComparableStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.TestStruct", "TestStruct")
                  .WithStringKeyMember("Value")
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("global::System.StringComparer.OrdinalIgnoreCase.Compare");
      result.Should().Contain("public int CompareTo(object? obj)"); // Object CompareTo is always generated

      // Verify generic CompareTo doesn't have null check for value types
      var genericCompareToIndex = result.IndexOf("public int CompareTo(global::Thinktecture.Tests.TestStruct obj)", StringComparison.Ordinal);
      genericCompareToIndex.Should().BeGreaterThan(0);

      var nextMethodIndex = result.IndexOf("public", genericCompareToIndex + 10, StringComparison.Ordinal);
      var genericCompareToEnd = nextMethodIndex > 0 ? nextMethodIndex : result.Length;
      var genericCompareTo = result.Substring(genericCompareToIndex, genericCompareToEnd - genericCompareToIndex);
      genericCompareTo.Should().NotContain("if(obj is null)");

      await Verifier.Verify(result);
   }

   [Fact]
   public async Task ObjectCompareTo_AlwaysHasNullCheck()
   {
      // Arrange - Even value types should have null check in object CompareTo
      var state = new ComparableStateBuilder()
                  .WithValueType("global::Thinktecture.Tests.MyStruct", "MyStruct")
                  .WithIntKeyMember()
                  .Build();

      var sb = new StringBuilder();
      var generator = ComparableCodeGenerator.Default;

      // Act
      generator.GenerateImplementation(sb, state);

      // Assert
      var result = sb.ToString();
      result.Should().Contain("public int CompareTo(object? obj)");
      result.Should().Contain("if(obj is null)");
      await Verifier.Verify(result);
   }
}
