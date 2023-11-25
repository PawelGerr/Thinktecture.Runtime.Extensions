using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Thinktecture.Runtime.Tests.BaseClasses;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG035_BaseClassPropertyMustBeReadOnly
{
   private const string _DIAGNOSTIC_ID = "TTRESG035";

   public class SameAssembly_EnumBaseClass_properties_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_instance_property()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public int Property { get; set; }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public int Property { get; }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }

      [Fact]
      public async Task Should_trigger_on_static_property()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public static int Property { get; set; }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public static int Property { get; }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_getter_expression_body()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public int Property
                          {
                             get => 42;
                             set { }
                          }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_getter_expression_body()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public static int Property
                          {
                             get => 42;
                             set { }
                          }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_getter_body()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public int Property
                          {
                             get { return 42; }
                             set { }
                          }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_getter_body()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public static int Property
                          {
                             get { return 42; }
                             set { }
                          }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
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
                    
                       public class BaseClass
                       {
                          public int Property
                          {
                             set => Helper.Property = value;
                          }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
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
                    
                       public class BaseClass
                       {
                          public static int Property
                          {
                             set => Helper.Property = value;
                          }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_setter_body()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public int Property
                          {
                             set { }
                          }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_setter_body()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public static int Property
                          {
                             set { }
                          }
                       }
                    
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClass
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }

   public class OtherAssembly_EnumBaseClass_properties_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_instance_property()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClassWithInstanceProperty
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClassWithInstanceProperty").WithSeverity(DiagnosticSeverity.Warning);
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithInstanceProperty).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClassWithInstancePropertyWithGetterOnly
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithInstancePropertyWithGetterOnly).Assembly });
      }

      [Fact]
      public async Task Should_trigger_on_static_property()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClassWithStaticProperty
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClassWithStaticProperty").WithSeverity(DiagnosticSeverity.Warning);
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithStaticProperty).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClassWithStaticPropertyWithGetterOnly
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithStaticPropertyWithGetterOnly).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_setter_expression_body()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClassWithSetExpressionProperty
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithSetExpressionProperty).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_setter_expression_body()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClassWithSetExpressionStaticProperty
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithSetExpressionStaticProperty).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_setter_body()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClassWithSetBodyProperty
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithSetBodyProperty).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_setter_body()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(IsValidatable = true)]
                    	public sealed partial class {|#0:TestEnum|} : BaseClassWithSetBodyStaticProperty
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithSetBodyStaticProperty).Assembly });
      }

      public class Technically_not_possible_to_enforce
      {
         [Fact]
         public async Task Should_not_BUT_DOES_trigger_on_instance_property_with_getter_expression_body()
         {
            var code = """

                       using System;
                       using Thinktecture;
                       using Thinktecture.Runtime.Tests.BaseClasses;

                       namespace TestNamespace
                       {
                          [SmartEnum<string>(IsValidatable = true)]
                       	public sealed partial class {|#0:TestEnum|} : BaseClassWithGetExpressionProperty
                       	{
                             public static readonly TestEnum Item1 = default;
                          }
                       }
                       """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClassWithGetExpressionProperty").WithSeverity(DiagnosticSeverity.Warning);
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithGetExpressionProperty).Assembly }, expected);
         }

         [Fact]
         public async Task Should_not_BUT_DOES_trigger_on_static_property_with_getter_expression_body()
         {
            var code = """

                       using System;
                       using Thinktecture;
                       using Thinktecture.Runtime.Tests.BaseClasses;

                       namespace TestNamespace
                       {
                          [SmartEnum<string>(IsValidatable = true)]
                       	public sealed partial class {|#0:TestEnum|} : BaseClassWithGetExpressionStaticProperty
                       	{
                             public static readonly TestEnum Item1 = default;
                          }
                       }
                       """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClassWithGetExpressionStaticProperty").WithSeverity(DiagnosticSeverity.Warning);
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithGetExpressionStaticProperty).Assembly }, expected);
         }

         [Fact]
         public async Task Should_not_BUT_DOES_trigger_on_instance_property_with_getter_body()
         {
            var code = """

                       using System;
                       using Thinktecture;
                       using Thinktecture.Runtime.Tests.BaseClasses;

                       namespace TestNamespace
                       {
                          [SmartEnum<string>(IsValidatable = true)]
                       	public sealed partial class {|#0:TestEnum|} : BaseClassWithGetBodyProperty
                       	{
                             public static readonly TestEnum Item1 = default;
                          }
                       }
                       """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClassWithGetBodyProperty").WithSeverity(DiagnosticSeverity.Warning);
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithGetBodyProperty).Assembly }, expected);
         }

         [Fact]
         public async Task Should_not_BUT_DOES_trigger_on_static_property_with_getter_body()
         {
            var code = """

                       using System;
                       using Thinktecture;
                       using Thinktecture.Runtime.Tests.BaseClasses;

                       namespace TestNamespace
                       {
                          [SmartEnum<string>(IsValidatable = true)]
                       	public sealed partial class {|#0:TestEnum|} : BaseClassWithGetBodyStaticProperty
                       	{
                             public static readonly TestEnum Item1 = default;
                          }
                       }
                       """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClassWithGetBodyStaticProperty").WithSeverity(DiagnosticSeverity.Warning);
            await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly, typeof(BaseClassWithGetBodyStaticProperty).Assembly }, expected);
         }
      }
   }

   public class SameAssembly_KeyedValueObjectBaseClass_properties_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_instance_property()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public int Property { get; set; }
                       }
                    
                       [ValueObject<string>]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClass
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public int Property { get; }
                       }
                    
                       [ValueObject<string>]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClass
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }

      [Fact]
      public async Task Should_trigger_on_static_property()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public static int Property { get; set; }
                       }
                    
                       [ValueObject<string>]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClass
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public static int Property { get; }
                       }
                    
                       [ValueObject<string>]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClass
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }
   }

   public class SameAssembly_ComplexValueObjectBaseClass_properties_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_instance_property()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public int Property { get; set; }
                       }
                    
                       [ComplexValueObject]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClass
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public int Property { get; }
                       }
                    
                       [ComplexValueObject]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClass
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }

      [Fact]
      public async Task Should_trigger_on_static_property()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public static int Property { get; set; }
                       }
                    
                       [ComplexValueObject]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClass
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClass");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       public class BaseClass
                       {
                          public static int Property { get; }
                       }
                    
                       [ComplexValueObject]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClass
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }
   }

   public class OtherAssembly_KeyedValueObjectBaseClass_properties_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_instance_property()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [ValueObject<string>]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClassWithInstanceProperty
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClassWithInstanceProperty").WithSeverity(DiagnosticSeverity.Warning);
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithInstanceProperty).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [ValueObject<string>]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClassWithInstancePropertyWithGetterOnly
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithInstancePropertyWithGetterOnly).Assembly });
      }

      [Fact]
      public async Task Should_trigger_on_static_property()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [ValueObject<string>]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClassWithStaticProperty
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClassWithStaticProperty").WithSeverity(DiagnosticSeverity.Warning);
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithStaticProperty).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [ValueObject<string>]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClassWithStaticPropertyWithGetterOnly
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithStaticPropertyWithGetterOnly).Assembly });
      }
   }

   public class OtherAssembly_ComplexValueObjectBaseClass_properties_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_on_instance_property()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [ComplexValueObject]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClassWithInstanceProperty
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClassWithInstanceProperty").WithSeverity(DiagnosticSeverity.Warning);
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithInstanceProperty).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_instance_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [ComplexValueObject]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClassWithInstancePropertyWithGetterOnly
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithInstancePropertyWithGetterOnly).Assembly });
      }

      [Fact]
      public async Task Should_trigger_on_static_property()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [ComplexValueObject]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClassWithStaticProperty
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Property", "BaseClassWithStaticProperty").WithSeverity(DiagnosticSeverity.Warning);
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithStaticProperty).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_property_with_getter_only()
      {
         var code = """

                    using System;
                    using Thinktecture;
                    using Thinktecture.Runtime.Tests.BaseClasses;

                    namespace TestNamespace
                    {
                       [ComplexValueObject]
                    	public sealed partial class {|#0:TestValueObject|} : BaseClassWithStaticPropertyWithGetterOnly
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(BaseClassWithStaticPropertyWithGetterOnly).Assembly });
      }
   }
}
