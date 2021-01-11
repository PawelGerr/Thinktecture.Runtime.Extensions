using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.AnalyzerAndCodeFixTests
{
   // ReSharper disable InconsistentNaming
   public class TTRESG006
   {
      private const string _DIAGNOSTIC_ID = "TTRESG006";

      public class Enum_must_be_partial
      {
         [Fact]
         public async Task Should_trigger_on_non_partial_class()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public class {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

            var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
         }

         [Fact]
         public async Task Should_trigger_on_non_partial_struct()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public readonly struct {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

            var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public readonly partial struct {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
         }

         [Fact]
         public async Task Should_not_trigger_on_partial_class()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
         }

         [Fact]
         public async Task Should_not_trigger_on_partial_struct()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public readonly partial struct {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
         }
      }

      public class ValueType_must_be_partial
      {
         [Fact]
         public async Task Should_trigger_on_non_partial_class()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public class {|#0:TestValueType|}
	{
   }
}";

            var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public partial class {|#0:TestValueType|}
	{
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueType");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ValueTypeAttribute).Assembly }, expected);
         }

         [Fact]
         public async Task Should_trigger_on_non_partial_struct()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public readonly struct {|#0:TestValueType|}
	{
   }
}";

            var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public readonly partial struct {|#0:TestValueType|}
	{
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueType");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ValueTypeAttribute).Assembly }, expected);
         }

         [Fact]
         public async Task Should_not_trigger_on_partial_class()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public partial class {|#0:TestValueType|}
	{
   }
}";

            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueTypeAttribute).Assembly });
         }

         [Fact]
         public async Task Should_not_trigger_on_partial_struct()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public readonly partial struct {|#0:TestValueType|}
	{
   }
}";

            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueTypeAttribute).Assembly });
         }
      }
   }
}