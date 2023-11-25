using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG045_CustomKeyMemberImplementationTypeMismatch
{
   private const string _DIAGNOSTIC_ID = "TTRESG045";

   public class Enum_CustomKeyMemberImplementationTypeMismatch
   {
      [Fact]
      public async Task Should_trigger_if_key_member_type_differs_from_type_specified_by_attribute()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(SkipKeyMember = true)]
                       public sealed partial class TestEnum
                    	{
                          public int {|#0:Key|} { get; }
                    
                          public static readonly TestEnum Item1 = default!;
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Key", "int", "string");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_if_key_member_type_is_nullable_reference_type()
      {
         var code = """

                    #nullable enable

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(SkipKeyMember = true)]
                       public sealed partial class TestEnum
                    	{
                          public string? {|#0:Key|} { get; }
                    
                          public static readonly TestEnum Item1 = default!;
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Key", "string?", "string");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_if_key_member_type_is_nullable_struct()
      {
         var code = """

                    #nullable enable

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [SmartEnum<int>(SkipKeyMember = true)]
                       public sealed partial class TestEnum
                    	{
                          public int? {|#0:Key|} { get; }
                    
                          public static readonly TestEnum Item1 = default!;
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Key", "int?", "int");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_key_member_type_is_correct()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(SkipKeyMember = true)]
                       public sealed partial class {|#0:TestEnum|}
                    	{
                          public string Key { get; }
                    
                          public static readonly TestEnum Item1 = default!;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }

   public class KeyedValueObject_CustomKeyMemberImplementationTypeMismatch
   {
      [Fact]
      public async Task Should_trigger_if_key_member_type_differs_from_type_specified_by_attribute()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<string>(SkipKeyMember = true)]
                       public sealed partial class ValueObject
                    	{
                          private readonly int {|#0:_value|};
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_value", "int", "string");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_if_key_member_type_is_nullable_reference_type()
      {
         var code = """

                    #nullable enable

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<string>(SkipKeyMember = true)]
                       public sealed partial class ValueObject
                    	{
                          private readonly string? {|#0:_value|};
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_value", "string?", "string");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_if_key_member_type_is_nullable_struct()
      {
         var code = """

                    #nullable enable

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<int>(SkipKeyMember = true)]
                       public sealed partial class ValueObject
                    	{
                          private readonly int? {|#0:_value|};
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("_value", "int?", "int");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_key_member_type_is_correct()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<string>(SkipKeyMember = true)]
                       public sealed partial class {|#0:ValueObject|}
                    	{
                          private readonly string _value;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }
}
