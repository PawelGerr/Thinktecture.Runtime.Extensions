using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG026_ExtensibleEnumCannotBeStruct
{
   private const string _DIAGNOSTIC_ID = "TTRESG026";

   public class ValueObject_key_member_should_not_be_nullable
   {
      [Fact]
      public async Task Should_trigger_if_extensible_enum_is_struct()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [{|#0:EnumGeneration(IsExtensible = true)|}]
	public readonly partial struct TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_struct_is_not_extensible()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [{|#0:EnumGeneration(IsExtensible = false)|}]
	public readonly partial struct TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }
}