using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG062_ObjectFactoryMustImplementToValueMethod
{
   private const string _DIAGNOSTIC_ID = "TTRESG062";

   [Fact]
   public async Task Should_trigger_when_ToValue_is_missing_and_UseForSerialization_is_All()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public string ToValue()
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "string");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_interface_IConvertible_is_present()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|} : IConvertible<string>
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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|} : IConvertible<string>
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public string ToValue()
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "string");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_ToValue_is_missing_and_UseForSerialization_is_SystemTextJson()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public string ToValue()
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "string");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_ToValue_is_missing_and_UseWithEntityFramework_is_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseWithEntityFramework = true)]
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
            [ObjectFactory<string>(UseWithEntityFramework = true)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public string ToValue()
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "string");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_when_ToValue_is_missing_and_UseForSerialization_is_None_and_UseWithEntityFramework_is_false()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.None, UseWithEntityFramework = false)]
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
   public async Task Should_not_trigger_when_ToValue_is_missing_and_UseForSerialization_is_not_specified()
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
   public async Task Should_not_trigger_when_ToValue_is_properly_implemented()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class TestClass
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public string ToValue()
               {
                  return string.Empty;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_ToValue_is_explicitly_implemented()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class TestClass : IConvertible<string>
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               string IConvertible<string>.ToValue()
               {
                  return string.Empty;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_trigger_when_ToValue_of_other_interface_is_explicitly_implemented()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public interface ITestConvertible
            {
               string ToValue();
            }

            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|} : ITestConvertible
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               string ITestConvertible.ToValue()
               {
                  return string.Empty;
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
            public interface ITestConvertible
            {
               string ToValue();
            }

            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|} : ITestConvertible
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               string ITestConvertible.ToValue()
               {
                  return string.Empty;
               }

                 public string ToValue()
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "string");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_ToValue_has_parameters()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public string ToValue(int parameter)
               {
                  return string.Empty;
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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public string ToValue(int parameter)
               {
                  return string.Empty;
               }

                 public string ToValue()
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "string");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_ToValue_has_wrong_return_type()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue()
               {
                  return 0;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "string");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_ToValue_is_static()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static string ToValue()
               {
                  return string.Empty;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "string");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_for_struct_with_ToValue_method()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial struct TestStruct
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestStruct item)
               {
                  item = default;
                  return null;
               }

               public string ToValue()
               {
                  return string.Empty;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_trigger_when_ToValue_is_missing_and_both_UseForSerialization_and_UseWithEntityFramework_are_set()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson, UseWithEntityFramework = true)]
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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson, UseWithEntityFramework = true)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public string ToValue()
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "string");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_when_ToValue_returns_nullable_reference_type()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class TestClass
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  throw new NotImplementedException();
               }

               public string ToValue()
               {
                  throw new NotImplementedException();
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_trigger_when_ToValue_is_missing_and_UseForSerialization_is_MessagePack()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public string ToValue()
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "string");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_ToValue_is_missing_and_UseForSerialization_is_NewtonsoftJson()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

                 public string ToValue()
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "string");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|} : IConvertible<string>
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               string IConvertible<string>.ToValue()
               {
                  throw new NotImplementedException();
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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|} : IConvertible<string>
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               string IConvertible<string>.ToValue()
               {
                  throw new NotImplementedException();
               }

                 public int ToValue()
                 {
                     throw new NotImplementedException();
                 }
             }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "int");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }
}
