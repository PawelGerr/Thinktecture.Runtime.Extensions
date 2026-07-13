using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Thinktecture.CodeAnalysis.AdHocUnions;
using Thinktecture.CodeAnalysis.Annotations;
using Thinktecture.CodeAnalysis.RegularUnions;
using Thinktecture.CodeAnalysis.SmartEnums;
using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

// Regression tests for keyword-named identifiers (findings 13, 14 and 15).
//
// Roslyn symbol names never carry the '@' escape, so a user-declared member, key member or delegate-method
// parameter whose name is a C# reserved keyword (e.g. 'default' or 'class') must be re-escaped with '@' at
// emit time; otherwise the generated code does not compile. In addition, a KeyMemberName that renders to the
// fixed 'item' identifier used by the generated lookup methods must not collide with it.
//
// Each test compiles the generated output and asserts that there are no compiler errors. Before the fix the
// generated code contained syntax errors (CS1001/CS1041) or duplicate identifiers (CS0100/CS0136).
public class KeywordIdentifierEscapingTests : SourceGeneratorTestsBase
{
   public KeywordIdentifierEscapingTests(ITestOutputHelper output)
      : base(output, 100_000)
   {
   }

   [Fact]
   public void Should_escape_keyword_named_delegate_method_parameter() // finding 13
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;

               [UseDelegateFromConstructor]
               public partial string GetText(string @default);
            }
         }
         """;

      AssertGeneratedCodeCompiles<SmartEnumSourceGenerator>(source);
   }

   [Fact]
   public void Should_escape_keyword_named_member_of_complex_value_object() // finding 14
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            public partial class TestValueObject
            {
               public string @default { get; }
               public int @class { get; }
            }
         }
         """;

      AssertGeneratedCodeCompiles<ValueObjectSourceGenerator>(source);
   }

   [Fact]
   public void Should_not_collide_when_key_member_is_named_Item() // finding 15
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(KeyMemberName = "Item")]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      AssertGeneratedCodeCompiles<SmartEnumSourceGenerator>(source);
   }

   [Fact]
   public void Should_not_collide_when_key_member_renders_to_Item() // finding 15 (underscore variant)
   {
      // KeyMemberName "_item" renders to the identifier 'item' (leading underscore dropped before a letter), which
      // collides with the fixed 'item' parameter/out-local of the generated lookup methods. A raw-name comparison
      // ("_item" vs "item") missed the collision, so the generated code declared 'item' twice (CS0100/CS0136).
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(KeyMemberName = "_item")]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      AssertGeneratedCodeCompiles<SmartEnumSourceGenerator>(source);
   }

   [Fact]
   public void Should_not_collide_when_smart_enum_key_member_renders_to_provider() // finding: Validate provider-parameter collision
   {
      // KeyMemberName "_provider" renders to the identifier 'provider' (leading underscore dropped before a
      // letter), which collides with the fixed 'provider' parameter of the generated Validate methods. A raw-name
      // comparison ("_provider" vs "provider") missed the collision, so Validate declared 'provider' twice (CS0100).
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(KeyMemberName = "_provider")]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      AssertGeneratedCodeCompiles<SmartEnumSourceGenerator>(source);
   }

   [Fact]
   public void Should_compile_when_smart_enum_is_nested_in_partial_interface() // finding: nested-in-interface
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public partial interface IShipping
            {
               [SmartEnum<int>]
               public partial class Method
               {
                  public static readonly Method Standard = default!;
                  public static readonly Method Express = default!;
               }
            }
         }
         """;

      AssertGeneratedCodeCompiles<SmartEnumSourceGenerator>(source);
   }

   [Fact]
   public void Should_not_collide_when_regular_union_derived_type_is_named_State() // finding: Switch/Map parameter collision
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record TestUnion
            {
               public sealed record State : TestUnion;
               public sealed record Verbose : TestUnion;
            }
         }
         """;

      AssertGeneratedCodeCompiles<RegularUnionSourceGenerator>(source);
   }

   [Fact]
   public void Should_not_collide_when_regular_union_derived_type_is_named_Value() // finding: Switch/Map pattern-local collision
   {
      // A derived type named 'Value' nested directly in the union renders its callback parameter to the identifier
      // 'value', which is the same identifier the Switch/Map bodies declare as their fixed pattern local
      // ('case ... value:'). A raw comparison never guarded the pattern local, so the parameter 'value' and the
      // pattern variable 'value' collided in the same scope (CS0136).
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record TestUnion
            {
               public sealed record Value : TestUnion;
               public sealed record Verbose : TestUnion;
            }
         }
         """;

      AssertGeneratedCodeCompiles<RegularUnionSourceGenerator>(source);
   }

   private static void AssertGeneratedCodeCompiles<T>(string source)
      where T : IIncrementalGenerator, new()
   {
      var syntaxTree = CSharpSyntaxTree.ParseText(source);

      // Reference the full framework closure from the trusted platform assemblies so that the generated code
      // (which uses e.g. System.Linq.Expressions) compiles, then add the Thinktecture runtime library.
      var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
                       .Split(Path.PathSeparator)
                       .Where(p => !string.IsNullOrEmpty(p) && !p.Contains("Thinktecture"))
                       .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
                       .Append(MetadataReference.CreateFromFile(typeof(ISmartEnum<>).Assembly.Location))
                       .ToList();

      var compilation = CSharpCompilation.Create("SourceGeneratorTests",
                                                 [syntaxTree],
                                                 references,
                                                 new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      // Precondition: the user-written source itself is valid. The partial delegate methods have no defining
      // declaration in this isolated compilation, so CS8795 is expected before the generator runs.
      compilation.GetDiagnostics()
                 .Where(d => d.Severity == DiagnosticSeverity.Error && d.Id != "CS8795")
                 .Should().BeEmpty();

      // The Annotations generator emits the JetBrains 'InstantHandleAttribute' into the compiled assembly; the
      // Switch/Map methods generated for smart enums reference it, so it must run alongside the main generator.
      CSharpGeneratorDriver.Create(new T(), new AnnotationsSourceGenerator())
                           .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

      outputCompilation.GetDiagnostics()
                       .Where(d => d.Severity == DiagnosticSeverity.Error)
                       .Should().BeEmpty();
   }
}
