using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using MessagePack;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Xunit.Abstractions;

namespace Thinktecture
{
   public class EnumSourceGeneratorTests
   {
      private readonly ITestOutputHelper _output;

      public EnumSourceGeneratorTests(ITestOutputHelper output)
      {
         _output = output ?? throw new ArgumentNullException(nameof(output));
      }

      [Fact]
      public void Should_generate_TypeConverterAttribute_to_enum_without_explicit_key()
      {
         string source = @"
using System;

namespace Thinktecture.EnumLikeClass
{
   //[EnumGeneration(KeyComparerProvidingMember = ""_equalityComparer"")]
	public sealed partial class ProductCategory : IEnum<string>
	{
		public static readonly ProductCategory Fruits = new(""Fruits"");
      public static readonly ProductCategory Dairy = new(""Dairy"");

      private static ProductCategory CreateInvalidItem(string key)
      {
         return new(key)
                {
                   IsValid = false
                };
      }
   }
}
";
         string output = GetGeneratedOutput<EnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      }

      [Fact]
      public void Should_generate_struct()
      {
         string source = @"
using System;

namespace Thinktecture.EnumLikeClass
{
   public partial struct EnumWithDerivedType : IValidatableEnum<int>
   {
      public static readonly EnumWithDerivedType Item1 = new(1);
      public static readonly EnumWithDerivedType ItemOfDerivedType = new DerivedEnum(2);
   }
}
";
         string output = GetGeneratedOutput<EnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      }

      [Fact]
      public void Should_generate()
      {
         string source = @"
using System;

namespace Thinktecture.EnumLikeClass
{
   [EnumGeneration(KeyPropertyName = ""Name"")]
	public sealed partial class ProductCategory : IEnum<int>
	{
		public static readonly ProductCategory Fruits = new(1);
      public static readonly ProductCategory Dairy = new(2);

      private static ProductCategory CreateInvalidItem(int key)
      {
         return new(key)
                {
                   IsValid = false
                };
      }
   }
}
";
         string output = GetGeneratedOutput<EnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      }

      [Fact]
      public void Should_generate2()
      {
         string source = @"
using System;

namespace Thinktecture.EnumLikeClass
{
	public sealed partial class ProductCategory : IEnum<int>
	{
		public static readonly ProductCategory Fruits = new(1);
      public static readonly ProductCategory Dairy = new(2);

      private static ProductCategory CreateInvalidItem(int key)
      {
         return new(key)
                {
                   IsValid = false
                };
      }
   }
}
";
         string output = GetGeneratedOutput<EnumJsonConverterSourceGenerator>(source, typeof(IEnum<>).Assembly);
      }

      [Fact]
      public void Should_generate3()
      {
         string source = @"
using System;
using MessagePack;

namespace Thinktecture.EnumLikeClass
{
   [MessagePackFormatter(typeof(ProductCategory_EnumMessagePackFormatter))]
	public sealed partial class ProductCategory : IEnum<int>
	{
		public static readonly ProductCategory Fruits = new(1);
      public static readonly ProductCategory Dairy = new(2);

      private static ProductCategory CreateInvalidItem(int key)
      {
         return new(key)
                {
                   IsValid = false
                };
      }
   }
}
";
         string output = GetGeneratedOutput<EnumMessagePackFormatterSourceGenerator>(source, typeof(IEnum<>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);
      }

      [Fact]
      public void Should_generate_TypeConverterAttribute_to_enum_with_explicit_key()
      {
         string source = @"
namespace Thinktecture.EnumLikeClass
{
	public sealed partial class ProductGroup : IEnum<ProductGroup, int>
	{
		public static readonly ProductGroup Apple = new(1, ""Apple"");
      public static readonly ProductGroup Orange = new(2, ""Orange"");

      public string DisplayName { get; }

      private ProductGroup(int key, string displayName)
         : base(key)
      {
         DisplayName = displayName;
      }

      protected override ProductGroup CreateInvalid(int key)
      {
         // the values can be anything besides the key,
         // the key must not be null
         return new(key, ""Unknown product group"");
      }
   }
}

";
         string output = GetGeneratedOutput<EnumSourceGenerator>(source);
      }

      [Fact]
      public void Should_generate_enum_with_derivedType()
      {
         string source = @"
namespace Thinktecture.EnumLikeClass
{
	public partial class EnumWithDerivedType : IEnum<int>
   {
      public static readonly EnumWithDerivedType Item1 = new(1);
      public static readonly EnumWithDerivedType ItemOfDerivedType = new DerivedEnum(2);

      private class DerivedEnum : EnumWithDerivedType
      {
         public DerivedEnum(int key)
            : base(key)
         {
         }
      }
   }
}
";

         string output = GetGeneratedOutput<EnumSourceGenerator>(source);
      }

      private string GetGeneratedOutput<T>(string source, params Assembly[] furtherAssemblies)
         where T : ISourceGenerator, new()
      {
         var syntaxTree = CSharpSyntaxTree.ParseText(source);
         var assemblies = new HashSet<Assembly>(AppDomain.CurrentDomain.GetAssemblies())
                          {
                             typeof(T).Assembly
                          };

         foreach (var furtherAssembly in furtherAssemblies)
         {
            assemblies.Add(furtherAssembly);
         }

         var references = assemblies.Where(assembly => !assembly.IsDynamic)
                                    .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                                    .Cast<MetadataReference>();

         var compilation = CSharpCompilation.Create("SourceGeneratorTests", new[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

         // var compileDiagnostics = compilation.GetDiagnostics();
         // compileDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

         var generator = new T();
         CSharpGeneratorDriver.Create(generator).RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generateDiagnostics);

         var errors = generateDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
         errors.Should().BeEmpty();

         string output = outputCompilation.SyntaxTrees.Last().ToString();

         _output.WriteLine(output);

         return output;
      }
   }
}
