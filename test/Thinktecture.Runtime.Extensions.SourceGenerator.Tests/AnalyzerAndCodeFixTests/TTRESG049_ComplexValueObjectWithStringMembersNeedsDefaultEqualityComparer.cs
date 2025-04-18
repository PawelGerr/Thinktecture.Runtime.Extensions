using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG049_ComplexValueObjectWithStringMembersNeedsDefaultEqualityComparer
{
   private const string _DIAGNOSTIC_ID = "TTRESG049";

   [Fact]
   public async Task Should_trigger_on_string_members_without_comparer()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ComplexValueObject]
         	public partial class {|#0:TestValueObject|}
         	{
               public string Property { get; }
            }
         }
         """;

      var expectedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
         	public partial class TestValueObject
         	{
               public string Property { get; }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0);
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_when_DefaultStringComparison_present()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
            public partial class {|#0:TestValueObject|}
         	{
               public string Property { get; }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_string_members_with_comparer()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ComplexValueObject]
         	public partial class {|#0:TestValueObject|}
         	{
         	   [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public string Property { get; }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
   }
}
