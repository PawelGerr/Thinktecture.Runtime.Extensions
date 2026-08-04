using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG080_AllowDefaultStructsCannotBeTrueIfTypeImplementsIDisallowDefaultValue
{
   private const string _DIAGNOSTIC_ID = "TTRESG080";

   [Fact]
   public async Task Should_trigger_on_keyed_struct_value_object_with_AllowDefaultStructs_and_manual_interface()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>(AllowDefaultStructs = true)]
            public partial struct TestValueObject : {|#0:IDisallowDefaultValue|};
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_complex_struct_value_object_with_AllowDefaultStructs_and_manual_interface()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ComplexValueObject(AllowDefaultStructs = true)]
            public partial struct TestValueObject : {|#0:IDisallowDefaultValue|}
            {
               public readonly int Value;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_when_AllowDefaultStructs_is_absent()
   {
      // Without AllowDefaultStructs the generator already emits the interface, so a manual implementation is
      // redundant but not a conflict. TTRESG080 must stay silent.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>]
            public partial struct TestValueObject : IDisallowDefaultValue;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_AllowDefaultStructs_is_false()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>(AllowDefaultStructs = false)]
            public partial struct TestValueObject : IDisallowDefaultValue;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_AllowDefaultStructs_is_true_without_manual_interface()
   {
      // AllowDefaultStructs = true suppresses the generator-emitted interface, so nothing implements it.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>(AllowDefaultStructs = true)]
            public partial struct TestValueObject;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
   }
}
