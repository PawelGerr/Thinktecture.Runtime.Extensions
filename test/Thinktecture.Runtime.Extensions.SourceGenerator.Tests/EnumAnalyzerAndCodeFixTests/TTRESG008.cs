using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Verifiers.CodeFixVerifier<Thinktecture.EnumDiagnosticAnalyzer, Thinktecture.EnumCodeFixProvider>;

namespace Thinktecture.EnumAnalyzerAndCodeFixTests
{
   // ReSharper disable once InconsistentNaming
   public class TTRESG008_Needs_CreateInvalidItem
   {
      private const string _DIAGNOSTIC_ID = "TTRESG008";

      [Fact]
      public async Task Should_trigger_if_abstract_class_has_no_CreateInvalidItem()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public abstract partial class {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public abstract partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

        private static TestEnum CreateInvalidItem(string key)
        {
            throw new System.NotImplementedException();
        }
    }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "string");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_abstract_class_has_a_CreateInvalidItem()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public abstract partial class {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private static TestEnum CreateInvalidItem(string key)
      {
         throw new System.NotImplementedException();
      }
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }
}
