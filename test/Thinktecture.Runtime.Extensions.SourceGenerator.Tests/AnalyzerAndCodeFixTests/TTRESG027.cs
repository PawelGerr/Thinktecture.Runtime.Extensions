using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests
{
   // ReSharper disable InconsistentNaming
   public class TTRESG027_ExtensibleEnumCannotBeAbstract
   {
      private const string _DIAGNOSTIC_ID = "TTRESG027";

      public class ValueObject_key_member_should_not_be_nullable
      {
         [Fact]
         public async Task Should_trigger_if_extensible_enum_is_abstract()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [{|#0:EnumGeneration(IsExtensible = true)|}]
	public abstract partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private static TestEnum CreateInvalidItem(string name)
      {
         return default;
      }
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
         }

         [Fact]
         public async Task Should_not_trigger_if_abstract_enum_is_not_extensible()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [{|#0:EnumGeneration(IsExtensible = false)|}]
	public abstract partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private static TestEnum CreateInvalidItem(string name)
      {
         return default;
      }
   }
}";
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
         }
      }
   }
}
