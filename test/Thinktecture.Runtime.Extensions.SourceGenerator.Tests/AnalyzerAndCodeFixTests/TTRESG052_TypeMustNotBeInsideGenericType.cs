using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG052_TypeMustNotBeInsideGenericType
{
   private const string _DIAGNOSTIC_ID = "TTRESG052";

   public class Enums_must_not_be_nested_in_generic_class
   {
      [Fact]
      public async Task Should_trigger_on_enum_directly_nested_in_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class Generic<T>
               {
                  [SmartEnum<int>]
                  public partial class {|#0:TestEnum|}
                  {
                     public static readonly TestEnum Item1 = null!;
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_enum_indirectly_nested_in_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class Generic<T>
               {
                  public class NonGeneric
                  {
                     [SmartEnum<int>]
                     public partial class {|#0:TestEnum|}
                     {
                        public static readonly TestEnum Item1 = null;
                     }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
      }
   }

   public class KeyedValue_objects_must_not_be_nested_in_generic_class
   {
      [Fact]
      public async Task Should_trigger_on_value_object_directly_nested_in_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class Generic<T>
               {
                  [ValueObject<int>]
                  public partial class {|#0:TestValueObject|}
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_value_object_indirectly_nested_in_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class Generic<T>
               {
                  public class NonGeneric
                  {
                     [ValueObject<int>]
                     public partial class {|#0:TestValueObject|}
                     {
                     }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
      }
   }

   public class ComplexValue_objects_must_not_be_nested_in_generic_class
   {
      [Fact]
      public async Task Should_trigger_on_complex_value_object_directly_nested_in_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class Generic<T>
               {
                  [ComplexValueObject]
                  public partial class {|#0:TestValueObject|}
                  {
                     public int Value { get; }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_complex_value_object_indirectly_nested_in_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class Generic<T>
               {
                  public class NonGeneric
                  {
                     [ComplexValueObject]
                     public partial class {|#0:TestValueObject|}
                     {
                        public int Value { get; }
                     }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
      }
   }
}
