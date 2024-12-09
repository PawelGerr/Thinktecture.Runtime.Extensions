using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.CodeFixes;
using Thinktecture.CodeAnalysis.Diagnostics;
using Thinktecture.Runtime.Tests.Verifiers;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG036_EnumKeyShouldNotBeNullable
{
   private const string _DIAGNOSTIC_ID = "TTRESG036";

   [Fact]
   public async Task Should_trigger_on_nullable_int_in_IValidatableEnum()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
         #pragma warning disable CS8632
            [SmartEnum<int?>(IsValidatable = true)]
         	public partial class {|#0:TestEnum|}
         #pragma warning restore CS8632
         	{
               public static readonly TestEnum Item1 = default;
            }
         }
         """;

      var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0);
      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_on_nullable_int_in_IEnum()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
         #pragma warning disable CS8632
            [SmartEnum<int?>]
         	public partial class {|#0:TestEnum|}
         #pragma warning restore CS8632
         	{
               public static readonly TestEnum Item1 = default;
            }
         }
         """;

      var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0);
      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_string_in_IValidatableEnum()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>(IsValidatable = true)]
         	public partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;
            }
         }
         """;

      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
   }

   [Fact]
   public async Task Should_not_trigger_on_string_in_IEnum()
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

      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
   }

   [Fact]
   public async Task Should_not_trigger_on_int_in_IValidatableEnum()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<int?>(IsValidatable = true)]
         	public partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;
            }
         }
         """;

      var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0);
      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_int_in_IEnum()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<int>]
         	public partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;
            }
         }
         """;

      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
   }
}
