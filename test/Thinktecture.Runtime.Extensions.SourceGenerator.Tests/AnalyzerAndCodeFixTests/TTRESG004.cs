using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG004
{
   private const string _DIAGNOSTIC_ID = "TTRESG004";

   public class Enum_must_be_class_or_struct
   {
      [Fact]
      public async Task Should_trigger_on_interface()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial interface {|#0:TestEnum|} : IValidatableEnum<string>
	{
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_record()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial record {|#0:TestEnum|} : IValidatableEnum<string>
	{
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_class()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_struct()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public readonly partial struct TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }

   public class ValueObject_must_be_class_or_struct
   {
      [Fact]
      public async Task Should_trigger_on_record()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	[ValueObject]
   public partial record {|#0:TestValueObject|}
	{
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_class()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	[ValueObject]
   public partial class TestValueObject
	{
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_struct()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	[ValueObject]
	public readonly partial struct TestValueObject
	{
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly });
      }
   }
}
