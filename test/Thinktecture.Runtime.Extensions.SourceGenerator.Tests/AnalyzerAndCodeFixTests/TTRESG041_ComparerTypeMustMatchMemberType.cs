using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.CodeFixes;
using Thinktecture.CodeAnalysis.Diagnostics;
using Thinktecture.Runtime.Tests.Verifiers;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG041_ComparerTypeMustMatchMemberType
{
   private const string _DIAGNOSTIC_ID = "TTRESG041";

   [Fact]
   public async Task Should_trigger_if_placed_on_member_of_different_type()
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
      [{|#0:ValueObjectMemberCompare<DefaultComparerAccessor<string>, string>|}]
      public readonly int Field;
   }
}";

      var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("global::Thinktecture.Runtime.Tests.DefaultComparerAccessor<string>");
      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly, typeof(DefaultComparerAccessor<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_placed_on_member_of_same_type()
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

      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly, typeof(DefaultComparerAccessor<>).Assembly });
   }
}
