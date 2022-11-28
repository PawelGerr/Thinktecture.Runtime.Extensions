using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG013_DerivedTypeMustNotImplementEnumInterfaces
{
   private const string _DIAGNOSTIC_ID = "TTRESG013";

   [Fact]
   public async Task Should_trigger_if_derived_type_implements_IEnum()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private sealed partial class {|#0:InnerTestEnum|} : TestEnum, IEnum<string>
	   {
         public static readonly InnerTestEnum Item1 = default;
      }
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerTestEnum");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_if_derived_type_implements_IValidatableEnum()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private sealed partial class {|#0:InnerTestEnum|} : TestEnum, IValidatableEnum<string>
	   {
         public static readonly InnerTestEnum Item1 = default;
      }
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerTestEnum");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_derived_type_doesnt_implements_enum_interfaces()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private sealed partial class {|#0:InnerTestEnum|} : TestEnum
	   {
         public static readonly InnerTestEnum Item1 = default;
      }
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
   }
}
