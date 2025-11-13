namespace Thinktecture.Runtime.Tests.AssemblySymbolExtensionsTests;

public class HasInternalsVisibleToFor : CompilationTestBase
{
   [Fact]
   public void Returns_true_when_assembly_has_matching_internals_visible_to_attribute()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TargetAssembly"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_assembly_has_internals_visible_to_with_public_key()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TargetAssembly, PublicKey=123"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_when_assembly_has_internals_visible_to_but_different_target()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""OtherAssembly"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_when_assembly_has_no_internals_visible_to_attributes()
   {
      var src = @"
namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_with_case_insensitive_matching()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""targetassembly"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_assembly_name_has_leading_whitespace()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""  TargetAssembly"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_assembly_name_has_trailing_whitespace()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TargetAssembly  "")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_assembly_name_has_whitespace_before_comma()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TargetAssembly  , PublicKey=123"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_multiple_internals_visible_to_attributes_and_one_matches()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""FirstAssembly"")]
[assembly: InternalsVisibleTo(""TargetAssembly"")]
[assembly: InternalsVisibleTo(""ThirdAssembly"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_when_multiple_internals_visible_to_attributes_but_none_match()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""FirstAssembly"")]
[assembly: InternalsVisibleTo(""SecondAssembly"")]
[assembly: InternalsVisibleTo(""ThirdAssembly"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_when_assembly_name_exactly_matches_without_public_key()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TargetAssembly"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_when_target_assembly_name_is_only_partial_match()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""Target"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_when_attribute_is_not_internals_visible_to()
   {
      var src = @"
using System;

[assembly: CLSCompliant(true)]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_when_matching_with_mixed_casing()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TaRgEtAsSeMbLy"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_assembly_name_has_both_leading_and_trailing_whitespace()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""  TargetAssembly  "")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_when_assembly_name_is_substring_of_target()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TargetAssemblyExtra"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_when_public_key_has_additional_properties()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TargetAssembly, PublicKey=123, Version=1.0.0.0"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_when_comparing_with_itself_but_different_name_in_attribute()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""OtherAssembly"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(compilation.Assembly);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_when_comparing_with_itself_and_same_name_in_attribute()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""SourceAssembly"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(compilation.Assembly);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_when_assembly_name_is_empty_string()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("""")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_when_internals_visible_to_has_only_whitespace()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""   "")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_when_assembly_has_internals_visible_to_with_public_key_token()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TargetAssembly, PublicKeyToken=123"")]

namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src, "SourceAssembly");
      var targetCompilation = CreateCompilation("namespace Target; public class Target;", "TargetAssembly");

      var result = compilation.Assembly.HasInternalsVisibleToFor(targetCompilation.Assembly);

      result.Should().BeTrue();
   }
}
