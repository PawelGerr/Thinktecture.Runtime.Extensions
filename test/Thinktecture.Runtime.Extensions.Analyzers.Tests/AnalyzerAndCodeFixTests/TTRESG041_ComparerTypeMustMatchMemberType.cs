using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.CodeFixes;
using Thinktecture.CodeAnalysis.Diagnostics;
using Thinktecture.Runtime.Tests.Verifiers;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG041_ComparerTypeMustMatchMemberType
{
   private const string _DIAGNOSTIC_ID = "TTRESG041";

   public class SmartEnum_ComparerTypeMustMatchMemberType
   {
      [Fact]
      public async Task Should_trigger_if_comparable_type_differs_from_key_member_of_different_type()
      {
         var code = """

            #nullable enable

            using System;
            using System.Collections.Generic;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               [KeyMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
               [{|#0:KeyMemberComparer<ComparerAccessors.Default<string>, string>|}]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default!;
               }
            }
            """;

         var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Default<string>", "int");
         await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_comparable_type_equals_to_key_member_type()
      {
         var code = """

            #nullable enable

            using System;
            using System.Collections.Generic;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               [KeyMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
               [{|#0:KeyMemberComparer<ComparerAccessors.Default<int>, int>|}]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default!;
               }
            }
            """;

         await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_report_the_resolved_type_parameter_when_key_type_is_a_TypeParamRef_marker()
      {
         // The key type must be resolved to the type parameter before the comparer is checked. Without the
         // resolution the reported member type would be the marker type 'TypeParamRef1' instead of 'T'.
         // The comparer attribute cannot use 'T' itself, because attributes must not reference the type
         // parameters of the annotated type (CS8968), so a concrete comparer type is used here.
         var code = """

            #nullable enable

            using System;
            using System.Collections.Generic;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<TypeParamRef1>]
               [{|#0:KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>|}]
            	public partial class TestEnum<T>
                  where T : notnull
            	{
                  public static readonly TestEnum<T> Item1 = default!;
               }
            }
            """;

         var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StringOrdinalIgnoreCase", "T");
         await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }
   }

   public class KeyedValueObject_ComparerTypeMustMatchMemberType
   {
      [Fact]
      public async Task Should_trigger_if_comparable_type_differs_from_key_member_of_different_type()
      {
         var code = """

            #nullable enable

            using System;
            using System.Collections.Generic;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>]
               [KeyMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
               [{|#0:KeyMemberComparer<ComparerAccessors.Default<string>, string>|}]
            	public partial class TestValueObject
            	{
               }
            }
            """;

         var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Default<string>", "int");
         await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_comparable_type_equals_to_key_member_type()
      {
         var code = """

            #nullable enable

            using System;
            using System.Collections.Generic;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>]
               [KeyMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
               [{|#0:KeyMemberComparer<ComparerAccessors.Default<int>, int>|}]
            	public partial class TestValueObject
            	{
               }
            }
            """;

         await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }
}
