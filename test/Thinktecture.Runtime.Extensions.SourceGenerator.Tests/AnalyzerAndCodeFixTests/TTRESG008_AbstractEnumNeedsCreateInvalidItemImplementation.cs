using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG008_AbstractEnumNeedsCreateInvalidItemImplementation
{
   private const string _DIAGNOSTIC_ID = "TTRESG008";

   [Fact]
   public async Task Should_trigger_if_abstract_class_has_no_CreateInvalidItem()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>(IsValidatable = true)]
         	public abstract partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;
            }
         }
         """;

      var expectedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>(IsValidatable = true)]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default;
         
                 private static TestEnum CreateInvalidItem(string key)
                 {
                     throw new global::System.NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "string");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(IEnum<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_abstract_class_has_a_CreateInvalidItem()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>(IsValidatable = true)]
         	public abstract partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;
         
               private static TestEnum CreateInvalidItem(string key)
               {
                  throw new System.NotImplementedException();
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly]);
   }
}
