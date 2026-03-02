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

   public class SmartEnum
   {
      [Fact]
      public async Task Should_trigger_on_smart_enum_nested_in_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class GenericOuter<T>
               {
                  [SmartEnum<int>]
                  public partial class {|#0:InnerEnum|}
                  {
                     public static readonly InnerEnum Item1 = default;
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerEnum");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_smart_enum_nested_deeply_in_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class GenericOuter<T>
               {
                  public class NonGenericMiddle
                  {
                     [SmartEnum<int>]
                     public partial class {|#0:InnerEnum|}
                     {
                        public static readonly InnerEnum Item1 = default;
                     }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerEnum");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_smart_enum_nested_in_multiple_generic_classes()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class GenericOuter<T>
               {
                  public class GenericMiddle<U>
                  {
                     [SmartEnum<int>]
                     public partial class {|#0:InnerEnum|}
                     {
                        public static readonly InnerEnum Item1 = default;
                     }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerEnum");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_smart_enum_in_non_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class NonGenericOuter
               {
                  [SmartEnum<int>]
                  public partial class InnerEnum
                  {
                     public static readonly InnerEnum Item1 = default;
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_top_level_smart_enum()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly]);
      }
   }

   public class KeyedValueObject
   {
      [Fact]
      public async Task Should_trigger_on_keyed_value_object_nested_in_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class GenericOuter<T>
               {
                  [ValueObject<int>]
                  public partial struct {|#0:InnerValueObject|}
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_keyed_value_object_nested_deeply_in_generic_hierarchy()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class Level1<T>
               {
                  public class Level2
                  {
                     public class Level3<U>
                     {
                        [ValueObject<int>]
                        public partial struct {|#0:InnerValueObject|}
                        {
                        }
                     }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_keyed_value_object_in_non_generic_nested_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class Level1
               {
                  public class Level2
                  {
                     public class Level3
                     {
                        [ValueObject<int>]
                        public partial struct InnerValueObject
                        {
                        }
                     }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
      }
   }

   public class ComplexValueObject
   {
      [Fact]
      public async Task Should_trigger_on_complex_value_object_nested_in_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class GenericOuter<T>
               {
                  [ComplexValueObject]
                  public partial class {|#0:InnerValueObject|}
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_complex_value_object_nested_in_generic_struct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public struct GenericOuter<T>
               {
                  [ComplexValueObject]
                  public partial class {|#0:InnerValueObject|}
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_complex_value_object_at_namespace_level()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestValueObject
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }

   public class MixedScenarios
   {
      [Fact]
      public async Task Should_trigger_on_value_object_in_generic_class_that_also_has_non_generic_constraint()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class GenericOuter<T> where T : class
               {
                  [ValueObject<int>]
                  public partial struct {|#0:InnerValueObject|}
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_value_object_in_generic_record()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public record GenericOuter<T>
               {
                  [ValueObject<int>]
                  public partial struct {|#0:InnerValueObject|}
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InnerValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_when_nested_in_closed_generic_type()
      {
         // Note: This test verifies that the analyzer correctly handles a scenario
         // where we're nested inside what appears to be a generic type instantiation.
         // However, since C# doesn't allow nesting classes inside closed generic types
         // at the syntax level, this is just a documentation of expected behavior.
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               public class Outer
               {
                  [ValueObject<int>]
                  public partial struct TestValueObject
                  {
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
      }
   }
}
