using System.Threading.Tasks;
using Thinktecture.Runtime.Tests.BaseClasses;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG034_BaseClassFieldMustBeReadOnly
{
   private const string _DIAGNOSTIC_ID = "TTRESG034";

   public class SameAssembly_EnumBaseClass_fields_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_static_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public static object Field = default;
               }
            
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class {|#0:TestEnum|} : BaseClass
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_readonly_static_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public static readonly object Field = default;
               }
            
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class {|#0:TestEnum|} : BaseClass
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_instance_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public object Field = default;
               }
            
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class {|#0:TestEnum|} : BaseClass
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_readonly_instance_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public readonly object Field = default;
               }
            
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class {|#0:TestEnum|} : BaseClass
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly]);
      }
   }

   public class OtherAssembly_EnumBaseClass_fields_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_static_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class {|#0:TestEnum|} : BaseClassWithStaticField
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClassWithStaticField");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly, typeof(BaseClassWithStaticField).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class {|#0:TestEnum|} : BaseClassWithStaticReadonlyField
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly, typeof(BaseClassWithStaticReadonlyField).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_instance_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class {|#0:TestEnum|} : BaseClassWithInstanceField
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClassWithInstanceField");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly, typeof(BaseClassWithInstanceField).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_readonly_instance_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class {|#0:TestEnum|} : BaseClassWithInstanceReadonlyField
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly, typeof(BaseClassWithInstanceReadonlyField).Assembly]);
      }
   }

   public class SameAssembly_KeyedValueObjectBaseClass_fields_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_static_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public static object Field = default;
               }

               [ValueObject<int>]
            	public partial class {|#0:TestValueObject|} : BaseClass
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public static readonly object Field = default;
               }

               [ValueObject<int>]
            	public partial class {|#0:TestValueObject|} : BaseClass
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_instance_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public object Field = default;
               }

               [ValueObject<int>]
            	public partial class {|#0:TestValueObject|} : BaseClass
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public readonly object Field = default;
               }

               [ValueObject<int>]
            	public partial class {|#0:TestValueObject|} : BaseClass
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }

   public class SameAssembly_ComplexValueObjectBaseClass_fields_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_static_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public static object Field = default;
               }
            
               [ComplexValueObject]
            	public partial class {|#0:TestValueObject|} : BaseClass
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public static readonly object Field = default;
               }
            
               [ComplexValueObject]
            	public partial class {|#0:TestValueObject|} : BaseClass
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_instance_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public object Field = default;
               }
            
               [ComplexValueObject]
            	public partial class {|#0:TestValueObject|} : BaseClass
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class BaseClass
               {
                  public readonly object Field = default;
               }
            
               [ComplexValueObject]
            	public partial class {|#0:TestValueObject|} : BaseClass
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }

   public class OtherAssembly_KeyedValueObjectBaseClass_fields_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_static_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [ValueObject<int>]
            	public partial class {|#0:TestValueObject|} : BaseClassWithStaticField
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClassWithStaticField");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithStaticField).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [ValueObject<int>]
            	public partial class {|#0:TestValueObject|} : BaseClassWithStaticReadonlyField
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithStaticReadonlyField).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_instance_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [ValueObject<int>]
            	public partial class {|#0:TestValueObject|} : BaseClassWithInstanceField
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClassWithInstanceField");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithInstanceField).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [ValueObject<int>]
            	public partial class {|#0:TestValueObject|} : BaseClassWithInstanceReadonlyField
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithInstanceReadonlyField).Assembly]);
      }
   }

   public class OtherAssembly_ComplexValueObjectBaseClass_fields_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_static_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class {|#0:TestValueObject|} : BaseClassWithStaticField
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClassWithStaticField");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithStaticField).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class {|#0:TestValueObject|} : BaseClassWithStaticReadonlyField
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithStaticReadonlyField).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_instance_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class {|#0:TestValueObject|} : BaseClassWithInstanceField
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field", "BaseClassWithInstanceField");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithInstanceField).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.BaseClasses;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class {|#0:TestValueObject|} : BaseClassWithInstanceReadonlyField
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithInstanceReadonlyField).Assembly]);
      }
   }
}
