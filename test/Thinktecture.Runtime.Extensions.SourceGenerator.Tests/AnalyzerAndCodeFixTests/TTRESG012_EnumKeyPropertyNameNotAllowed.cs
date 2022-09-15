using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG012_EnumKeyPropertyNameNotAllowed
{
   private const string _DIAGNOSTIC_ID = "TTRESG012";

   [Fact]
   public async Task Should_trigger_if_name_is_item()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration{|#0:(KeyPropertyName = ""Item"")|}]
	public sealed partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Item");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_name_is_not_item()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration{|#0:(KeyPropertyName = ""Foo"")|}]
	public sealed partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
   }
}
