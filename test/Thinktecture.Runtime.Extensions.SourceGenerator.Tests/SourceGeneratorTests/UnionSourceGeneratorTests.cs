using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.Unions;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class UnionSourceGeneratorTests : SourceGeneratorTestsBase
{
   public UnionSourceGeneratorTests(ITestOutputHelper output)
      : base(output)
   {
   }

   [Fact]
   public async Task Should_generate_record_with_generic()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_conversion_for_non_unique_ctor_arguments()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;

               public partial record Failure2(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_conversion_for_non_unique_non_primary_ctor_arguments()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>
               {
                  public Success(string value) : this(default!)
                  {
                  }
               }

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs");
   }

   [Fact]
   public async Task Should_ignore_private_ctors_when_generating_implicit_conversions()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>
               {
                  private Success(string value) : this(default!)
                  {
                  }
               }

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_conversion_for_non_unique_generic_ctor_arguments()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Success2(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs");
   }

   [Fact]
   public async Task Should_generate_record_with_generic_without_implicit_conversion()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union(SkipImplicitConversionFromValue = true)]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_base_union_has_required_property()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public required string Property { get; set; }

               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_base_union_has_required_field()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public required string Field;

               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_derived_union_has_required_property()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>
               {
                  public required string Property { get; set; }
               }

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_derived_has_required_field()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>
               {
                  public required string Field;
               }

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs");
   }

   [Fact]
   public async Task Should_generate_record_with_multiple_implicit_conversions()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>
               {
                  public Failure(int error) : this(error.ToString())
                  {
                  }
               };
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs");
   }

   [Fact]
   public async Task Should_generate_record_with_and_without_generic()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }

            [Union]
            public partial record Result
            {
               public partial record Success : Result;

               public partial record Failure(string Error) : Result;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.g.cs",
                        "Thinktecture.Tests.Result.g.cs");
   }

   [Fact]
   public async Task Should_generate_record_with_non_default_ctor()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record TestUnion
            {
               public string Name { get; }

               private TestUnion(string name)
               {
                  Name = name;
               }

               public sealed record Child1(string Name) : TestUnion(Name);

               public sealed record Child2(string Name) : TestUnion(Name);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_without_ctor()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion;

               public sealed class Child2 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_classes_having_same_name()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion;

               public sealed class Child2 : TestUnion
               {
                  public sealed class Child1 : TestUnion;
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_special_chars()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class _1TestUnionWithSpecialChars
            {
               public sealed class _1Test : _1TestUnionWithSpecialChars;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<UnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests._1TestUnionWithSpecialChars.g.cs");
   }
}
