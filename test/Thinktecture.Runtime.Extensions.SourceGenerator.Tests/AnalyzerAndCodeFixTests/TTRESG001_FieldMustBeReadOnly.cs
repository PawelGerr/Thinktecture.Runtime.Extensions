using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG001_FieldMustBeReadOnly
{
   private const string _DIAGNOSTIC_ID = "TTRESG001";

   public class Enum_fields_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_non_readonly_enum_item()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class TestEnum
            	{
                  public static TestEnum {|#0:Item1|} = default;
                  public static readonly TestEnum Item2 = default;
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;
                  public static readonly TestEnum Item2 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Item1", "TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_static_non_readonly_non_enum_item()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;
            
                  public static object {|#0:SomeStaticField|} = default;
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;
            
                  public static readonly object {|#0:SomeStaticField|} = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("SomeStaticField", "TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_readonly_instance_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;
            
                  public int {|#0:InstanceField|};
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;
            
                  public readonly int InstanceField;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceField", "TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_readonly_instance_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;
            
                  public readonly int {|#0:InstanceField|};
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }

   public class KeyedValueObject_fields_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_static_non_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public partial class TestValueObject
            	{
                  public static object {|#0:Field|} = default;
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public partial class TestValueObject
            	{
                  public static readonly object {|#0:Field|} = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_instance_non_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public partial class TestValueObject
            	{
                  public object {|#0:Field|} = default;
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public partial class TestValueObject
            	{
                  public readonly object {|#0:Field|} = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }
   }

   public class ComplexValueObject_fields_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_static_non_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  public static object {|#0:Field|} = default;
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
                  public static readonly object {|#0:Field|} = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_instance_non_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  public object {|#0:Field|} = default;
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
                  public readonly object {|#0:Field|} = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }
   }
}
