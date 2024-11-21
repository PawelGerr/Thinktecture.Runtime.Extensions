using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG017_KeyMemberShouldNotBeNullable
{
   private const string _DIAGNOSTIC_ID = "TTRESG017";

   public class KeyedValueObject_key_member_should_not_be_nullable
   {
      [Fact]
      public async Task Should_trigger_if_key_member_is_nullable_reference_type()
      {
         var code = """

                    #nullable enable

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [{|#0:ValueObject<string?>|}]
                    	public partial class TestValueObject
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly },
                                            Verifier.Diagnostic("CS8970", "Type 'string' cannot be used in this context because it cannot be represented in metadata.").WithSpan(9, 5, 9, 25),
                                            Verifier.Diagnostic("CS8714", "The type 'string?' cannot be used as type parameter 'TKey' in the generic type or method 'ValueObjectAttribute<TKey>'. Nullability of type argument 'string?' doesn't match 'notnull' constraint.").WithSpan(9, 17, 9, 24).WithArguments("Thinktecture.ValueObjectAttribute<TKey>", "TKey", "string"),
                                            expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_key_member_is_non_nullable_reference_type()
      {
         var code = """

                    #nullable enable

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<string>]
                    	public partial class {|#0:TestValueObject|}
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_if_key_member_is_non_nullable_reference_type_and_nullability_is_not_active()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<string>]
                    	public partial class {|#0:TestValueObject|}
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }

      [Fact]
      public async Task Should_trigger_if_key_member_is_nullable_struct()
      {
         var code = """

                    #nullable enable

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [{|#0:ValueObject<int?>|}]
                    	public partial class TestValueObject
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly },
                                            Verifier.Diagnostic("CS8714", "The type 'int?' cannot be used as type parameter 'TKey' in the generic type or method 'ValueObjectAttribute<TKey>'. Nullability of type argument 'int?' doesn't match 'notnull' constraint.").WithSpan(9, 17, 9, 21).WithArguments("Thinktecture.ValueObjectAttribute<TKey>", "TKey", "int?"),
                                            expected);
      }

      [Fact]
      public async Task Should_trigger_if_key_member_is_nullable_struct_even_if_nullability_is_not_active()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [{|#0:ValueObject<int?>|}]
                    	public partial class TestValueObject
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Field");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_key_member_is_non_nullable_struct()
      {
         var code = """

                    #nullable enable

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<int>]
                    	public partial class {|#0:TestValueObject|}
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }
   }
}
