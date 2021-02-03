using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests
{
   // ReSharper disable InconsistentNaming
   public class TTRESG017
   {
      private const string _DIAGNOSTIC_ID = "TTRESG017";

      public class ValueType_key_member_should_not_be_nullable
      {
         [Fact]
         public async Task Should_trigger_if_key_member_is_nullable_reference_type()
         {
            var code = @"
#nullable enable

using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public partial class TestValueType
	{
      public readonly string? {|#0:Field|};
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field");
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueTypeAttribute).Assembly }, expected);
         }

         [Fact]
         public async Task Should_not_trigger_if_key_member_is_non_nullable_reference_type()
         {
            var code = @"
#nullable enable

using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public partial class TestValueType
	{
      public readonly string {|#0:Field|};

      public TestValueType() { Field = String.Empty; }
   }
}";

            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueTypeAttribute).Assembly });
         }

         [Fact]
         public async Task Should_not_trigger_if_nullability_is_not_active()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public partial class TestValueType
	{
      public readonly string {|#0:Field|};
   }
}";

            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueTypeAttribute).Assembly });
         }

         [Fact]
         public async Task Should_trigger_if_key_member_is_nullable_struct()
         {
            var code = @"
#nullable enable

using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public partial class TestValueType
	{
      public readonly int? {|#0:Field|};
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field");
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueTypeAttribute).Assembly }, expected);
         }

         [Fact]
         public async Task Should_trigger_if_key_member_is_nullable_struct_even_if_nullability_is_not_active()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public partial class TestValueType
	{
      public readonly int? {|#0:Field|};
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field");
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueTypeAttribute).Assembly }, expected);
         }

         [Fact]
         public async Task Should_not_trigger_if_key_member_is_non_nullable_struct()
         {
            var code = @"
#nullable enable

using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueType]
	public partial class TestValueType
	{
      public readonly int {|#0:Field|};
   }
}";

            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueTypeAttribute).Assembly });
         }
      }
   }
}
