using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG010_NonValidatableEnumsMustBeClass
{
   private const string _DIAGNOSTIC_ID = "TTRESG010";

   [Fact]
   public async Task Should_trigger_if_IEnum_is_struct()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>]
	public readonly partial struct {|#0:TestEnum|}
	{
      public static readonly TestEnum Item1 = default;
   }

   // simulate source gen
	partial struct TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_IEnum_is_class()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>]
	public sealed partial class {|#0:TestEnum|}
	{
      public static readonly TestEnum Item1 = default;
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
