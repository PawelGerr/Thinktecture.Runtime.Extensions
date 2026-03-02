using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG070_ObjectFactorySerializationFrameworksConflict
{
   private const string _DIAGNOSTIC_ID = "TTRESG070";

   [Fact]
   public async Task Should_trigger_when_two_attributes_both_have_SystemTextJson()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
               public string ToValue(int _) => "";
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "SystemTextJson");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected,
         Verifier.Diagnostic("TTRESG062").WithSpan(9, 25, 9, 34).WithArguments("TestClass", "string"));
   }

   [Fact]
   public async Task Should_trigger_when_two_attributes_have_overlapping_frameworks_with_composite_flag()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.SystemTextJson | SerializationFrameworks.MessagePack)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
               public string ToValue(int _) => "";
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "SystemTextJson");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected,
         Verifier.Diagnostic("TTRESG062").WithSpan(9, 25, 9, 34).WithArguments("TestClass", "string"));
   }

   [Fact]
   public async Task Should_trigger_when_using_Json_composite_flag_that_overlaps_with_SystemTextJson()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.Json)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
               public string ToValue(int _) => "";
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "SystemTextJson");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected,
         Verifier.Diagnostic("TTRESG062").WithSpan(9, 25, 9, 34).WithArguments("TestClass", "string"));
   }

   [Fact]
   public async Task Should_trigger_when_using_Json_composite_flag_that_overlaps_with_NewtonsoftJson()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.Json)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
               public string ToValue(int _) => "";
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "NewtonsoftJson");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected,
         Verifier.Diagnostic("TTRESG062").WithSpan(9, 25, 9, 34).WithArguments("TestClass", "string"));
   }

   [Fact]
   public async Task Should_trigger_for_MessagePack_overlap_with_All()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.MessagePack)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
               public string ToValue(int _) => "";
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "MessagePack");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected,
         Verifier.Diagnostic("TTRESG062").WithSpan(9, 25, 9, 34).WithArguments("TestClass", "string"));
   }

   [Fact]
   public async Task Should_trigger_when_both_use_All()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.All)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
               public string ToValue(int _) => "";
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "All");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly],
         Verifier.Diagnostic("TTRESG062").WithSpan(9, 25, 9, 34).WithArguments("TestClass", "string"),
         expected);
   }

   [Fact]
   public async Task Should_trigger_when_using_Json_flag_with_both_SystemTextJson_and_NewtonsoftJson()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.Json)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.Json)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
               public string ToValue(int _) => "";
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "Json");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly],
         Verifier.Diagnostic("TTRESG062").WithSpan(9, 25, 9, 34).WithArguments("TestClass", "string"),
         expected);
   }

   [Fact]
   public async Task Should_trigger_with_three_attributes_having_multiple_overlaps()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson | SerializationFrameworks.MessagePack)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.MessagePack)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(Guid value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
               public string ToValue(int _) => "";
               public Guid ToValue(string _) => Guid.Empty;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "SystemTextJson");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly],
         Verifier.Diagnostic("TTRESG062").WithSpan(10, 25, 10, 34).WithArguments("TestClass", "Guid"),
         Verifier.Diagnostic("TTRESG062").WithSpan(10, 25, 10, 34).WithArguments("TestClass", "string"),
         expected);
   }

   [Fact]
   public async Task Should_not_trigger_when_frameworks_are_distinct()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly],
         Verifier.Diagnostic("TTRESG062").WithSpan(9, 25, 9, 34).WithArguments("TestClass", "int"),
         Verifier.Diagnostic("TTRESG062").WithSpan(9, 25, 9, 34).WithArguments("TestClass", "string"));
   }

   [Fact]
   public async Task Should_not_trigger_when_frameworks_are_completely_different()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(Guid value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly],
         Verifier.Diagnostic("TTRESG062").WithSpan(10, 25, 10, 34).WithArguments("TestClass", "int"),
         Verifier.Diagnostic("TTRESG062").WithSpan(10, 25, 10, 34).WithArguments("TestClass", "string"),
         Verifier.Diagnostic("TTRESG062").WithSpan(10, 25, 10, 34).WithArguments("TestClass", "Guid"));
   }

   [Fact]
   public async Task Should_not_trigger_when_only_one_attribute_has_frameworks()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.All)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.None)]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_all_attributes_have_None()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.None)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.None)]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_for_single_attribute()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.All)]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_UseForSerialization_not_explicitly_set()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>]
            [ObjectFactory<string>]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }
}
