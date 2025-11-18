using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG061_ObjectFactoryMustImplementValidateMethod
{
   private const string _DIAGNOSTIC_ID = "TTRESG061";

   [Fact]
   public async Task Should_trigger_when_Validate_method_is_missing()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class {|#0:TestClass|}
            {
            }
         }
         """;

      var expectedCode = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class {|#0:TestClass|}
            {

                 public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "string", "string?", "ValidationError");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_interface_IObjectFactory_is_present()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class {|#0:TestClass|} : IObjectFactory<TestClass, string, ValidationError>
            {
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "string", "string?", "ValidationError");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly],
                                         expected,
                                         Verifier.Diagnostic("CS0535", "'TestClass' does not implement interface member 'IObjectFactory<TestClass, string, ValidationError>.Validate(string?, IFormatProvider?, out TestClass?)'").WithSpan(8, 37, 8, 87));
   }

   [Fact]
   public async Task Should_not_trigger_when_Validate_is_using_configured_validation_error()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public class TestValidationError : IValidationError<TestValidationError>
            {
               public static TestValidationError Create(string message) => throw new NotImplementedException();
            }

            [ObjectFactory<string>]
            [ValidationError<TestValidationError>]
            public partial class TestClass
            {
               public static TestValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_Validate_is_explicitly_implemented()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class TestClass : IObjectFactory<TestClass, string, ValidationError>
            {
               static ValidationError? IObjectFactory<TestClass, string, ValidationError>.Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_trigger_when_Validate_of_other_interface_is_explicitly_implemented()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public interface ITestObjectFactory
            {
               static abstract ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item);
            }

            [ObjectFactory<string>]
            public partial class {|#0:TestClass|} : ITestObjectFactory
            {
               static ValidationError? ITestObjectFactory.Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expectedCode = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public interface ITestObjectFactory
            {
               static abstract ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item);
            }

            [ObjectFactory<string>]
            public partial class {|#0:TestClass|} : ITestObjectFactory
            {
               static ValidationError? ITestObjectFactory.Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "string", "string?", "ValidationError");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_Validate_has_wrong_parameter_count()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expectedCode = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "string", "string?", "ValidationError");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_Validate_has_nullable_struct_value_type()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expectedCode = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "int", "int", "ValidationError");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_Validate_has_wrong_second_parameter_type()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, string? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expectedCode = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, string? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "string", "string?", "ValidationError");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_Validate_has_wrong_third_parameter_type()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out string? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expectedCode = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out string? item)
               {
                  item = null;
                  return null;
               }

                 public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "string", "string?", "ValidationError");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_Validate_has_wrong_return_type()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public class TestValidationError : IValidationError<TestValidationError>
            {
               public static TestValidationError Create(string message) => throw new NotImplementedException();
            }

            [ObjectFactory<string>]
            public partial class {|#0:TestClass|}
            {
               public static TestValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "string", "string?", "ValidationError");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_Validate_is_not_using_configured_validation_error()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public class TestValidationError : IValidationError<TestValidationError>
            {
               public static TestValidationError Create(string message) => throw new NotImplementedException();
            }

            [ObjectFactory<string>]
            [ValidationError<TestValidationError>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "string", "string?", "TestValidationError");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_Validate_has_wrong_third_parameter_ref_kind()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, ref TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "string", "string?", "ValidationError");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_Validate_is_not_static()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class {|#0:TestClass|}
            {
               public ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "string", "string?", "ValidationError");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_for_struct_with_struct_value_parameter()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>]
            public partial struct TestStruct
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestStruct item)
               {
                  item = default;
                  return null;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_for_reference_type_with_reference_value_parameter()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial class TestClass
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_for_reference_type_with_struct_value_parameter()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_trigger_for_value_type_with_wrong_value_parameter_type()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expectedCode = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "int", "int", "ValidationError");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_for_struct_with_reference_value_parameter()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            public partial struct TestStruct
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestStruct item)
               {
                  item = default;
                  return null;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_trigger_for_one_of_two_object_factory()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            [ObjectFactory<int>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expectedCode = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>]
            [ObjectFactory<int>]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "TestClass?", "int", "int", "ValidationError");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }
}
