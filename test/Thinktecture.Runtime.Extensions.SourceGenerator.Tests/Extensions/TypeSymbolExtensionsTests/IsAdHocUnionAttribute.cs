using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsAdHocUnionAttribute
{
   [Fact]
   public void Should_return_true_for_generic_union_attribute_with_2_types()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union<string, int>]
public partial class TestUnion;
";
      var attributeType = GetAttributeType(src, "Test.TestUnion");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_generic_union_attribute_with_3_types()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union<string, int, bool>]
public partial class TestUnion;
";
      var attributeType = GetAttributeType(src, "Test.TestUnion");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_generic_union_attribute_with_4_types()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union<string, int, bool, double>]
public partial class TestUnion;
";
      var attributeType = GetAttributeType(src, "Test.TestUnion");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_generic_union_attribute_with_5_types()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union<string, int, bool, double, decimal>]
public partial class TestUnion;
";
      var attributeType = GetAttributeType(src, "Test.TestUnion");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_non_generic_ad_hoc_union_attribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[AdHocUnion]
public partial class TestUnion;
";
      var attributeType = GetAttributeType(src, "Test.TestUnion");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_union_attribute_on_struct()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union<string, int>]
public partial struct TestUnion;
";
      var attributeType = GetAttributeType(src, "Test.TestUnion");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_regular_union_attribute_without_generic_parameters()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union]
public partial class TestUnion;
";
      var attributeType = GetAttributeType(src, "Test.TestUnion");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_smart_enum_attribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum<int>]
public partial class TestEnum;
";
      var attributeType = GetAttributeType(src, "Test.TestEnum");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_value_object_attribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[ValueObject<int>]
public partial class TestValueObject;
";
      var attributeType = GetAttributeType(src, "Test.TestValueObject");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_complex_value_object_attribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[ComplexValueObject]
public partial class TestValueObject;
";
      var attributeType = GetAttributeType(src, "Test.TestValueObject");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_null_attribute_type()
   {
      ITypeSymbol? attributeType = null;

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_error_type()
   {
      var src = @"
namespace Test;

[NonExistentAttribute]
public class TestClass;
";
      var attributeType = GetAttributeType(src, "Test.TestClass");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_union_attribute_in_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace
{
   public class UnionAttribute<T1, T2> : System.Attribute;
}

namespace Test
{
   [WrongNamespace.Union<string, int>]
   public class TestClass;
}
";
      var attributeType = GetAttributeType(src, "Test.TestClass");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_ad_hoc_union_attribute_in_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace
{
   public class AdHocUnionAttribute : System.Attribute;
}

namespace Test
{
   [WrongNamespace.AdHocUnion]
   public class TestClass;
}
";
      var attributeType = GetAttributeType(src, "Test.TestClass");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_non_named_type_symbol()
   {
      var src = @"
namespace Test;

public class TestClass;
";
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(src, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "IsAdHocUnionAttributeTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      // Get an array type symbol (not a named type)
      var arrayType = compilation.CreateArrayTypeSymbol(compilation.GetSpecialType(SpecialType.System_Int32));

      var result = arrayType.IsAdHocUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_union_attribute_on_nested_class()
   {
      var src = @"
using Thinktecture;

namespace Test;

public class Outer
{
   [Union<string, int>]
   public partial class Inner;
}
";
      var attributeType = GetAttributeType(src, "Test.Outer+Inner");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_first_union_attribute_when_multiple_attributes_present()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union<string, int>]
[System.Serializable]
public partial class TestUnion;
";
      var attributeType = GetAttributeType(src, "Test.TestUnion");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_union_attribute_with_custom_types()
   {
      var src = @"
using Thinktecture;

namespace Test;

public class CustomType1;
public class CustomType2;

[Union<CustomType1, CustomType2>]
public partial class TestUnion;
";
      var attributeType = GetAttributeType(src, "Test.TestUnion");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_union_attribute_with_nullable_types()
   {
      var src = @"
#nullable enable
using Thinktecture;

namespace Test;

[Union<string?, int?>]
public partial class TestUnion;
";
      var attributeType = GetAttributeType(src, "Test.TestUnion");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_union_attribute_with_generic_types()
   {
      var src = @"
using Thinktecture;
using System.Collections.Generic;

namespace Test;

[Union<List<string>, Dictionary<int, string>>]
public partial class TestUnion;
";
      var attributeType = GetAttributeType(src, "Test.TestUnion");

      var result = attributeType.IsAdHocUnionAttribute();

      result.Should().BeTrue();
   }

   private static ITypeSymbol GetAttributeType(string source, string typeMetadataName)
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location))
                                .Concat([
                                   MetadataReference.CreateFromFile(typeof(UnionAttribute<,>).Assembly.Location),
                                ]);

      var compilation = CSharpCompilation.Create(
         "IsAdHocUnionAttributeTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName(typeMetadataName)
                 ?? throw new InvalidOperationException($"Type '{typeMetadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");

      var attribute = type.GetAttributes().FirstOrDefault();

      return attribute?.AttributeClass ?? throw new InvalidOperationException($"No attribute found on type '{typeMetadataName}'");
   }
}
