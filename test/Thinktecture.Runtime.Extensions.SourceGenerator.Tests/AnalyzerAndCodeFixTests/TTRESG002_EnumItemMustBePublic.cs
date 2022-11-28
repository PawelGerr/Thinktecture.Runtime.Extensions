using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG002_EnumItemMustBePublic
{
   private const string _DIAGNOSTIC_ID = "TTRESG002";

   [Fact]
   public async Task Should_trigger_on_internal_enum_item()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public sealed partial class TestEnum : IValidatableEnum<string>
	{
      internal static readonly TestEnum {|#0:Item1|} = default;
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public sealed partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Item1", "TestEnum");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_even_if_modifier_is_not_first_one()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public sealed partial class TestEnum : IValidatableEnum<string>
	{
      static private readonly TestEnum {|#0:Item1|} = default;
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public sealed partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Item1", "TestEnum");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_on_non_enum_item_without_explicit_modifier()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public sealed partial class TestEnum : IValidatableEnum<string>
	{
      static readonly TestEnum {|#0:Item1|} = default;
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      var expectedCode = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public sealed partial class TestEnum : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Item1", "TestEnum");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
   }
}
