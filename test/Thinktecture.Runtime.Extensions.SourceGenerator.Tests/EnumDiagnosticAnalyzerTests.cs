using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Verifiers.CodeFixVerifier<Thinktecture.EnumDiagnosticAnalyzer, Thinktecture.EnumCodeFixProvider>;

namespace Thinktecture
{
   public class EnumDiagnosticAnalyzerTests
   {
      [Fact]
      public async Task Should_trigger_on_non_partial_class()
      {
         var code = @"
using System;
using Thinktecture;

namespace Thinktecture.EnumLikeClass
{
	public class {|#0:ProductCategory|} : IValidatableEnum<string>
	{
      public static readonly ProductCategory Item1 = null;
   }
}";

         var expectedCode = @"
using System;
using Thinktecture;

namespace Thinktecture.EnumLikeClass
{
	public partial class ProductCategory : IValidatableEnum<string>
	{
      public static readonly ProductCategory Item1 = null;
   }
}";

         var expected = Verifier.Diagnostic("TTRESG020").WithLocation(0).WithArguments("ProductCategory");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
      }
   }
}
