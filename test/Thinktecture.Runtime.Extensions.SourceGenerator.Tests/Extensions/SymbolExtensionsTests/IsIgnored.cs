using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.SymbolExtensionsTests;

public class IsIgnored
{
   public class Field
   {
      [Fact]
      public void Returns_true_for_IgnoreMemberAttribute()
      {
         var src = @"
using Thinktecture;

namespace Test;

public class C
{
   [IgnoreMember]
   public int F1;
}
";
         var type = GetTypeSymbol(src);
         var field = GetField(type, "F1");

         var result = field.IsIgnored();

         result.Should().BeTrue();
      }

      [Fact]
      public void Returns_true_for_ValueObjectMemberIgnoreAttribute()
      {
         var src = @"
using Thinktecture;

namespace Test;

public class C
{
   [ValueObjectMemberIgnore]
   public int F2;
}
";
         var type = GetTypeSymbol(src);
         var field = GetField(type, "F2");

         var result = field.IsIgnored();

         result.Should().BeTrue();
      }

      [Fact]
      public void Returns_false_for_other_attribute()
      {
         var src = @"
using System;

namespace Test;

public class C
{
   [Obsolete]
   public int F3;
}
";
         var type = GetTypeSymbol(src);
         var field = GetField(type, "F3");

         var result = field.IsIgnored();

         result.Should().BeFalse();
      }

      [Fact]
      public void Returns_false_when_no_attribute()
      {
         var src = @"
namespace Test;

public class C
{
   public int F4;
}
";
         var type = GetTypeSymbol(src);
         var field = GetField(type, "F4");

         var result = field.IsIgnored();

         result.Should().BeFalse();
      }

      [Fact]
      public void Returns_false_for_unknown_attribute_with_error_type()
      {
         var src = @"
namespace Test;
public class C
{
   [UnknownAttribute] // not defined - ErrorTypeSymbol
   public int F5;
}
";
         var type = GetTypeSymbol(src);
         var field = GetField(type, "F5");

         var result = field.IsIgnored();

         result.Should().BeFalse();
      }

      private static IFieldSymbol GetField(INamedTypeSymbol type, string name)
      {
         var field = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(f => f.Name == name);

         return field ?? throw new InvalidOperationException($"Field '{name}' not found.");
      }
   }

   public class Property
   {
      [Fact]
      public void Returns_true_for_IgnoreMemberAttribute()
      {
         var src = @"
using Thinktecture;

namespace Test;

public class C
{
   [IgnoreMember]
   public int P1 { get; }
}
";
         var type = GetTypeSymbol(src);
         var prop = GetProperty(type, "P1");

         var result = prop.IsIgnored();

         result.Should().BeTrue();
      }

      [Fact]
      public void Returns_true_for_ValueObjectMemberIgnoreAttribute()
      {
         var src = @"
using Thinktecture;
namespace Test;
public class C
{
   [ValueObjectMemberIgnore]
   public int P2 { get; }
}
";
         var type = GetTypeSymbol(src);
         var prop = GetProperty(type, "P2");

         var result = prop.IsIgnored();

         result.Should().BeTrue();
      }

      [Fact]
      public void Returns_false_for_other_attribute()
      {
         var src = @"
using System;
namespace Test;
public class C
{
   [Obsolete]
   public int P3 { get; }
}
";
         var type = GetTypeSymbol(src);
         var prop = GetProperty(type, "P3");

         var result = prop.IsIgnored();

         result.Should().BeFalse();
      }

      [Fact]
      public void Returns_false_when_no_attribute()
      {
         var src = @"
namespace Test;

public class C
{
   public int P4 { get; }
}
";
         var type = GetTypeSymbol(src);
         var prop = GetProperty(type, "P4");

         var result = prop.IsIgnored();

         result.Should().BeFalse();
      }

      [Fact]
      public void Returns_false_for_unknown_attribute_with_error_type()
      {
         var src = @"
namespace Test;
public class C
{
   [UnknownAttribute] // not defined - ErrorTypeSymbol
   public int P5 { get; }
}
";
         var type = GetTypeSymbol(src);
         var prop = GetProperty(type, "P5");

         var result = prop.IsIgnored();

         result.Should().BeFalse();
      }

      private static IPropertySymbol GetProperty(INamedTypeSymbol type, string name)
      {
         var prop = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => p.Name == name);

         return prop ?? throw new InvalidOperationException($"Property '{name}' not found.");
      }
   }

   private static INamedTypeSymbol GetTypeSymbol(string source, string metadataName = "Test.C")
   {
      _ = typeof(IgnoreMemberAttribute);
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "IsIgnored_FieldTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var type = compilation.GetTypeByMetadataName(metadataName);

      return type ?? throw new InvalidOperationException($"Type '{metadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");
   }
}
