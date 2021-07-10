using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests
{
   // ReSharper disable InconsistentNaming
   public class TTRESG029_KeyComparerOfExtensibleEnumMustBeProtectedOrPublic
   {
      private const string _DIAGNOSTIC_ID = "TTRESG029";

      public class ValueObject_key_member_should_not_be_nullable
      {
         [Fact]
         public async Task Should_trigger_if_keycomparer_is_private()
         {
            var code = @"
using System;
using System.Collections.Generic;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(IsExtensible = true, KeyComparer = nameof(_equalityComparer))]
	public partial class TestEnum : IEnum<string>
	{
      private static readonly IEqualityComparer<string> {|#0:_equalityComparer|} = StringComparer.Ordinal;

      public static readonly TestEnum Item1 = default;
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_equalityComparer");
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
         }

         [Fact]
         public async Task Should_trigger_if_keycomparer_is_internal()
         {
            var code = @"
using System;
using System.Collections.Generic;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(IsExtensible = true, KeyComparer = nameof(_equalityComparer))]
	public partial class TestEnum : IEnum<string>
	{
      internal static readonly IEqualityComparer<string> {|#0:_equalityComparer|} = StringComparer.Ordinal;

      public static readonly TestEnum Item1 = default;
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_equalityComparer");
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
         }

         [Fact]
         public async Task Should_not_trigger_if_keycomparer_is_protected()
         {
            var code = @"
using System;
using System.Collections.Generic;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(IsExtensible = true, KeyComparer = nameof(_equalityComparer))]
	public partial class TestEnum : IEnum<string>
	{
      protected static readonly IEqualityComparer<string> {|#0:_equalityComparer|} = StringComparer.Ordinal;

      public static readonly TestEnum Item1 = default;
   }
}";
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
         }

         [Fact]
         public async Task Should_not_trigger_if_keycomparer_is_public()
         {
            var code = @"
using System;
using System.Collections.Generic;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(IsExtensible = true, KeyComparer = nameof(_equalityComparer))]
	public partial class TestEnum : IEnum<string>
	{
      public static readonly IEqualityComparer<string> {|#0:_equalityComparer|} = StringComparer.Ordinal;

      public static readonly TestEnum Item1 = default;
   }
}";
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
         }
      }
   }
}
