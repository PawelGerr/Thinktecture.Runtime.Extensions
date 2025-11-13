using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.SymbolExtensionsTests;

public class IsValidateFactoryArgumentsImplementation
{
   [Fact]
   public void Returns_true_and_sets_method_for_static_void_method_with_matching_name()
   {
      var src = @"
#nullable enable
namespace Test;
public class C
{
   public static void ValidateFactoryArguments(ref object? validationError, ref int value) { }
}
";
      var type = GetTypeSymbol(src);
      var member = type.GetMembers("ValidateFactoryArguments").OfType<IMethodSymbol>().First();

      var result = member.IsValidateFactoryArgumentsImplementation(out var method);

      result.Should().BeTrue();
      method.Should().Be(method);
   }

   [Fact]
   public void Returns_true_for_static_non_void_method_with_matching_name()
   {
      var src = @"
#nullable enable
namespace Test;
public class C
{
   public static int ValidateFactoryArguments(ref object? validationError, ref int value) => 0;
}
";
      var type = GetTypeSymbol(src);
      var member = type.GetMembers("ValidateFactoryArguments").OfType<IMethodSymbol>().First();

      var result = member.IsValidateFactoryArgumentsImplementation(out var method);

      result.Should().BeTrue();
      method.Should().Be(member);
   }

   [Fact]
   public void Returns_false_for_static_generic_method_with_matching_name()
   {
      var src = @"
namespace Test;
public class C
{
   public static T ValidateFactoryArguments<T>(T value) => value;
}
";
      var type = GetTypeSymbol(src);
      var member = type.GetMembers("ValidateFactoryArguments").OfType<IMethodSymbol>().First();

      var result = member.IsValidateFactoryArgumentsImplementation(out var method);

      result.Should().BeFalse();
      method.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_instance_method_with_matching_name()
   {
      var src = @"
namespace Test;
public class C
{
   public void ValidateFactoryArguments() { }
}
";
      var type = GetTypeSymbol(src);
      var member = type.GetMembers("ValidateFactoryArguments").OfType<IMethodSymbol>().First();

      var result = member.IsValidateFactoryArgumentsImplementation(out var method);

      result.Should().BeFalse();
      method.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_static_method_with_different_name()
   {
      var src = @"
namespace Test;
public class C
{
   public static void SomethingElse() { }
}
";
      var type = GetTypeSymbol(src);
      var member = type.GetMembers("SomethingElse").OfType<IMethodSymbol>().First();

      var result = member.IsValidateFactoryArgumentsImplementation(out var method);

      result.Should().BeFalse();
      method.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_non_method_member_even_if_name_matches()
   {
      var src = @"
namespace Test;
public class C
{
   public static int ValidateFactoryArguments => 42;
}
";
      var type = GetTypeSymbol(src);
      var member = type.GetMembers("ValidateFactoryArguments").First(m => m is not IMethodSymbol);

      var result = member.IsValidateFactoryArgumentsImplementation(out var method);

      result.Should().BeFalse();
      method.Should().BeNull();
   }

   private static INamedTypeSymbol GetTypeSymbol(string source, string metadataName = "Test.C")
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "IsValidateFactoryArgumentsImplementationTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var type = compilation.GetTypeByMetadataName(metadataName);

      return type ?? throw new InvalidOperationException($"Type '{metadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");
   }
}
