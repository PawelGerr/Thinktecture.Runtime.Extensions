using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG045_CustomKeyMemberImplementationTypeMismatch
{
   private const string _DIAGNOSTIC_ID = "TTRESG045";

   public class KeyedValueObject_CustomKeyMemberImplementationTypeMismatch
   {
      [Fact]
      public async Task Should_trigger_if_key_member_type_differs_from_type_specified_by_attribute()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>(SkipKeyMember = true)]
               [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
               public partial class ValueObject
            	{
                  private readonly int {|#0:_value|};
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_value", "int", "string");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_if_key_member_type_is_nullable_reference_type()
      {
         var code = """

            #nullable enable

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>(SkipKeyMember = true)]
               [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
               public partial class ValueObject
            	{
                  private readonly string? {|#0:_value|};
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_value", "string?", "string");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_if_key_member_type_is_nullable_struct()
      {
         var code = """

            #nullable enable

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>(SkipKeyMember = true)]
               public partial class ValueObject
            	{
                  private readonly int? {|#0:_value|};
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_value", "int?", "int");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_key_member_type_is_correct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>(SkipKeyMember = true)]
               [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
               public partial class {|#0:ValueObject|}
            	{
                  private readonly string _value;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly]);
      }
   }
}
