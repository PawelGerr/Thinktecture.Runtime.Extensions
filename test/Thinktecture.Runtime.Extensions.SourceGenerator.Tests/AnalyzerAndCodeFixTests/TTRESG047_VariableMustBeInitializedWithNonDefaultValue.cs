using System.Threading.Tasks;
using Thinktecture.Runtime.Tests.TestAdHocUnions;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG047_VariableMustBeInitializedWithNonDefaultValue
{
   private const string _DIAGNOSTIC_ID = "TTRESG047";

   public class Union
   {
      [Fact]
      public async Task Should_trigger_on_variable_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      TestUnion_struct_string_int testStruct = {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion_struct_string_int");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(TestUnion_struct_string_int).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_field_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class TestClass
               {
                   private TestUnion_struct_string_int _field;

                   public void TestMethod()
                   {
                      _field = {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion_struct_string_int");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(TestUnion_struct_string_int).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_property_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public required TestUnion_struct_string_int Property { get; set; }

                   public void TestMethod()
                   {
                      Property = {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion_struct_string_int");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(TestUnion_struct_string_int).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_tuple_construction_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      (_, TestUnion_struct_string_int value) = (42, {|#0:default(TestUnion_struct_string_int)|});
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion_struct_string_int");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(TestUnion_struct_string_int).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_coalesce_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public TestUnion_struct_string_int? Property { get; set; }

                   public void TestMethod()
                   {
                      Property ??= {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion_struct_string_int");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(TestUnion_struct_string_int).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_method_arg_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      OtherMethod({|#0:default(TestUnion_struct_string_int)|});
                   }

                   public void OtherMethod(TestUnion_struct_string_int value)
                   {
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion_struct_string_int");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(TestUnion_struct_string_int).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_parameter_with_default_value()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod(TestUnion_struct_string_int value = {|#0:default(TestUnion_struct_string_int)|})
                   {
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion_struct_string_int");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(TestUnion_struct_string_int).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_comparison_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      if(default(TestUnion_struct_string_int) == default)
                      {
                      }
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(TestUnion_struct_string_int).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_default_ctor()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      TestUnion_struct_string_int testStruct = {|#0:new()|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion_struct_string_int");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(TestUnion_struct_string_int).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }
   }

   public class KeyedValueObject
   {
      [Fact]
      public async Task Should_not_trigger_on_default_assignment_if_AllowDefaultStruct_is_true()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      IntBasedStructValueObjectWithAllowDefaultStructs testStruct = {|#0:default|};
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(IntBasedStructValueObjectWithAllowDefaultStructs).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_variable_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      StructValueObject testStruct = {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_field_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   private StructValueObject _field;

                   public void TestMethod()
                   {
                      _field = {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_property_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public required StructValueObject Property { get; set; }

                   public void TestMethod()
                   {
                      Property = {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_tuple_construction_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      (_, StructValueObject value) = (42, {|#0:default(StructValueObject)|});
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_coalesce_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public StructValueObject? Property { get; set; }

                   public void TestMethod()
                   {
                      Property ??= {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_method_arg_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      OtherMethod({|#0:default(StructValueObject)|});
                   }

                   public void OtherMethod(StructValueObject value)
                   {
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_parameter_with_default_value()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod(StructValueObject value = {|#0:default(StructValueObject)|})
                   {
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_comparison_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      if(default(StructValueObject) == default)
                      {
                      }
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_default_ctor()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      StructValueObject testStruct = {|#0:new()|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_default_ctor_if_AllowDefaultStruct_is_true()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      IntBasedStructValueObjectWithAllowDefaultStructs testStruct = {|#0:new()|};
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(IntBasedStructValueObjectWithAllowDefaultStructs).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }
   }

   public class ComplexValueObject
   {
      [Fact]
      public async Task Should_not_trigger_on_default_assignment_if_AllowDefaultStruct_is_true()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      BoundaryStructWithAllowDefaultStructs testStruct = {|#0:default|};
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(BoundaryStruct).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_variable_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      BoundaryStruct testStruct = {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("BoundaryStruct");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(BoundaryStruct).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_field_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   private BoundaryStruct _field;

                   public void TestMethod()
                   {
                      _field = {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("BoundaryStruct");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(BoundaryStruct).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_property_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public required BoundaryStruct Property { get; set; }

                   public void TestMethod()
                   {
                      Property = {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("BoundaryStruct");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(BoundaryStruct).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_tuple_construction_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      (_, BoundaryStruct value) = (42, {|#0:default(BoundaryStruct)|});
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("BoundaryStruct");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(BoundaryStruct).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_coalesce_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public BoundaryStruct? Property { get; set; }

                   public void TestMethod()
                   {
                      Property ??= {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("BoundaryStruct");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(BoundaryStruct).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_method_arg_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      OtherMethod({|#0:default(BoundaryStruct)|});
                   }

                   public void OtherMethod(BoundaryStruct value)
                   {
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("BoundaryStruct");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(BoundaryStruct).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_parameter_with_default_value()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod(BoundaryStruct value = {|#0:default(BoundaryStruct)|})
                   {
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("BoundaryStruct");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(BoundaryStruct).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_comparison_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      if(default(BoundaryStruct) == default)
                      {
                      }
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(BoundaryStruct).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_default_ctor()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      BoundaryStruct testStruct = {|#0:new()|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("BoundaryStruct");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(BoundaryStruct).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_default_ctor_if_AllowDefaultStruct_is_true()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      BoundaryStructWithAllowDefaultStructs testStruct = {|#0:new()|};
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(BoundaryStruct).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }
   }

   public class CompoundAssignment
   {
      [Fact]
      public async Task Should_trigger_on_compound_addition_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      StructValueObject value = StructValueObject.Create(42);
                      value += {|#0:default(StructValueObject)|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_compound_assignment_with_non_default_value()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      StructValueObject value = StructValueObject.Create(42);
                      value += StructValueObject.Create(10);
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }
   }

   public class DeconstructionAssignment
   {
      [Fact]
      public async Task Should_trigger_on_deconstruction_with_default_in_tuple()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      (int x, StructValueObject y) = (42, {|#0:default(StructValueObject)|});
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_nested_deconstruction_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      ((int a, StructValueObject b), int c) = ((1, {|#0:default|}), 3);
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_deconstruction_with_valid_value()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      (int x, StructValueObject y) = (42, StructValueObject.Create(10));
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }
   }

   public class ConditionalExpression
   {
      [Fact]
      public async Task Should_trigger_on_ternary_with_default_in_true_branch()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod(bool condition)
                   {
                      StructValueObject value = condition ? {|#0:default|} : StructValueObject.Create(10);
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_ternary_with_default_in_false_branch()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod(bool condition)
                   {
                      StructValueObject value = condition ? StructValueObject.Create(10) : {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_ternary_with_default_in_both_branches()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod(bool condition)
                   {
                      StructValueObject value = condition ? {|#0:default|} : {|#1:default(StructValueObject)|};
                   }
               }
            }
            """;

         var expected1 = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         var expected2 = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(1).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected1, expected2);
      }

      [Fact]
      public async Task Should_not_trigger_on_ternary_with_valid_values()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod(bool condition)
                   {
                      StructValueObject value = condition ? StructValueObject.Create(5) : StructValueObject.Create(10);
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }
   }

   public class ArrayInitialization
   {
      [Fact]
      public async Task Should_trigger_on_array_initialization_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      StructValueObject[] array = new[] { StructValueObject.Create(1), {|#0:default|} };
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_collection_initialization_with_default()
      {
         var code = """

            using System;
            using System.Collections.Generic;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      List<StructValueObject> list = new List<StructValueObject> { StructValueObject.Create(1), {|#0:default|} };
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_array_initialization_with_valid_values()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      StructValueObject[] array = new[] { StructValueObject.Create(1), StructValueObject.Create(2) };
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }
   }

   public class ObjectInitializer
   {
      [Fact]
      public async Task Should_trigger_on_object_initializer_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class Container
               {
                  public required StructValueObject Value { get; init; }
               }

               public class TestClass
               {
                   public void TestMethod()
                   {
                      var container = new Container { Value = {|#0:default|} };
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_nested_object_initializer_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class Inner
               {
                  public required StructValueObject Value { get; init; }
               }

               public class Outer
               {
                  public required Inner InnerObject { get; init; }
               }

               public class TestClass
               {
                   public void TestMethod()
                   {
                      var outer = new Outer
                      {
                         InnerObject = new Inner { Value = {|#0:default|} }
                      };
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_object_initializer_with_valid_value()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class Container
               {
                  public required StructValueObject Value { get; init; }
               }

               public class TestClass
               {
                   public void TestMethod()
                   {
                      var container = new Container { Value = StructValueObject.Create(42) };
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }
   }

   public class ReturnStatement
   {
      [Fact]
      public async Task Should_trigger_on_return_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public StructValueObject TestMethod()
                   {
                      return {|#0:default|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_expression_bodied_member_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public StructValueObject GetValue() => {|#0:default|};
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_return_with_valid_value()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public StructValueObject TestMethod()
                   {
                      return StructValueObject.Create(42);
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }
   }

   public class NullableContexts
   {
      [Fact]
      public async Task Should_trigger_on_nullable_struct_assignment_with_default()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      StructValueObject? nullable = {|#0:default(StructValueObject)|};
                   }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("StructValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_nullable_struct_assignment_with_null()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestValueObjects;

            namespace TestNamespace
            {
               public class TestClass
               {
                   public void TestMethod()
                   {
                      StructValueObject? nullable = null;
                   }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(StructValueObject).Assembly, typeof(UnionAttribute<,>).Assembly]);
      }
   }
}
