using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public record TTRESG057_AllowDefaultStructsCannotBeTrueIfValueObjectIsStructButKeyTypeIsClass
{
   private const string _DIAGNOSTIC_ID = "TTRESG057";

   [Fact]
   public async Task Should_not_trigger_on_class_with_struct_key_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>(AllowDefaultStructs = true)]
            public partial class {|#0:TestValueObject|};
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_class_with_class_key_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public record TestClass;

            [ValueObject<TestClass>(AllowDefaultStructs = true)]
            public partial class {|#0:TestValueObject|};
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_struct_with_struct_key_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>(AllowDefaultStructs = true)]
            public partial struct {|#0:TestValueObject|};
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
   }

   [Fact]
   public async Task Should_trigger_on_struct_with_reference_key_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
           public record TestClass;

            [{|#0:ValueObject<TestClass>(AllowDefaultStructs = true)|}]
            public partial struct TestValueObject;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject", "TestClass");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_struct_with_TypeParamRef_key_that_resolves_to_a_struct_type_parameter()
   {
      // The key type is the unresolved marker 'TypeParamRef1' (a reference type). The analyzer must resolve it
      // to the actual type parameter 'T' (a value type because of 'where T : struct'), like the source generator
      // does, before checking 'AllowDefaultStructs'. Otherwise the marker's reference-type nature produces a
      // false-positive TTRESG057.
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<TypeParamRef1>(AllowDefaultStructs = true)]
            public readonly partial struct {|#0:TestValueObject|}<T>
               where T : struct;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
   }
}
