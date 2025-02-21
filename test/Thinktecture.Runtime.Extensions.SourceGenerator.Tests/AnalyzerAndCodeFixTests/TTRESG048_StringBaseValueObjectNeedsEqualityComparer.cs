using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG048_StringBaseValueObjectNeedsEqualityComparer
{
   private const string _DIAGNOSTIC_ID = "TTRESG048";

   [Fact]
   public async Task Should_trigger_on_string_based_value_object_without_comparer()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<string>]
         	public partial class {|#0:TestValueObject|}
         	{
            }
         }
         """;

      var expectedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<string>]
             [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
             public partial class TestValueObject
         	{
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0);
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ValueObjectAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_string_based_value_object_with_comparer()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<string>]
            [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
         	public partial class {|#0:TestValueObject|}
         	{
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
   }
}
