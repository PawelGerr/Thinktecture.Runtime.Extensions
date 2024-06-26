using System.Threading.Tasks;
using Thinktecture.Runtime.Tests.TestEnums;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG033_SwitchAndMapMustCoverAllItems
{
   private const string _DIAGNOSTIC_ID = "TTRESG039";

   [Fact]
   public async Task Should_trigger_on_Switch_missing_items_having_action()
   {
      var code = """

                 using System;
                 using Thinktecture;
                 using Thinktecture.Runtime.Tests.TestEnums;

                 namespace TestNamespace
                 {
                    public class Test
                    {
                       public void Do()
                       {
                          var testEnum = TestEnum.Item1;
                 
                          {|#0:testEnum.Switch(TestEnum.Item1, () => {},
                                             TestEnum.Item1, () => {})|};
                       }
                    }
                 }
                 """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "Item2");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
   }

   [Fact]
   public async Task Should_trigger_on_Switch_missing_items_having_action_with_context()
   {
      var code = """

                 using System;
                 using Thinktecture;
                 using Thinktecture.Runtime.Tests.TestEnums;

                 namespace TestNamespace
                 {
                    public class Test
                    {
                       public void Do()
                       {
                          var testEnum = TestEnum.Item1;

                          {|#0:testEnum.Switch(42,
                                             TestEnum.Item1, value => {},
                                             TestEnum.Item1, value => {})|};
                       }
                    }
                 }
                 """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "Item2");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_Switch_when_all_items_are_covered_having_action()
   {
      var code = """

                 using System;
                 using Thinktecture;
                 using Thinktecture.Runtime.Tests.TestEnums;

                 namespace TestNamespace
                 {
                    public class Test
                    {
                       public void Do()
                       {
                          var testEnum = TestEnum.Item1;
                 
                          {|#0:testEnum.Switch(TestEnum.Item1, () => {},
                                             TestEnum.Item2, () => {})|};
                       }
                    }
                 }
                 """;

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly });
   }

   [Fact]
   public async Task Should_trigger_on_Switch_missing_items_having_func()
   {
      var code = """

                 using System;
                 using Thinktecture;
                 using Thinktecture.Runtime.Tests.TestEnums;

                 namespace TestNamespace
                 {
                    public class Test
                    {
                       public void Do()
                       {
                          var testEnum = TestEnum.Item1;
                 
                          var returnValue = {|#0:testEnum.Switch(TestEnum.Item1, () => 1,
                                             TestEnum.Item1, () => 2)|};
                       }
                    }
                 }
                 """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "Item2");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_Switch_when_all_items_are_covered_having_func()
   {
      var code = """

                 using System;
                 using Thinktecture;
                 using Thinktecture.Runtime.Tests.TestEnums;

                 namespace TestNamespace
                 {
                    public class Test
                    {
                       public void Do()
                       {
                          var testEnum = TestEnum.Item1;
                 
                          var returnValue = {|#0:testEnum.Switch(TestEnum.Item1, () => 1,
                                             TestEnum.Item2, () => 2)|};
                       }
                    }
                 }
                 """;

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly });
   }

   [Fact]
   public async Task Should_trigger_on_Map_missing_items()
   {
      var code = """

                 using System;
                 using Thinktecture;
                 using Thinktecture.Runtime.Tests.TestEnums;

                 namespace TestNamespace
                 {
                    public class Test
                    {
                       public void Do()
                       {
                          var testEnum = TestEnum.Item1;
                 
                          var returnValue = {|#0:testEnum.Map(TestEnum.Item1, 1,
                                             TestEnum.Item1, 2)|};
                       }
                    }
                 }
                 """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "Item2");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_Map_when_all_items_are_covered()
   {
      var code = """

                 using System;
                 using Thinktecture;
                 using Thinktecture.Runtime.Tests.TestEnums;

                 namespace TestNamespace
                 {
                    public class Test
                    {
                       public void Do()
                       {
                          var testEnum = TestEnum.Item1;
                 
                          var returnValue = {|#0:testEnum.Map(TestEnum.Item1, 1,
                                             TestEnum.Item2, 2)|};
                       }
                    }
                 }
                 """;

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly });
   }
}
