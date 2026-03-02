using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG101_StaticPropertiesAreNotConsideredItems
{
   private const string _DIAGNOSTIC_ID = "TTRESG101";

   [Fact]
   public async Task Should_trigger_if_potential_item_is_a_property()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default;

               public static TestEnum {|#0:Item2|} => default;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Item2");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
   }
}
