using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG003_PropertyMustBeReadOnly
{
   private const string _DIAGNOSTIC_ID = "TTRESG003";

   public class Enum_properties_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_non_readonly_instance_property()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public int {|#0:InstanceProperty|} { get; set; }
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
   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public int InstanceProperty { get; }
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_readonly_static_property()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public static int {|#0:InstanceProperty|} { get; set; }
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
   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public static int InstanceProperty { get; }
   }

   // simulate source gen
	partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_getter_expression_body()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public int {|#0:InstanceProperty|}
      {
         get => 42;
         set { }
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
      public async Task Should_not_trigger_on_static_property_with_getter_expression_body()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public static int {|#0:InstanceProperty|}
      {
         get => 42;
         set { }
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
      public async Task Should_not_trigger_on_instance_property_with_getter_body()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public int {|#0:InstanceProperty|}
      {
         get { return 42; }
         set { }
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
      public async Task Should_not_trigger_on_static_property_with_getter_body()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public static int {|#0:InstanceProperty|}
      {
         get { return 42; }
         set { }
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
      public async Task Should_not_trigger_on_instance_property_with_setter_expression_body()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   public static class Helper
   {
      public static int Property { get; set; }
   }

   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public int {|#0:InstanceProperty|}
      {
         set => Helper.Property = value;
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
      public async Task Should_not_trigger_on_static_property_with_setter_expression_body()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   public static class Helper
   {
      public static int Property { get; set; }
   }

   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public static int {|#0:InstanceProperty|}
      {
         set => Helper.Property = value;
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
      public async Task Should_not_trigger_on_instance_property_with_setter_body()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public int {|#0:InstanceProperty|}
      {
         set { }
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
      public async Task Should_not_trigger_on_static_property_with_setter_body()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [SmartEnum<string>(IsValidatable = true)]
	public sealed partial class TestEnum
	{
      public static readonly TestEnum Item1 = default;

      public static int {|#0:InstanceProperty|}
      {
         set { }
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

   public class ValueObject_properties_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_non_readonly_instance_property()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public sealed partial class TestValueObject
	{
      public int {|#0:InstanceProperty|} { get; set; }
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
      public int InstanceProperty { get; }
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_readonly_static_property()
      {
         var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   [ValueObject]
	public sealed partial class TestValueObject
	{
      public static int {|#0:InstanceProperty|} { get; set; }
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
      public static int InstanceProperty { get; }
   }
}";

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ValueObjectAttribute).Assembly }, expected);
      }
   }
}
