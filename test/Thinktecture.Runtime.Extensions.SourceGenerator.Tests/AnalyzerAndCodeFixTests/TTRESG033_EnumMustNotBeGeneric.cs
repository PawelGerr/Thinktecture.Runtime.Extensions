using Thinktecture.CodeAnalysis.CodeFixes;
using Thinktecture.CodeAnalysis.Diagnostics;
using Thinktecture.Runtime.Tests.Verifiers;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG033_EnumMustNotBeGeneric
{
   private const string _DIAGNOSTIC_ID = "TTRESG033";

   [Fact]
   public async Task Should_trigger_on_generic_class()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|}<T> : IValidatableEnum<string>
	{
      public static readonly TestEnum<T> Item1 = default;
   }
}";

      var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum<T>");
      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_on_generic_struct()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public readonly partial struct {|#0:TestEnum|}<T> : IValidatableEnum<string>
	{
      public static readonly TestEnum<T> Item1 = default;
   }
}";

      var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum<T>");
      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }
}
