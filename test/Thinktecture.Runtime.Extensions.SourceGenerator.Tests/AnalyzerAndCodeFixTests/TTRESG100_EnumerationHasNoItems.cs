using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG100_EnumerationHasNoItems
{
   private const string _DIAGNOSTIC_ID = "TTRESG100";

   [Fact]
   public async Task Should_trigger_if_enumeration_has_no_items()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class {|#0:TestEnum|}
         	{
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_enumeration_has_items()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }
}
