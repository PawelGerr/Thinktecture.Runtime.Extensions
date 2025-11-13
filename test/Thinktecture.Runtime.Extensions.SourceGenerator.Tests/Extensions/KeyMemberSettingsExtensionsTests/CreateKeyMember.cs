using NSubstitute;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.KeyMemberSettingsExtensionsTests;

public class CreateKeyMember
{
   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Private, Thinktecture.CodeAnalysis.MemberKind.Field, "myField")]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Public, Thinktecture.CodeAnalysis.MemberKind.Property, "MyProperty")]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Protected, Thinktecture.CodeAnalysis.MemberKind.Field, "_value")]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Internal, Thinktecture.CodeAnalysis.MemberKind.Property, "Value")]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.PrivateProtected, Thinktecture.CodeAnalysis.MemberKind.Field, "_keyMember")]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.ProtectedInternal, Thinktecture.CodeAnalysis.MemberKind.Property, "KeyMember")]
   public void Should_create_KeyMemberState_with_properties_from_settings(
      Thinktecture.CodeAnalysis.AccessModifier accessModifier,
      Thinktecture.CodeAnalysis.MemberKind memberKind,
      string memberName)
   {
      // Arrange
      var settings = Substitute.For<IKeyMemberSettings>();
      settings.KeyMemberAccessModifier.Returns(accessModifier);
      settings.KeyMemberKind.Returns(memberKind);
      settings.KeyMemberName.Returns(memberName);

      var keyMemberState = Substitute.For<ITypedMemberState>();

      // Act
      var result = settings.CreateKeyMember(keyMemberState);

      // Assert
      result.AccessModifier.Should().Be(accessModifier);
      result.Kind.Should().Be(memberKind);
      result.Name.Should().Be(memberName);
   }

   [Fact]
   public void Should_create_ArgumentName_from_KeyMemberName()
   {
      // Arrange
      var settings = Substitute.For<IKeyMemberSettings>();
      settings.KeyMemberAccessModifier.Returns(Thinktecture.CodeAnalysis.AccessModifier.Public);
      settings.KeyMemberKind.Returns(Thinktecture.CodeAnalysis.MemberKind.Property);
      settings.KeyMemberName.Returns("Value");

      var keyMemberState = Substitute.For<ITypedMemberState>();

      // Act
      var result = settings.CreateKeyMember(keyMemberState);

      // Assert
      result.ArgumentName.Should().Be(ArgumentName.Create("Value"));
   }

   [Fact]
   public void Should_pass_through_ITypedMemberState_properties()
   {
      // Arrange
      var settings = Substitute.For<IKeyMemberSettings>();
      settings.KeyMemberAccessModifier.Returns(Thinktecture.CodeAnalysis.AccessModifier.Public);
      settings.KeyMemberKind.Returns(Thinktecture.CodeAnalysis.MemberKind.Property);
      settings.KeyMemberName.Returns("Value");

      var keyMemberState = Substitute.For<ITypedMemberState>();
      keyMemberState.TypeFullyQualified.Returns("System.String");
      keyMemberState.IsReferenceType.Returns(true);
      keyMemberState.SpecialType.Returns(Microsoft.CodeAnalysis.SpecialType.System_String);

      // Act
      var result = settings.CreateKeyMember(keyMemberState);

      // Assert
      result.TypeFullyQualified.Should().Be("System.String");
      result.IsReferenceType.Should().BeTrue();
      result.SpecialType.Should().Be(Microsoft.CodeAnalysis.SpecialType.System_String);
   }
}
