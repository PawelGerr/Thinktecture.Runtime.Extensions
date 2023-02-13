using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG031_ComparerApplicableOnKeyMemberOnly
{
   private const string _DIAGNOSTIC_ID = "TTRESG031";

   [Fact]
   public async Task Should_trigger_if_placed_on_non_key_member()
   {
      var code = @"
#nullable enable

using System;
using System.Collections.Generic;
using Thinktecture;
using Thinktecture.Runtime.Tests;

namespace TestNamespace
{
   [ValueObject]
	public sealed partial class TestValueObject
	{
      [{|#0:ValueObjectMemberCompare<DefaultComparerAccessor<int>, int>|}]
      public readonly int Field;

      public readonly int Field2;
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("global::Thinktecture.Runtime.Tests.DefaultComparerAccessor<int>");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly, typeof(DefaultComparerAccessor<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_placed_on_key_member()
   {
      var code = @"
#nullable enable

using System;
using System.Collections.Generic;
using Thinktecture;
using Thinktecture.Runtime.Tests;

namespace TestNamespace
{
   [ValueObject]
	public sealed partial class TestValueObject
	{
      [{|#0:ValueObjectMemberCompare<DefaultComparerAccessor<int>, int>|}]
      public readonly int Field;
   }
}";

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly, typeof(DefaultComparerAccessor<>).Assembly });
   }
}
