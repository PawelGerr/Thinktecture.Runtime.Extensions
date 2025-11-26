using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.Annotations;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class AnnotationsSourceGeneratorTests : SourceGeneratorTestsBase
{
   public AnnotationsSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 10_000)
   {
   }

   [Fact]
   public async Task Should_generate_InstantHandleAttribute_when_no_local_attribute_and_no_package_reference()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            public class TestClass
            {
               public void TestMethod()
               {
               }
            }
         }
         """;

      var output = GetGeneratedOutput<AnnotationsSourceGenerator>(source);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_local_InstantHandleAttribute_exists()
   {
      var source = """
         using System;

         namespace JetBrains.Annotations
         {
            [AttributeUsage(AttributeTargets.Parameter)]
            internal sealed class InstantHandleAttribute : Attribute
            {
               public bool RequireAwait { get; set; }
            }
         }

         namespace Thinktecture.Tests
         {
            public class TestClass
            {
               public void TestMethod([JetBrains.Annotations.InstantHandle] Action action)
               {
                  action();
               }
            }
         }
         """;

      var outputs = GetGeneratedOutputs<AnnotationsSourceGenerator>(source);
      outputs.Should().BeEmpty();
   }

   [Fact]
   public void Should_not_generate_when_local_InstantHandleAttribute_exists_with_different_accessibility()
   {
      var source = """
         using System;

         namespace JetBrains.Annotations
         {
            [AttributeUsage(AttributeTargets.Parameter)]
            public sealed class InstantHandleAttribute : Attribute
            {
               public bool RequireAwait { get; set; }
            }
         }

         namespace Thinktecture.Tests
         {
            public class TestClass
            {
               public void TestMethod([JetBrains.Annotations.InstantHandle] Action action)
               {
                  action();
               }
            }
         }
         """;

      var outputs = GetGeneratedOutputs<AnnotationsSourceGenerator>(source);
      outputs.Should().BeEmpty();
   }

   [Fact]
   public void Should_generate_when_InstantHandleAttribute_in_wrong_namespace()
   {
      var source = """
         using System;

         namespace WrongNamespace.Annotations
         {
            [AttributeUsage(AttributeTargets.Parameter)]
            internal sealed class InstantHandleAttribute : Attribute
            {
               public bool RequireAwait { get; set; }
            }
         }

         namespace Thinktecture.Tests
         {
            public class TestClass
            {
               public void TestMethod()
               {
               }
            }
         }
         """;

      var output = GetGeneratedOutput<AnnotationsSourceGenerator>(source);
      output.Should().NotBeNullOrWhiteSpace();
      output.Should().Contain("namespace JetBrains.Annotations");
      output.Should().Contain("internal sealed class InstantHandleAttribute");
   }

   [Fact]
   public void Should_generate_when_InstantHandleAttribute_in_nested_wrong_namespace()
   {
      var source = """
         using System;

         namespace JetBrains.WrongAnnotations
         {
            [AttributeUsage(AttributeTargets.Parameter)]
            internal sealed class InstantHandleAttribute : Attribute
            {
               public bool RequireAwait { get; set; }
            }
         }

         namespace Thinktecture.Tests
         {
            public class TestClass
            {
               public void TestMethod()
               {
               }
            }
         }
         """;

      var output = GetGeneratedOutput<AnnotationsSourceGenerator>(source);
      output.Should().NotBeNullOrWhiteSpace();
      output.Should().Contain("namespace JetBrains.Annotations");
      output.Should().Contain("internal sealed class InstantHandleAttribute");
   }

   [Fact]
   public void Should_generate_when_InstantHandleAttribute_in_annotations_namespace_but_wrong_parent()
   {
      var source = """
         using System;

         namespace WrongJetBrains.Annotations
         {
            [AttributeUsage(AttributeTargets.Parameter)]
            internal sealed class InstantHandleAttribute : Attribute
            {
               public bool RequireAwait { get; set; }
            }
         }

         namespace Thinktecture.Tests
         {
            public class TestClass
            {
               public void TestMethod()
               {
               }
            }
         }
         """;

      var output = GetGeneratedOutput<AnnotationsSourceGenerator>(source);
      output.Should().NotBeNullOrWhiteSpace();
      output.Should().Contain("namespace JetBrains.Annotations");
      output.Should().Contain("internal sealed class InstantHandleAttribute");
   }

   [Fact]
   public void Should_generate_when_InstantHandleAttribute_with_different_name()
   {
      var source = """
         using System;

         namespace JetBrains.Annotations
         {
            [AttributeUsage(AttributeTargets.Parameter)]
            internal sealed class InstantHandleAttributeWrong : Attribute
            {
               public bool RequireAwait { get; set; }
            }
         }

         namespace Thinktecture.Tests
         {
            public class TestClass
            {
               public void TestMethod()
               {
               }
            }
         }
         """;

      var output = GetGeneratedOutput<AnnotationsSourceGenerator>(source);
      output.Should().NotBeNullOrWhiteSpace();
      output.Should().Contain("namespace JetBrains.Annotations");
      output.Should().Contain("internal sealed class InstantHandleAttribute");
   }

   [Fact]
   public async Task Should_generate_with_correct_namespace_structure()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            public class TestClass
            {
               public void TestMethod()
               {
               }
            }
         }
         """;

      var output = GetGeneratedOutput<AnnotationsSourceGenerator>(source);

      output.Should().Contain("namespace JetBrains.Annotations");
      output.Should().Contain("internal sealed class InstantHandleAttribute : Attribute");
      output.Should().Contain("public bool RequireAwait { get; set; }");
      output.Should().Contain("[AttributeUsage(AttributeTargets.Parameter)]");

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_with_MIT_license_header()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            public class TestClass
            {
               public void TestMethod()
               {
               }
            }
         }
         """;

      var output = GetGeneratedOutput<AnnotationsSourceGenerator>(source);

      output.Should().Contain("MIT License");
      output.Should().Contain("Copyright (c) 2025 JetBrains http://www.jetbrains.com");
      output.Should().Contain("THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND");

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_generate_when_namespace_is_nested_in_different_parent_namespace()
   {
      var source = """
         using System;

         namespace Some.Other.JetBrains.Annotations
         {
            [AttributeUsage(AttributeTargets.Parameter)]
            internal sealed class InstantHandleAttribute : Attribute
            {
               public bool RequireAwait { get; set; }
            }
         }

         namespace Thinktecture.Tests
         {
            public class TestClass
            {
               public void TestMethod()
               {
               }
            }
         }
         """;

      var output = GetGeneratedOutput<AnnotationsSourceGenerator>(source);
      output.Should().NotBeNullOrWhiteSpace();
      output.Should().Contain("namespace JetBrains.Annotations");
      output.Should().Contain("internal sealed class InstantHandleAttribute");
   }

   [Fact]
   public async Task Should_generate_when_empty_source_file()
   {
      var source = "";

      var output = GetGeneratedOutput<AnnotationsSourceGenerator>(source, [], ["CS1733"]);

      output.Should().NotBeNullOrWhiteSpace();
      output.Should().Contain("namespace JetBrains.Annotations");
      output.Should().Contain("internal sealed class InstantHandleAttribute");

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_generate_when_global_namespace_is_not_parent()
   {
      var source = """
         using System;

         namespace OuterNamespace
         {
            namespace JetBrains.Annotations
            {
               [AttributeUsage(AttributeTargets.Parameter)]
               internal sealed class InstantHandleAttribute : Attribute
               {
                  public bool RequireAwait { get; set; }
               }
            }
         }

         namespace Thinktecture.Tests
         {
            public class TestClass
            {
               public void TestMethod()
               {
               }
            }
         }
         """;

      var output = GetGeneratedOutput<AnnotationsSourceGenerator>(source);
      output.Should().NotBeNullOrWhiteSpace();
      output.Should().Contain("namespace JetBrains.Annotations");
      output.Should().Contain("internal sealed class InstantHandleAttribute");
   }
}
