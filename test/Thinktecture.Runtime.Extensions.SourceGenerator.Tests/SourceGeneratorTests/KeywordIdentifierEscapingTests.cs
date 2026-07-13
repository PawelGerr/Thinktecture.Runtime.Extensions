using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using MessagePack;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using Thinktecture.CodeAnalysis.AdHocUnions;
using Thinktecture.CodeAnalysis.Annotations;
using Thinktecture.CodeAnalysis.RegularUnions;
using Thinktecture.CodeAnalysis.SmartEnums;
using Thinktecture.CodeAnalysis.ValueObjects;
using Thinktecture.Formatters;
using Thinktecture.Json;
using Thinktecture.Text.Json.Serialization;

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
   public void Should_not_collide_when_value_object_key_member_renders_to_fixed_factory_identifier() // finding: keyed value object factory-parameter collision
   {
      // The generated factory/validation methods declare fixed 'obj', 'validationError' and 'provider' parameters
      // and locals in the same scope as the user's key-member parameter (for example the partial
      // ValidateFactoryArguments declares both 'validationError' and the key member). A KeyMemberName that renders
      // to one of these identifiers (leading underscore dropped before a letter) collided with them
      // (CS0100/CS0136). A raw-name comparison also missed the underscore-prefixed variants.
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(KeyMemberName = "_obj")]
            public partial class ValueObjectWithObjKey
            {
            }

            [ValueObject<int>(KeyMemberName = "_validationError")]
            public partial class ValueObjectWithValidationErrorKey
            {
            }

            [ValueObject<int>(KeyMemberName = "_provider")]
            public partial class ValueObjectWithProviderKey
            {
            }
         }
         """;

      AssertGeneratedCodeCompiles<ValueObjectSourceGenerator>(source);
   }

   [Fact]
   public void Should_escape_keyword_named_key_member_of_keyed_value_object() // finding 14
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(KeyMemberName = "class")]
            public partial class TestValueObject
            {
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

   [Fact]
   public void Should_not_collide_when_derived_field_renders_to_base_constructor_parameter() // finding: base-ctor argument collision
   {
      // The derived field '_value' renders to the identifier 'value' (leading underscore dropped), which is the
      // same identifier the base constructor parameter 'value' renders to. A raw-name comparison ("_value" vs
      // "value") missed the collision, so the generated constructor emitted '@value' twice (CS0100).
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public class BaseClass
            {
               protected BaseClass(int value)
               {
               }
            }

            [SmartEnum<string>]
            public partial class TestEnum : BaseClass
            {
               public static readonly TestEnum Item1 = default!;

               private readonly int _value;

               private TestEnum(int value)
                  : base(value)
               {
               }
            }
         }
         """;

      AssertGeneratedCodeCompiles<SmartEnumSourceGenerator>(source);
   }

   [Fact]
   public void Should_not_collide_when_complex_value_object_members_are_named_like_the_factory_method_locals()
   {
      // The generated factory and validation methods declare fixed 'obj' and 'validationError' parameters and
      // locals in the same scope as one parameter per member. Members rendering to these identifiers collided
      // (CS0100/CS0128/CS0136), so the fixed names are renamed to 'resultObj' and 'resultValidationError'.
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            public partial class TestValueObject
            {
               public string Obj { get; }
               public string ValidationError { get; }
            }
         }
         """;

      AssertGeneratedCodeCompiles<ValueObjectSourceGenerator>(source);
   }

   [Fact]
   public void Should_not_collide_when_complex_value_object_has_members_named_Obj_and_ResultObj()
   {
      // 'Obj' pushes the fixed parameter name to the alternative 'resultObj', which the member 'ResultObj' takes as
      // well. The alternative therefore needs a numeric suffix ('resultObj1'); without it the generated factory
      // methods declared 'obj' and 'resultObj' twice (CS0100/CS0128). The same applies to 'ValidationError' and
      // 'ResultValidationError'.
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            public partial class TestValueObject
            {
               public string Obj { get; }
               public string ResultObj { get; }
               public string ValidationError { get; }
               public string ResultValidationError { get; }
            }
         }
         """;

      AssertGeneratedCodeCompiles<ValueObjectSourceGenerator>(source);
   }

   [Fact]
   public void Should_not_collide_when_complex_value_object_member_is_named_FactoryArgumentsValidationError()
   {
      // A non-void 'ValidateFactoryArguments' makes the generator declare a local named
      // 'factoryArgumentsValidationError' in the same scope as the members' parameters, and a 'FactoryPostInit'
      // parameter with the same name. A member rendering to that identifier collided (CS0100/CS0128).
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            public partial class TestValueObject
            {
               public int FactoryArgumentsValidationError { get; }

               private static partial int ValidateFactoryArguments(
                  ref ValidationError validationError,
                  ref int factoryArgumentsValidationError)
               {
                  return 42;
               }
            }
         }
         """;

      AssertGeneratedCodeCompiles<ValueObjectSourceGenerator>(source);
   }

   [Fact]
   public void Should_not_collide_when_complex_value_object_members_are_named_like_the_MessagePack_locals()
   {
      // The generated 'Deserialize' declares the fixed locals '__count', '__resolver', '__validationError',
      // '__obj' and the loop variable '__i' in the same scope as one local per member, and its parameters
      // '__reader' and '__options' share that scope too. Before the double-underscore prefix these were plain
      // identifiers and collided with members named 'Count', 'Resolver', 'ValidationError', 'Obj', 'I', 'Reader'
      // and 'Options' (CS0128/CS0136/CS0841).
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            public partial class TestValueObject
            {
               public int Count { get; }
               public string Resolver { get; }
               public string ValidationError { get; }
               public string Obj { get; }
               public int I { get; }
               public string Reader { get; }
               public string Options { get; }
            }
         }
         """;

      AssertGeneratedCodeCompiles<ValueObjectSourceGenerator>(source,
                                                              typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                              typeof(MessagePackFormatterAttribute).Assembly,
                                                              typeof(MessagePackSerializerOptions).Assembly);
   }

   [Fact]
   public void Should_not_collide_when_complex_value_object_members_are_named_like_the_Json_locals()
   {
      // The generated 'Read' declares the fixed locals '__comparer', '__propName', '__validationError' and '__obj'
      // in the same scope as one local per member, and its parameters '__reader', '__typeToConvert' and '__options'
      // share that scope too. Before the double-underscore prefix these were plain identifiers and collided with
      // members named 'Comparer', 'PropName', 'ValidationError', 'Obj', 'Reader', 'TypeToConvert' and 'Options'
      // (CS0128/CS0136/CS0841).
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            public partial class TestValueObject
            {
               public string Comparer { get; }
               public string PropName { get; }
               public string ValidationError { get; }
               public string Obj { get; }
               public string Reader { get; }
               public string TypeToConvert { get; }
               public string Options { get; }
            }
         }
         """;

      AssertGeneratedCodeCompiles<ValueObjectSourceGenerator>(source,
                                                              typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                              typeof(JsonDocument).Assembly);
   }

   [Fact]
   public void Should_not_collide_when_complex_value_object_members_are_named_like_the_NewtonsoftJson_locals()
   {
      // The generated 'ReadJson' declares the fixed locals '__comparer', '__propName', '__validationError', '__obj'
      // and the deconstruction variables '__lineNumber' and '__linePosition' in scopes shared with one local per
      // member, and its parameters '__reader', '__objectType', '__existingValue' and '__serializer' share that
      // scope too. Before the double-underscore prefix these were plain identifiers and collided with members named
      // 'Comparer', 'PropName', 'ValidationError', 'Obj', 'LineNumber', 'LinePosition', 'Reader', 'ObjectType',
      // 'ExistingValue' and 'Serializer' (CS0128/CS0136/CS0841).
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            public partial class TestValueObject
            {
               public string Comparer { get; }
               public string PropName { get; }
               public string ValidationError { get; }
               public string Obj { get; }
               public int LineNumber { get; }
               public int LinePosition { get; }
               public string Reader { get; }
               public string ObjectType { get; }
               public string ExistingValue { get; }
               public string Serializer { get; }
            }
         }
         """;

      AssertGeneratedCodeCompiles<ValueObjectSourceGenerator>(source,
                                                              typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                              typeof(JsonToken).Assembly);
   }

   private static void AssertGeneratedCodeCompiles<T>(string source, params Assembly[] furtherAssemblies)
      where T : IIncrementalGenerator, new()
   {
      var syntaxTree = CSharpSyntaxTree.ParseText(source);

      // Reference the full framework closure from the trusted platform assemblies so that the generated code
      // (which uses e.g. System.Linq.Expressions) compiles, then add the Thinktecture runtime library. The
      // Thinktecture assemblies are excluded from the closure and added back explicitly, so that a test decides
      // which serializer integration - and therefore which additional code generator - takes part.
      var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
                       .Split(Path.PathSeparator)
                       .Where(p => !string.IsNullOrEmpty(p) && !p.Contains("Thinktecture"))
                       .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
                       .Append(MetadataReference.CreateFromFile(typeof(ISmartEnum<>).Assembly.Location))
                       .Concat(furtherAssemblies.Select(a => (MetadataReference)MetadataReference.CreateFromFile(a.Location)))
                       .ToList();

      var compilation = CSharpCompilation.Create("SourceGeneratorTests",
                                                 [syntaxTree],
                                                 references,
                                                 new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      // Precondition: the user-written source itself is valid. The partial delegate methods have no defining
      // declaration in this isolated compilation, so CS8795 is expected before the generator runs. For the same
      // reason a user-written 'ValidateFactoryArguments' implementation reports CS0759 until the generator emits
      // the matching defining declaration.
      compilation.GetDiagnostics()
                 .Where(d => d.Severity == DiagnosticSeverity.Error && d.Id != "CS8795" && d.Id != "CS0759")
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
