using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG043_PrimaryConstructorNotAllowed
{
   private const string _DIAGNOSTIC_ID = "TTRESG043";

   public class Enum_cannot_have_a_primary_constructor
   {
      [Fact]
      public async Task Should_trigger_if_enum_is_class_and_has_primary_constructor()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>]
   public sealed partial class {|#0:TestEnum|}()
	{
      public static readonly TestEnum Item1 = default;
   }

   // simulate source gen
   partial class TestEnum
   {
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_if_enum_is_struct_and_has_primary_constructor()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>(IsValidatable = true)]
   public readonly partial struct {|#0:TestEnum|}()
	{
      public static readonly TestEnum Item1 = default;
   }

   // simulate source gen
   partial struct TestEnum
   {
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }
   }

   public class ValueObject_cannot_have_a_primary_constructor
   {
      [Fact]
      public async Task Should_trigger_if_value_object_is_class_and_has_primary_constructor()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
   public sealed partial class {|#0:ValueObject|}()
	{
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("ValueObject");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_if_value_object_is_struct_and_has_primary_constructor()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
   public readonly partial struct {|#0:ValueObject|}()
	{
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("ValueObject");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }
   }
}
