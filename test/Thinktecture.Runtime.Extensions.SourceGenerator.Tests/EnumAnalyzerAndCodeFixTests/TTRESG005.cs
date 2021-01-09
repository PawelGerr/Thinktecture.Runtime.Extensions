using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Verifiers.CodeFixVerifier<Thinktecture.EnumDiagnosticAnalyzer, Thinktecture.EnumCodeFixProvider>;

namespace Thinktecture.EnumAnalyzerAndCodeFixTests
{
   // ReSharper disable once InconsistentNaming
   public class TTRESG005_Multiple_interfaces_with_different_key_types
   {
      private const string _DIAGNOSTIC_ID = "TTRESG005";

      [Fact]
      public async Task Should_trigger_on_2_IValidatableEnum_with_different_keys()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IValidatableEnum<string>, IValidatableEnum<int>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_2_IEnum_with_different_keys()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IEnum<string>, IEnum<int>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_2_interfaces_with_different_keys()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IEnum<string>, IValidatableEnum<int>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_2_interfaces_with_same_key()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IEnum<int>, IValidatableEnum<int>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }
}