using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG007_InvalidSignatureOfCreateInvalidItem
{
   private const string _DIAGNOSTIC_ID = "TTRESG007";

   [Fact]
   public async Task Should_trigger_if_not_private()
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
         
               public static TestEnum {|#0:CreateInvalidItem|}(string key)
               {
                  return null;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "string");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_if_not_static()
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
         
               private TestEnum {|#0:CreateInvalidItem|}(string key)
               {
                  return null;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "string");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_if_return_type_incorrect()
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
         
               private static string {|#0:CreateInvalidItem|}(string key)
               {
                  return null;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "string");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_if_argument_type_incorrect()
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
         
               private static TestEnum {|#0:CreateInvalidItem|}(int key)
               {
                  return null;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "string");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_if_argument_count_incorrect()
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
         
               private static TestEnum {|#0:CreateInvalidItem|}(string key, string other)
               {
                  return null;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "string");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_valid_implementation()
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
         
               private static TestEnum {|#0:CreateInvalidItem|}(string key)
               {
                  return null;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
   }
}
