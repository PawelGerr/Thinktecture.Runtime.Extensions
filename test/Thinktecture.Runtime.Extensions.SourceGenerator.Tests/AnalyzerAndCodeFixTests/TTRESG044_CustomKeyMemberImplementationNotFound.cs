using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG044_CustomKeyMemberImplementationNotFound
{
   private const string _DIAGNOSTIC_ID = "TTRESG044";

   public class KeyedValueObject_CustomKeyMemberImplementationNotFound
   {
      [Fact]
      public async Task Should_trigger_if_key_member_implementation_missing()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<string>(SkipKeyMember = true)]
                       public sealed partial class {|#0:ValueObject|}
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_value");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_key_member_implementation_found()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<string>(SkipKeyMember = true)]
                       public sealed partial class {|#0:ValueObject|}
                    	{
                          private readonly string _value;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }
}
