using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG103_ExplicitEqualityComparerWithoutComparer
{
   private const string _DIAGNOSTIC_ID = "TTRESG103";

   public class SmartEnums
   {
      [Fact]
      public async Task Should_trigger_when_having_equality_comparer_but_no_comparer()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               [KeyMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
            	public partial class {|#0:TestEnum|}
            	{
            	   public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               [KeyMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
                [KeyMemberComparer<ComparerAccessors.Default<int>, int>]
                public partial class TestEnum
            	{
            	   public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_string_based_when_having_equality_comparer_but_no_comparer()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
               [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
            	public partial class {|#0:TestEnum|}
            	{
            	   public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
               [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
                [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
                public partial class TestEnum
            	{
            	   public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_when_having_equality_comparer_and_comparer()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               [KeyMemberComparer<ComparerAccessors.Default<int>, int>]
               [KeyMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
            	public partial class {|#0:TestEnum|}
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_string_based_when_having_equality_comparer_and_comparer()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>]
               [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
               [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            	public partial class {|#0:TestEnum|}
            	{
            	   public static readonly TestEnum Item1 = default;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
      }
   }

   public class KeyedValueObjects
   {
      [Fact]
      public async Task Should_trigger_when_having_equality_comparer_but_no_comparer()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>]
               [KeyMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
            	public partial class {|#0:TestValueObject|}
            	{
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>]
               [KeyMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
                [KeyMemberComparer<ComparerAccessors.Default<int>, int>]
                public partial class TestValueObject
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_string_based_when_having_equality_comparer_but_no_comparer()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
               [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
            	public partial class {|#0:TestValueObject|}
            	{
               }
            }
            """;

         var expectedCode = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
               [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
                [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
                public partial class TestValueObject
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ValueObjectAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_when_having_equality_comparer_and_comparer()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>]
               [KeyMemberComparer<ComparerAccessors.Default<int>, int>]
               [KeyMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
            	public partial class {|#0:TestValueObject|}
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_string_based_when_having_equality_comparer_and_comparer()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
               [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
               [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            	public partial class {|#0:TestValueObject|}
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
      }
   }
}
