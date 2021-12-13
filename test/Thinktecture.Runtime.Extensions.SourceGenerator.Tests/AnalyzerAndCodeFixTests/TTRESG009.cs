using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG009_Constructor_must_be_private
{
   private const string _DIAGNOSTIC_ID = "TTRESG009";

   [Fact]
   public async Task Should_trigger_if_constructor_is_protected()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      protected {|#0:TestEnum|}()
      {
      }
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_if_constructor_is_private_protected()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private protected {|#0:TestEnum|}()
      {
      }
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_constructor_is_private()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private {|#0:TestEnum|}()
      {
      }
   }
}";

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
   }
}