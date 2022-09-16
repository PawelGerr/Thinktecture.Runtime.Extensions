using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG006_TypeMustBePartial
{
   private const string _DIAGNOSTIC_ID = "TTRESG006";

   public class Enum_must_be_partial
   {
      [Fact]
      public async Task Should_trigger_on_non_partial_class()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public sealed class {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public sealed partial class {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_partial_struct()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public readonly struct {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public readonly partial struct {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_class()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public sealed partial class {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_struct()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public readonly partial struct {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }

   public class ValueObject_must_be_partial
   {
      [Fact]
      public async Task Should_trigger_on_non_partial_class()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public sealed class {|#0:TestValueObject|}
	{
   }
}";

         var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public sealed partial class {|#0:TestValueObject|}
	{
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_partial_struct()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public readonly struct {|#0:TestValueObject|}
	{
   }
}";

         var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public readonly partial struct {|#0:TestValueObject|}
	{
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_class()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public sealed partial class {|#0:TestValueObject|}
	{
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_struct()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public readonly partial struct {|#0:TestValueObject|}
	{
   }
}";

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly });
      }
   }
}
