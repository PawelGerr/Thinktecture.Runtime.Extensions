using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG040_DontImplementEnumInterfaceWithTwoGenerics
{
   private const string _DIAGNOSTIC_ID = "TTRESG040";

   [Fact]
   public async Task Should_trigger_on_IEnum_with_2_generics_with_incorrect_enum_type()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
	public sealed partial class OtherEnum : IEnum<string>
	{
      public static readonly OtherEnum Item1 = default;
   }

	public sealed partial class {|#0:TestEnum|} : IEnum<string, OtherEnum>, IEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }

   // simulate source gen
   partial class OtherEnum
   {
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
   }

   // simulate source generator
	public sealed partial class TestEnum
	{
      public static TestEnum Get(string key) => throw new System.NotImplementedException();
      public static bool TryGet(string key, out TestEnum item) => throw new System.NotImplementedException();
" +
#if NET7_0
                 @"
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
      public static global::System.Collections.Generic.IReadOnlyList<TestEnum> Items => default;
      static global::System.Collections.Generic.IReadOnlyList<OtherEnum> IEnum<string, OtherEnum>.Items => default;

      static OtherEnum IEnum<string, OtherEnum>.Get(string key) => throw new System.NotImplementedException();
      static bool IEnum<string, OtherEnum>.TryGet(string key, out OtherEnum item) => throw new System.NotImplementedException();

      public static System.ComponentModel.DataAnnotations.ValidationResult Validate(string key, out TestEnum obj) => throw new System.NotImplementedException();
      static System.ComponentModel.DataAnnotations.ValidationResult IKeyedValueObject<OtherEnum, string>.Validate(string key, out OtherEnum obj) => throw new System.NotImplementedException();
" +
#endif
                 @"   }
}";

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("string", "TestEnum");
      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_IEnum_with_2_generics_with_correct_keys()
   {
      var code = @"
using System;
using Thinktecture;

namespace TestNamespace
{
   // simulate source generator
	public sealed partial class TestEnum
	{
      public static global::System.Collections.Generic.IEqualityComparer<string> KeyEqualityComparer => default;
      public static global::System.Collections.Generic.IReadOnlyList<TestEnum> Items => default;

      public static TestEnum Get(string key) => throw new System.NotImplementedException();
      public static bool TryGet(string key, out TestEnum item) => throw new System.NotImplementedException();
"
#if NET7_0
+ @"
      public static System.ComponentModel.DataAnnotations.ValidationResult Validate(string key, out TestEnum obj) => throw new System.NotImplementedException();"
#endif
                 + @"
   }

	public sealed partial class {|#0:TestEnum|} : IEnum<string, TestEnum>, IEnum<string>
	{
      public static readonly TestEnum Item1 = default;
   }
}";

      await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
   }
}
