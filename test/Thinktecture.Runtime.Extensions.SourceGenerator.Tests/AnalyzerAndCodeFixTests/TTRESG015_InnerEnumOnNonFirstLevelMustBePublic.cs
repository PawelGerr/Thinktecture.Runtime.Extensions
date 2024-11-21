using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG015_InnerEnumOnNonFirstLevelMustBePublic
{
   private const string _DIAGNOSTIC_ID = "TTRESG015";

   [Fact]
   public async Task Should_trigger_if_2st_level_type_is_public()
   {
      var code = """

                 using System;
                 using Thinktecture;

                 namespace TestNamespace
                 {
                    [SmartEnum<string>(IsValidatable = true)]
                 	public partial class TestEnum
                 	{
                       public static readonly TestEnum Item1 = default;

                       private sealed class InnerTestEnum : TestEnum
                 	   {
                          private sealed class {|#0:MostInnerTestEnum|} : TestEnum
                 	      {
                          }
                       }
                    }
                 }
                 """;

      var expectedCode = """

                         using System;
                         using Thinktecture;

                         namespace TestNamespace
                         {
                            [SmartEnum<string>(IsValidatable = true)]
                         	public partial class TestEnum
                         	{
                               public static readonly TestEnum Item1 = default;

                               private sealed class InnerTestEnum : TestEnum
                         	   {
                                  public sealed class {|#0:MostInnerTestEnum|} : TestEnum
                         	      {
                                  }
                               }
                            }
                         }
                         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("MostInnerTestEnum");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_2nd_level_type_is_public()
   {
      var code = """

                 using System;
                 using Thinktecture;

                 namespace TestNamespace
                 {
                    [SmartEnum<string>(IsValidatable = true)]
                 	public partial class TestEnum
                 	{
                       public static readonly TestEnum Item1 = default;

                       private sealed class InnerTestEnum : TestEnum
                 	   {
                          public sealed class {|#0:MostInnerTestEnum|} : TestEnum
                 	      {
                          }
                       }
                    }
                 }
                 """;

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
   }
}
