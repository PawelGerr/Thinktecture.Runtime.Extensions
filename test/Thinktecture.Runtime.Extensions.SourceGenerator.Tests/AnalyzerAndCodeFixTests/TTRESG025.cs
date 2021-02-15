using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests
{
   // ReSharper disable InconsistentNaming
   public class TTRESG025_BaseEnumMustBeExtensible
   {
      private const string _DIAGNOSTIC_ID = "TTRESG025";

      public class ValueType_key_member_should_not_be_nullable
      {
         [Fact]
         public async Task Should_trigger_if_base_enum_is_not_extensible()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class TestEnum : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }

   [EnumGeneration]
	public partial class {|#0:ExtendedTestEnum|} : TestEnum
	{
      public static readonly ExtendedTestEnum Item2 = default;
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
         }

         [Fact]
         public async Task Should_not_trigger_if_base_enum_is_extensible()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(IsExtensible = true)]
	public partial class TestEnum : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }

   [EnumGeneration]
	public partial class {|#0:ExtendedTestEnum|} : TestEnum
	{
      public static readonly ExtendedTestEnum Item2 = default;
   }
}";
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
         }
      }
   }
}
