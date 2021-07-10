using System.Threading.Tasks;
using Xunit;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests
{
   // ReSharper disable InconsistentNaming
   public class TTRESG016
   {
      private const string _DIAGNOSTIC_ID = "TTRESG016";

      public class Enum_cannot_be_nested_class
      {
         [Fact]
         public async Task Should_trigger_if_enum_is_nested_class()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public class SomeClass
	{
      public partial class {|#0:TestEnum|} : IValidatableEnum<string>
	   {
         public static readonly TestEnum Item1 = default;
      }
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
         }
      }

      public class ValueObject_cannot_be_nested_class
      {
         [Fact]
         public async Task Should_trigger_if_valueobject_is_nested_class()
         {
            var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public class SomeClass
	{
      [ValueObject]
      public partial class {|#0:TestValueObject|}
	   {
      }
   }
}";

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly }, expected);
         }
      }
   }
}
