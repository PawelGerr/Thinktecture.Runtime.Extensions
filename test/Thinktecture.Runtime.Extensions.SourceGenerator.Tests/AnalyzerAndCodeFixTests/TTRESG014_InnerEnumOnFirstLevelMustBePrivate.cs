using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG014_InnerEnumOnFirstLevelMustBePrivate
{
   private const string _DIAGNOSTIC_ID = "TTRESG014";

   [Fact]
   public async Task Should_trigger_if_1st_level_type_is_public()
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

               public sealed class {|#0:InnerTestEnum|} : TestEnum
         	   {
               }
            }
         }
         """;

      var expectedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default;

               private sealed class {|#0:InnerTestEnum|} : TestEnum
         	   {
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerTestEnum");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ISmartEnum<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_1st_level_type_is_private()
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

               private sealed class {|#0:InnerTestEnum|} : TestEnum
         	   {
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_if_2nd_level_type_is_public()
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

               private sealed class InnerTestEnum : TestEnum
         	   {
                  public sealed class {|#0:MostInnerTestEnum|} : TestEnum
         	      {
                  }
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }
}
