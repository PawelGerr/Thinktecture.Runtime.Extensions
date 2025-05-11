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
         	public {{typeModifer}}partial class TestClass
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
         	public {{typeModifer}}partial class TestClass
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
   [InlineData("public IntBasedStructValueObjectDoesNotAllowDefaultStructs Member => default!;")]                                                                   // property: has expression body
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
}
