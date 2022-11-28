using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG037_EnumWithoutDerivedTypesMustBeSealed
{
   private const string _DIAGNOSTIC_ID = "TTRESG037";

   [Fact]
   public async Task Should_trigger_on_IValidatableEnum()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;
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

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_on_IEnum()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;
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
	public sealed partial class TestEnum : IEnum<string>
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
      await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_on_inner_type()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class TestEnum : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private class Type_1 : TestEnum
      {
         public class {|#0:Type_3|} : Type_1
         {
         }

         public sealed class Type_4 : Type_2
         {
         }
      }

      private class Type_2 : Type_1
      {
      }
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
	public partial class TestEnum : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private class Type_1 : TestEnum
      {
         public sealed class Type_3 : Type_1
         {
         }

         public sealed class Type_4 : Type_2
         {
         }
      }

      private class Type_2 : Type_1
      {
      }
   }

   // simulate source gen
   partial class TestEnum
   {
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Type_3");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_on_sibling_type()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class TestEnum : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private class Type_1 : TestEnum
      {
         public sealed class Type_3 : Type_1
         {
         }

         public class {|#0:Type_4|} : Type_2
         {
         }
      }

      private class Type_2 : Type_1
      {
      }
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
	public partial class TestEnum : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private class Type_1 : TestEnum
      {
         public sealed class Type_3 : Type_1
         {
         }

         public sealed class Type_4 : Type_2
         {
         }
      }

      private class Type_2 : Type_1
      {
      }
   }

   // simulate source gen
   partial class TestEnum
   {
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Type_4");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_IValidatableEnum_with_derived_type()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private sealed class DerivedType : TestEnum
      {
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

   [Fact]
   public async Task Should_not_trigger_on_IEnum_with_derived_type()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private sealed class DerivedType : TestEnum
      {
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

   [Fact]
   public async Task Should_not_trigger_on_IValidatableEnum_with_generic_derived_type()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IValidatableEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private sealed class DerivedType<T> : TestEnum
      {
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

   [Fact]
   public async Task Should_not_trigger_on_IEnum_with_generic_derived_type()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private sealed class DerivedType<T> : TestEnum
      {
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

   [Fact]
   public async Task Should_not_trigger_on_derived_types_which_are_derived()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private class Type_1 : TestEnum
      {
         public sealed class Type_3 : Type_1
         {
         }

         public sealed class Type_4 : Type_2
         {
         }
      }

      private class Type_2 : Type_1
      {
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

   [Fact]
   public async Task Should_not_trigger_on_abstract_type()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public abstract partial class {|#0:TestEnum|} : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }

   // simulate source gen
   partial class TestEnum
   {
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
   }

   [Fact]
   public async Task Should_not_trigger_on_derived_abstract_type()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public partial class {|#0:TestEnum|} : IEnum<string>
	{
      public static readonly TestEnum Item1 = default;

      private abstract class Type_1 : TestEnum
      {
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
