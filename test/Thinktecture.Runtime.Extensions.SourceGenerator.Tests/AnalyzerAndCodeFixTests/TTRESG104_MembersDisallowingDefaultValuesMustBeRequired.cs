using System.Threading.Tasks;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG104_MembersDisallowingDefaultValuesMustBeRequired
{
   private const string _DIAGNOSTIC_ID = "TTRESG104";

   [Theory]
   [InlineData("field", "IntBasedStructValueObjectDoesNotAllowDefaultStructs", "IntBasedStructValueObjectDoesNotAllowDefaultStructs Member;")]                          // field: non-readonly VO
   [InlineData("field", "TestUnion_struct_string_int", "TestUnion_struct_string_int Member;")]                                                                          // field: non-readonly DU
   [InlineData("property", "IntBasedStructValueObjectDoesNotAllowDefaultStructs", "IntBasedStructValueObjectDoesNotAllowDefaultStructs Member { get; set; }")]          // property: non-readonly VO
   [InlineData("property", "TestUnion_struct_string_int", "TestUnion_struct_string_int Member { get; set; }")]                                                          // property: non-readonly VO
   [InlineData("property", "IntBasedStructValueObjectDoesNotAllowDefaultStructs", "IntBasedStructValueObjectDoesNotAllowDefaultStructs Member { get; init; }")]         // property: non-readonly with init
   [InlineData("property", "IntBasedStructValueObjectDoesNotAllowDefaultStructs", "IntBasedStructValueObjectDoesNotAllowDefaultStructs Member { set { } }")]            // property: setter only
   [InlineData("property", "IntBasedStructValueObjectDoesNotAllowDefaultStructs", "abstract IntBasedStructValueObjectDoesNotAllowDefaultStructs Member { get; set; }")] // property: abstract
   public async Task Should_trigger_on_members(
      string memberKind,
      string memberType,
      string member)
   {
      var typeModifer = member.Contains("abstract") ? "abstract " : null;

      var code = $$"""

         using System;
         using Thinktecture;
         using Thinktecture.Runtime.Tests.TestValueObjects;
         using Thinktecture.Runtime.Tests.TestEnums;
         using Thinktecture.Runtime.Tests.TestAdHocUnions;

         namespace TestNamespace
         {
         	public {{typeModifer}} class TestClass
         	{
         	   {|#0:public {{member}}|}
            }
         }
         """;

      var fixedMember = member.Contains("abstract")
                           ? member.Replace("abstract", "abstract required")
                           : $"required {member}";

      var expectedCode = $$"""

         using System;
         using Thinktecture;
         using Thinktecture.Runtime.Tests.TestValueObjects;
         using Thinktecture.Runtime.Tests.TestEnums;
         using Thinktecture.Runtime.Tests.TestAdHocUnions;

         namespace TestNamespace
         {
         	public {{typeModifer}} class TestClass
         	{
         	   {|#0:public {{fixedMember}}|}
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(memberKind, "Member", memberType);
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ValueObjectAttribute<>).Assembly, typeof(IntBasedStructValueObjectDoesNotAllowDefaultStructs).Assembly], expected);
   }

   [Theory]
   [InlineData("internal IntBasedStructValueObjectDoesNotAllowDefaultStructs Member;")]                                                                             // field: less visible
   [InlineData("internal IntBasedStructValueObjectDoesNotAllowDefaultStructs Member { get; set; }")]                                                                // property: less visible
   [InlineData("public IntBasedStructValueObjectDoesNotAllowDefaultStructs Member { get; internal set; }")]                                                         // property: setter less visible
   [InlineData("public required IntBasedStructValueObjectDoesNotAllowDefaultStructs Member;")]                                                                      // field: required
   [InlineData("public required IntBasedStructValueObjectDoesNotAllowDefaultStructs Member { get; set; }")]                                                         // property: required
   [InlineData("public required TestUnion_struct_string_int Member { get; set; }")]                                                                                 // property: required
   [InlineData("public static IntBasedStructValueObjectDoesNotAllowDefaultStructs Member;")]                                                                        // field: static
   [InlineData("public static IntBasedStructValueObjectDoesNotAllowDefaultStructs Member { get; set; }")]                                                           // property: static
   [InlineData("public IntBasedStructValueObjectDoesNotAllowDefaultStructs? Member;")]                                                                              // field: nullable VO
   [InlineData("public IntBasedStructValueObjectDoesNotAllowDefaultStructs? Member { get; set; }")]                                                                 // property: nullable VO
   [InlineData("public TestUnion_struct_string_int? Member { get; set; }")]                                                                                         // property: nullable DU
   [InlineData("public IntBasedStructValueObject Member;")]                                                                                                         // field: AllowDefaultStructs = true
   [InlineData("public IntBasedStructValueObject Member { get; set; }")]                                                                                            // property: AllowDefaultStructs = true
   [InlineData("public IntBasedStructValueObjectDoesNotAllowDefaultStructs Member => (IntBasedStructValueObjectDoesNotAllowDefaultStructs)(object)42;")]            // property: expression body
   [InlineData("public IntBasedStructValueObjectDoesNotAllowDefaultStructs Member { get; }")]                                                                       // property: no setter
   [InlineData("public IntBasedStructValueObjectDoesNotAllowDefaultStructs Member { get; set; } = IntBasedStructValueObjectDoesNotAllowDefaultStructs.Create(1);")] // property: with initializer
   [InlineData("public IntBasedStructValueObjectDoesNotAllowDefaultStructs Member = IntBasedStructValueObjectDoesNotAllowDefaultStructs.Create(1);")]               // field: with initializer
   public async Task Should_not_trigger_on_members(string member)
   {
      var code = $$"""

         using System;
         using Thinktecture;
         using Thinktecture.Runtime.Tests.TestValueObjects;
         using Thinktecture.Runtime.Tests.TestEnums;
         using Thinktecture.Runtime.Tests.TestAdHocUnions;

         namespace TestNamespace
         {
         	public partial class TestClass
         	{
         	   {{member}}
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly, typeof(IntBasedStructValueObjectDoesNotAllowDefaultStructs).Assembly]);
   }

   public class PropertyEdgeCases
   {
      [Fact]
      public async Task Should_not_trigger_on_property_with_expression_body()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  private readonly StructValueObject _backing;

                  public StructValueObject DisallowingProperty => _backing;
               }

               [ValueObject<int>]
               public readonly partial struct StructValueObject : IDisallowDefaultValue
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_property_with_initializer()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  public StructValueObject DisallowingProperty { get; } = (StructValueObject)(object)42;
               }

               public readonly struct StructValueObject : IDisallowDefaultValue
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_property_less_visible_than_containing_type()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  private StructValueObject DisallowingProperty { get; }
               }

               [ValueObject<int>]
               public readonly partial struct StructValueObject : IDisallowDefaultValue
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_interface_property()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public interface IMyInterface
               {
                  IntBasedStructValueObjectDoesNotAllowDefaultStructs DisallowingProperty { get; set; }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly, typeof(IntBasedStructValueObjectDoesNotAllowDefaultStructs).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_special_type_property()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  public int IntProperty { get; }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }

   public class FieldEdgeCases
   {
      [Fact]
      public async Task Should_not_trigger_on_field_without_initializer()
      {
         // there could be a constructor that initializes the field

         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  public readonly StructValueObject {|#0:DisallowingField|};
               }

               [ValueObject<int>]
               public readonly partial struct StructValueObject : IDisallowDefaultValue
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_field_with_initializer()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  public readonly StructValueObject DisallowingField = new StructValueObject();
               }

               [ValueObject<int>(AllowDefaultStructs = true)]
               public readonly partial struct StructValueObject
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_readonly_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  public readonly StructValueObject DisallowingField;
               }

               [ValueObject<int>]
               public readonly partial struct StructValueObject : IDisallowDefaultValue
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_static_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  public static readonly StructValueObject DisallowingField;
               }

               public readonly struct StructValueObject : IDisallowDefaultValue
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_field_less_visible_than_containing_type()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  private readonly StructValueObject DisallowingField;
               }

               [ValueObject<int>]
               public readonly partial struct StructValueObject : IDisallowDefaultValue
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_reference_type_field()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  public readonly DisallowingClass DisallowingField;
               }

               [ValueObject<int>]
               public partial class DisallowingClass
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_multiple_field_declarations_in_same_statement_without_initializers()
      {
         // there could be a constructor that initializes the field

         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  public readonly StructValueObject {|#0:Field1|}, {|#1:Field2|};
               }

               public readonly struct StructValueObject : IDisallowDefaultValue
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }

   public class MultipleViolations
   {
      [Fact]
      public async Task Should_not_trigger_on_both_field_and_property_in_same_class()
      {
         // both could be initialized in constructor

         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  public readonly StructValueObject {|#0:DisallowingField|};
                  public StructValueObject {|#1:DisallowingProperty|} { get; }
               }

               public readonly struct StructValueObject : IDisallowDefaultValue
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_multiple_properties_with_different_disallowing_types()
      {
         // both could be initialized in constructor

         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
               public partial class TestClass
               {
                  public StructValueObject1 {|#0:Property1|} { get; }
                  public StructValueObject2 {|#1:Property2|} { get; }
               }

               public readonly struct StructValueObject1 : IDisallowDefaultValue
               {
               }

               public readonly struct StructValueObject2 : IDisallowDefaultValue
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }
}
