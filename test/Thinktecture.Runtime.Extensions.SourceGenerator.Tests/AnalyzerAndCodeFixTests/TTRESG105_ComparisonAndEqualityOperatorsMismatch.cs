using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG105_ComparisonAndEqualityOperatorsMismatch
{
   private const string _DIAGNOSTIC_ID = "TTRESG105";

   public class SmartEnums
   {
      [Fact]
      public async Task CodeFix_should_align_equality_to_comparison_when_equality_lower()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:SmartEnum<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, EqualityComparisonOperators = OperatorsGeneration.Default)|}]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "DefaultWithKeyTypeOverloads", "Default");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task CodeFix_should_align_comparison_to_none_when_equality_none()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:SmartEnum<int>(ComparisonOperators = OperatorsGeneration.Default, EqualityComparisonOperators = OperatorsGeneration.None)|}]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>(ComparisonOperators = OperatorsGeneration.None, EqualityComparisonOperators = OperatorsGeneration.None)]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "Default", "None");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task CodeFix_should_align_comparison_to_equality_when_comparison_lower()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:SmartEnum<int>(ComparisonOperators = OperatorsGeneration.Default, EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)|}]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "Default", "DefaultWithKeyTypeOverloads");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task CodeFix_should_align_equality_to_none_when_comparison_none()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:SmartEnum<int>(ComparisonOperators = OperatorsGeneration.None, EqualityComparisonOperators = OperatorsGeneration.Default)|}]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>(ComparisonOperators = OperatorsGeneration.None, EqualityComparisonOperators = OperatorsGeneration.None)]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "None", "Default");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task CodeFix_should_handle_only_equality_set_explicit()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:SmartEnum<int>(EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)|}]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>(EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "Default", "DefaultWithKeyTypeOverloads");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task CodeFix_should_handle_only_comparison_set_explicit()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:SmartEnum<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)|}]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "DefaultWithKeyTypeOverloads", "Default");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }
   }

   public class ValueObjects
   {
      [Fact]
      public async Task CodeFix_should_align_equality_to_comparison_when_equality_lower()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:ValueObject<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, EqualityComparisonOperators = OperatorsGeneration.Default)|}]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject", "DefaultWithKeyTypeOverloads", "Default");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task CodeFix_should_align_comparison_to_none_when_equality_none()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:ValueObject<int>(ComparisonOperators = OperatorsGeneration.Default, EqualityComparisonOperators = OperatorsGeneration.None)|}]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>(ComparisonOperators = OperatorsGeneration.None, EqualityComparisonOperators = OperatorsGeneration.None)]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject", "Default", "None");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task CodeFix_should_align_comparison_to_equality_when_comparison_lower()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:ValueObject<int>(ComparisonOperators = OperatorsGeneration.Default, EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)|}]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject", "Default", "DefaultWithKeyTypeOverloads");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task CodeFix_should_align_equality_to_none_when_comparison_none()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:ValueObject<int>(ComparisonOperators = OperatorsGeneration.None, EqualityComparisonOperators = OperatorsGeneration.Default)|}]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>(ComparisonOperators = OperatorsGeneration.None, EqualityComparisonOperators = OperatorsGeneration.None)]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject", "None", "Default");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task CodeFix_should_handle_only_equality_set_explicit()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:ValueObject<int>(EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)|}]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>(EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject", "Default", "DefaultWithKeyTypeOverloads");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task CodeFix_should_handle_only_comparison_set_explicit()
      {
         var code = """
            using Thinktecture;

            namespace TestNamespace
            {
               [{|#0:ValueObject<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)|}]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expectedCode = """
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads, EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
               public partial class TestValueObject
               {
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject", "DefaultWithKeyTypeOverloads", "Default");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }
   }
}
