using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Verifiers.CodeFixVerifier<Thinktecture.EnumDiagnosticAnalyzer, Thinktecture.EnumCodeFixProvider>;

namespace Thinktecture.EnumAnalyzerAndCodeFixTests
{
   // ReSharper disable once InconsistentNaming
   public class TTRESG016_Enum_cannot_be_nested_class
   {
      private const string _DIAGNOSTIC_ID = "TTRESG016";

      [Fact]
      public async Task Should_trigger_if_enum_is_nested_class()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public class SomeClass
	{
      public partial class {|#0:TestEnum|} : IValidatableEnum<string>
	   {
         public static readonly TestEnum Item1 = default;
      }
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }
   }
}
