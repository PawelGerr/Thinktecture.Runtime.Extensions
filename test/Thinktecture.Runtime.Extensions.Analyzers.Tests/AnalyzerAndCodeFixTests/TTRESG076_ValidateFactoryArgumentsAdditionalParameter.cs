using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG076_ValidateFactoryArgumentsAdditionalParameter
{
   private const string _DIAGNOSTIC_ID = "TTRESG076";

   [Fact]
   public async Task Should_diagnose_keyed_additional_parameter_with_default_value()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>]
            public partial class TestValueObject
            {
               private static void ValidateFactoryArguments(ref ValidationError? validationError, ref int value, string? {|#0:extra|} = null)
               {
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("extra", "global::TestNamespace.TestValueObject");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_diagnose_keyed_additional_parameter_that_is_by_ref()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>]
            public partial class TestValueObject
            {
               private static void ValidateFactoryArguments(ref ValidationError? validationError, ref int value, ref string? {|#0:extra|})
               {
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("extra", "global::TestNamespace.TestValueObject");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_diagnose_complex_additional_parameter_with_default_value()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ComplexValueObject]
            public partial class TestValueObject
            {
               public int Property1 { get; }
               public int Property2 { get; }

               private static void ValidateFactoryArguments(ref ValidationError? validationError, ref int property1, ref int property2, string? {|#0:extra|} = null)
               {
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("extra", "global::TestNamespace.TestValueObject");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_diagnose_keyed_additional_parameter_without_default_or_by_ref()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>]
            public partial class TestValueObject
            {
               private static void ValidateFactoryArguments(ref ValidationError? validationError, ref int value, string? extra)
               {
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_diagnose_when_no_additional_parameters()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>]
            public partial class TestValueObject
            {
               private static void ValidateFactoryArguments(ref ValidationError? validationError, ref int value)
               {
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
   }
}
