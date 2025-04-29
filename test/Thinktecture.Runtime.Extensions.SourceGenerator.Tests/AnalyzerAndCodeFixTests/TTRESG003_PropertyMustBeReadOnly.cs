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
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public int {|#0:InstanceProperty|} { get; set; }
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public int InstanceProperty { get; }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ISmartEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_readonly_static_property()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public static int {|#0:InstanceProperty|} { get; set; }
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public static int InstanceProperty { get; }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ISmartEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_getter_expression_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public int {|#0:InstanceProperty|}
                  {
                     get => 42;
                     set { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_getter_expression_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public static int {|#0:InstanceProperty|}
                  {
                     get => 42;
                     set { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_getter_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public int {|#0:InstanceProperty|}
                  {
                     get { return 42; }
                     set { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_getter_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public static int {|#0:InstanceProperty|}
                  {
                     get { return 42; }
                     set { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_setter_expression_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public static class Helper
               {
                  public static int Property { get; set; }
               }

               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public int {|#0:InstanceProperty|}
                  {
                     set => Helper.Property = value;
                  }
               }
            }
            """;
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_setter_expression_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public static class Helper
               {
                  public static int Property { get; set; }
               }

               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public static int {|#0:InstanceProperty|}
                  {
                     set => Helper.Property = value;
                  }
               }
            }
            """;
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_setter_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public int {|#0:InstanceProperty|}
                  {
                     set { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_setter_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;

                  public static int {|#0:InstanceProperty|}
                  {
                     set { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
      }
   }

   public class KeyedValueObject_properties_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_non_readonly_instance_property()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>]
            	public partial class TestValueObject
            	{
                  public int {|#0:InstanceProperty|} { get; set; }
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>]
            	public partial class TestValueObject
            	{
                  public int InstanceProperty { get; }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_readonly_static_property()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>]
            	public partial class TestValueObject
            	{
                  public static int {|#0:InstanceProperty|} { get; set; }
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>]
            	public partial class TestValueObject
            	{
                  public static int InstanceProperty { get; }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }
   }

   public class ComplexValueObject_properties_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_non_readonly_instance_property()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  public int {|#0:InstanceProperty|} { get; set; }
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  public int InstanceProperty { get; }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_readonly_static_property()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  public static int {|#0:InstanceProperty|} { get; set; }
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  public static int InstanceProperty { get; }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }
   }
}
