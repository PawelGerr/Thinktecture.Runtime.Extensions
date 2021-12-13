using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG020_MultipleMembersWithSameName
{
   private const string _DIAGNOSTIC_ID = "TTRESG020";

   public class ValueObject_key_member_should_not_be_nullable
   {
      [Fact]
      public async Task Should_trigger_if_mapped_member_is_not_found()
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

      [{|#0:EnumGenerationMember(MapsToMember = nameof(Action))|}]
      private readonly Action _field;

      public void Action() { }
      public void Action(int value) { }
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Action");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_mapped_member_is_unique()
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