using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG009_EnumConstructorsMustBePrivate
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
	public sealed partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      protected {|#0:TestEnum|}()
      {
      }
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
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
	public sealed partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private protected {|#0:TestEnum|}()
      {
      }
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
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
	public sealed partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private {|#0:TestEnum|}()
      {
      }
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
   }
}
