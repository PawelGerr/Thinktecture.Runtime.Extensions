using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG043_PrimaryConstructorNotAllowed
{
   private const string _DIAGNOSTIC_ID = "TTRESG043";

   public class Enum_cannot_have_a_primary_constructor
   {
      [Fact]
      public async Task Should_trigger_if_enum_is_class_and_has_primary_constructor()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>]
                       public partial class {|#0:TestEnum|}()
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_if_enum_is_struct_and_has_primary_constructor()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(IsValidatable = true)]
                       public partial struct {|#0:TestEnum|}()
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }
   }

   public class ValueObject_cannot_have_a_primary_constructor
   {
      [Fact]
      public async Task Should_trigger_if_value_object_is_class_and_has_primary_constructor()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<string>]
                       public partial class {|#0:ValueObject|}()
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("ValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_if_value_object_is_struct_and_has_primary_constructor()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<string>]
                       public partial struct {|#0:ValueObject|}()
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("ValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }
   }

   public class ComplexValueObject_cannot_have_a_primary_constructor
   {
      [Fact]
      public async Task Should_trigger_if_value_object_is_class_and_has_primary_constructor()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ComplexValueObject]
                       public partial class {|#0:ValueObject|}()
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("ValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_if_value_object_is_struct_and_has_primary_constructor()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ComplexValueObject]
                       public partial struct {|#0:ValueObject|}()
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("ValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }
   }

   public class AdHocUnion_cannot_have_a_primary_constructor
   {
      [Fact]
      public async Task Should_trigger_if_union_is_class_and_has_primary_constructor()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [Union<string, int>]
                       public partial class {|#0:TestUnion|}();
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_if_union_is_struct_and_has_primary_constructor()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [Union<string, int>]
                       public partial struct {|#0:TestUnion|}();
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_if_union_is_ref_struct_and_has_primary_constructor()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [Union<string, int>]
                       public ref partial struct {|#0:TestUnion|}();
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
      }
   }

   public class Union_cannot_have_a_primary_constructor
   {
      [Fact]
      public async Task Should_trigger_if_union_is_class_and_has_primary_constructor()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [Union]
                       public partial class {|#0:TestUnion|}();
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_if_union_is_record_and_has_primary_constructor()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [Union]
                       public partial record {|#0:TestUnion|}();
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }
   }
}
