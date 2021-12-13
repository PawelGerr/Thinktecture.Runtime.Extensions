using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG030_KeyComparerMustBeStaticFieldOrProperty
{
   private const string _DIAGNOSTIC_ID = "TTRESG030";

   public class ValueObject_key_member_should_not_be_nullable
   {
      [Fact]
      public async Task Should_trigger_if_keycomparer_is_not_static()
      {
         var code = @"
using System;
using System.Collections.Generic;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(KeyComparer = nameof(_equalityComparer))]
	public partial class TestEnum : IEnum<string>
	{
      private readonly IEqualityComparer<string> {|#0:_equalityComparer|} = StringComparer.Ordinal;

      public static readonly TestEnum Item1 = default;
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_equalityComparer");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_keycomparer_is_static()
      {
         var code = @"
using System;
using System.Collections.Generic;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(KeyComparer = nameof(_equalityComparer))]
	public partial class TestEnum : IEnum<string>
	{
      private static readonly IEqualityComparer<string> {|#0:_equalityComparer|} = StringComparer.Ordinal;

      public static readonly TestEnum Item1 = default;
   }
}";
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }
}