using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG051_MethodWithUseDelegateFromConstructorMustNotHaveGenerics
{
   private const string _DIAGNOSTIC_ID = "TTRESG051";

   [Fact]
   public async Task Should_trigger_on_method_with_generics()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<int>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default;

               [UseDelegateFromConstructor]
               public partial string {|#0:Process|}<T>(T input);
            }

            public partial class TestEnum
            {
               public partial string Process<T>(T input) => "";
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Process");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UseDelegateFromConstructorAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_method_without_generics()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<int>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default;

               [UseDelegateFromConstructor]
               public partial string Process(int input);
            }

            public partial class TestEnum
            {
               public partial string Process(int input) => "";
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UseDelegateFromConstructorAttribute).Assembly]);
   }
}
