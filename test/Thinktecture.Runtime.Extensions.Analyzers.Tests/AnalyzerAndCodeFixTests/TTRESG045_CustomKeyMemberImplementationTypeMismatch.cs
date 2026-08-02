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
               [ValueObject<int>(SkipKeyMember = true)]
               public partial class ValueObject
            	{
                  private readonly string {|#0:_value|};
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_value", "string", "int");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
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
               [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
               [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
               public partial class ValueObject
            	{
                  private readonly string? {|#0:_value|};
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_value", "string?", "string");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
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
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_key_member_has_the_type_of_the_resolved_TypeParamRef_marker()
      {
         // Without the resolution of the marker the key type would be the reference type 'TypeParamRef1',
         // which does not match 'T' and would produce a false positive.
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<TypeParamRef1>(SkipKeyMember = true)]
               public partial class ValueObject<T>
                  where T : notnull
            	{
                  private readonly T _value;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
      }

      [Fact]
      public async Task Should_report_the_resolved_type_parameter_when_key_type_is_a_TypeParamRef_marker()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<TypeParamRef1>(SkipKeyMember = true)]
               public partial class ValueObject<T>
                  where T : notnull
            	{
                  private readonly string {|#0:_value|};
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_value", "string", "T");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_key_member_type_is_correct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>(SkipKeyMember = true)]
               public partial class {|#0:ValueObject|}
            	{
                  private readonly int _value;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
      }
   }
}
