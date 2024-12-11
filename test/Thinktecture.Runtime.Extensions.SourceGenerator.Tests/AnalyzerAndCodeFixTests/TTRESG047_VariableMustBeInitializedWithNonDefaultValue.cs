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
                   public TestUnion_struct_string_int Property { get; set; }
            
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
                   public StructValueObject Property { get; set; }
            
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
                   public BoundaryStruct Property { get; set; }
            
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
}
