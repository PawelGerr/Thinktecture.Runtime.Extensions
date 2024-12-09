using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG042_InitAccessorMustBePrivate
{
   private const string _DIAGNOSTIC_ID = "TTRESG042";

   public class KeyedValueObject_InitAccessorMustBePrivate
   {
      [Fact]
      public async Task Should_trigger_on_public_property_with_default_init()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public partial class TestValueObject
            	{
                  public int {|#0:InstanceProperty|} { get; init; }
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
                  public int InstanceProperty { get; private init; }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_public_property_with_init_expression()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public partial class TestValueObject
            	{
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  public int {|#0:InstanceProperty|}
                  {
                     get => _instanceProperty;
                     init => _instanceProperty = value;
                  }
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
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  public int {|#0:InstanceProperty|}
                  {
                     get => _instanceProperty;
                     private init => _instanceProperty = value;
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_public_property_with_init_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public partial class TestValueObject
            	{
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  public int {|#0:InstanceProperty|}
                  {
                     get { return _instanceProperty; }
                     init { _instanceProperty = value; }
                  }
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
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  public int {|#0:InstanceProperty|}
                  {
                     get { return _instanceProperty; }
                     private init { _instanceProperty = value; }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_private_property_with_default_init()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public partial class TestValueObject
            	{
                  private int {|#0:InstanceProperty|} { get; init; }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_private_property_with_init_expression()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public partial class TestValueObject
            	{
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  private int {|#0:InstanceProperty|}
                  {
                     get => _instanceProperty;
                     init => _instanceProperty = value;
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_private_property_with_init_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public partial class TestValueObject
            	{
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  private int {|#0:InstanceProperty|}
                  {
                     get { return _instanceProperty; }
                     init { _instanceProperty = value; }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }
   }

   public class ComplexValueObject_InitAccessorMustBePrivate
   {
      [Fact]
      public async Task Should_trigger_on_public_property_with_default_init()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  public int {|#0:InstanceProperty|} { get; init; }
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
                  public int InstanceProperty { get; private init; }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_public_property_with_init_expression()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  public int {|#0:InstanceProperty|}
                  {
                     get => _instanceProperty;
                     init => _instanceProperty = value;
                  }
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
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  public int {|#0:InstanceProperty|}
                  {
                     get => _instanceProperty;
                     private init => _instanceProperty = value;
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_on_public_property_with_init_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  public int {|#0:InstanceProperty|}
                  {
                     get { return _instanceProperty; }
                     init { _instanceProperty = value; }
                  }
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
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  public int {|#0:InstanceProperty|}
                  {
                     get { return _instanceProperty; }
                     private init { _instanceProperty = value; }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InstanceProperty", "TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_private_property_with_default_init()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  private int {|#0:InstanceProperty|} { get; init; }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_private_property_with_init_expression()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  private int {|#0:InstanceProperty|}
                  {
                     get => _instanceProperty;
                     init => _instanceProperty = value;
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_private_property_with_init_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class TestValueObject
            	{
                  [ValueObjectMemberIgnore]
                  private readonly int _instanceProperty;
            
                  private int {|#0:InstanceProperty|}
                  {
                     get { return _instanceProperty; }
                     init { _instanceProperty = value; }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }
   }
}
