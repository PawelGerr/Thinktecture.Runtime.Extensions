using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG038_ValueObjectMustBeSealed
{
   private const string _DIAGNOSTIC_ID = "TTRESG038";

   [Fact]
   public async Task Should_trigger_on_keyed_value_object()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public partial class {|#0:TestValueObject|}
	{
      public int Property { get; }
   }
}";

      var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public sealed partial class TestValueObject
	{
      public int Property { get; }
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ValueObjectAttribute).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_on_non_keyed_value_object()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public partial class {|#0:TestValueObject|}
	{
      public int Property { get; }
      public int Other { get; }
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
      public int Property { get; }
      public int Other { get; }
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ValueObjectAttribute).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_sealed_keyed_value_object()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public sealed partial class {|#0:TestValueObject|}
	{
      public int Property { get; }
   }
}";

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly });
   }

   [Fact]
   public async Task Should_not_trigger_on_sealed_non_keyed_value_object()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public sealed partial class {|#0:TestValueObject|}
	{
      public int Property { get; }
      public int Other { get; }
   }
}";

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ValueObjectAttribute).Assembly });
   }
}
