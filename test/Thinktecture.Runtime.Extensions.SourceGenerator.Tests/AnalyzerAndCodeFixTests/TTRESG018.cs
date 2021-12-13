using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG018_ExtensibleEnumMemberMustBePublicOrHaveMapping
{
   private const string _DIAGNOSTIC_ID = "TTRESG018";

   public class ValueObject_key_member_should_not_be_nullable
   {
      [Fact]
      public async Task Should_trigger_if_member_is_private_and_without_mapping()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(IsExtensible = true)]
	public partial class TestEnum : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private readonly string {|#0:_field|};
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_field");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_if_member_is_protected_and_without_mapping()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(IsExtensible = true)]
	public partial class TestEnum : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      protected readonly string {|#0:_field|};
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_field");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_if_member_is_internal_and_without_mapping()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(IsExtensible = true)]
	public partial class TestEnum : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      internal readonly string {|#0:_field|};
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_field");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_member_is_public()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(IsExtensible = true)]
	public partial class TestEnum : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      public readonly string _field;
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_if_member_has_mapping_to_public_member()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [EnumGeneration(IsExtensible = true)]
	public partial class TestEnum : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      [EnumGenerationMember(MapsToMember = nameof(Field))]
      private readonly string _field;

      public string Field => _field;
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }
}