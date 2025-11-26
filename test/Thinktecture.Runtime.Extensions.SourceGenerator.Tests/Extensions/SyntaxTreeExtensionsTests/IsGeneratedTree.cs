using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

#nullable enable

namespace Thinktecture.Runtime.Tests.SyntaxTreeExtensionsTests;

public class IsGeneratedTree
{
   [Theory]
   [InlineData("File.g.cs")]
   [InlineData("File.G.CS")]
   [InlineData("File.G.cs")]
   [InlineData("path/to/File.g.cs")]
   [InlineData("C:\\Projects\\File.g.cs")]
   [InlineData("my.complex.file.g.cs")]
   [InlineData("File.g.i.cs")]
   [InlineData("File.G.I.CS")]
   [InlineData("File.G.I.cs")]
   [InlineData("path/to/File.g.i.cs")]
   [InlineData("File.designer.cs")]
   [InlineData("File.DESIGNER.CS")]
   [InlineData("File.Designer.Cs")]
   [InlineData("Form1.designer.cs")]
   [InlineData("File.generated.cs")]
   [InlineData("File.GENERATED.CS")]
   [InlineData("File.Generated.Cs")]
   [InlineData("path/to/File.generated.cs")]
   [InlineData(".g.cs")]
   [InlineData(".g.i.cs")]
   [InlineData(".designer.cs")]
   [InlineData(".generated.cs")]
   public void Returns_true_for_generated_file_paths(string filePath)
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: filePath, cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeTrue($"because '{filePath}' is a generated file path");
   }

   [Theory]
   [InlineData("File.cs")]
   [InlineData("File.txt")]
   [InlineData("File.g.cs.txt")]
   [InlineData("File.g.cs.bak")]
   [InlineData("Fileg.cs")]
   [InlineData("File.gcs")]
   [InlineData("File.designer")]
   [InlineData("File.generated")]
   [InlineData("g.cs")]
   [InlineData("designer.cs")]
   [InlineData("generated.cs")]
   [InlineData("path/to/File.cs")]
   [InlineData("C:\\Projects\\MyClass.cs")]
   [InlineData("MyClass.vb")]
   [InlineData("File.g.i.cs.old")]
   [InlineData("path.g.cs/File.cs")]
   [InlineData("directory.designer.cs/MyClass.cs")]
   public void Returns_false_for_non_generated_file_paths(string filePath)
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: filePath, cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeFalse($"because '{filePath}' is not a generated file path");
   }

   [Fact]
   public void Returns_false_for_null_file_path()
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: null!, cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeFalse("because null file path is not a generated file");
   }

   [Fact]
   public void Returns_false_for_empty_file_path()
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: "", cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeFalse("because empty file path is not a generated file");
   }

   [Theory]
   [InlineData(" ")]
   [InlineData("   ")]
   [InlineData("\t")]
   [InlineData("\n")]
   [InlineData("\r\n")]
   public void Returns_false_for_whitespace_only_file_path(string filePath)
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: filePath, cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeFalse($"because whitespace-only file path '{filePath.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t")}' is not a generated file");
   }

   [Fact]
   public void Returns_true_for_file_path_with_multiple_periods_ending_in_generated_extension()
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: "My.Complex.File.Name.g.cs", cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeTrue("because the file ends with .g.cs extension");
   }

   [Fact]
   public void Returns_false_when_directory_name_contains_generated_extension_but_file_does_not()
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: "C:\\folder.g.cs\\MyClass.cs", cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeFalse("because only the file name, not directory name, should be checked");
   }

   [Fact]
   public void Returns_true_for_very_long_file_path_with_generated_extension()
   {
      // Arrange
      var longPath = new string('a', 500) + ".g.cs";
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: longPath, cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeTrue("because the file ends with .g.cs extension regardless of path length");
   }

   [Fact]
   public void Returns_true_with_unix_style_paths()
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: "/usr/local/src/File.g.cs", cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeTrue("because Unix-style paths with .g.cs extension should be recognized");
   }

   [Fact]
   public void Returns_true_with_windows_style_paths()
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: "C:\\Users\\Developer\\Source\\File.designer.cs", cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeTrue("because Windows-style paths with .designer.cs extension should be recognized");
   }

   [Theory]
   [InlineData("File.g.cs.")]
   [InlineData(".g..cs")]
   public void Returns_false_for_malformed_extensions(string filePath)
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: filePath, cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeFalse($"because '{filePath}' has a malformed extension");
   }

   [Fact]
   public void Returns_true_for_file_with_double_dot_before_extension()
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: "File..g.cs", cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeTrue("because the file path ends with .g.cs, regardless of preceding dots");
   }

   [Fact]
   public void Works_with_cancelled_cancellation_token()
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: "File.g.cs", cancellationToken: TestContext.Current.CancellationToken);
      var cancellationTokenSource = new CancellationTokenSource();
      cancellationTokenSource.Cancel();

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationTokenSource.Token);

      // Assert
      // The method doesn't actually use the cancellation token, so it should still work
      result.Should().BeTrue("because the CancellationToken parameter is not used in the implementation");
   }

   [Fact]
   public void Returns_true_for_file_name_with_special_characters()
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: "My File (Special).g.cs", cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeTrue("because special characters in file name should not affect extension checking");
   }

   [Theory]
   [InlineData("File.g.CS")]
   [InlineData("File.G.i.CS")]
   [InlineData("File.DESIGNER.cs")]
   [InlineData("File.generated.CS")]
   public void Case_insensitivity_is_respected(string filePath)
   {
      // Arrange
      var syntaxTree = CSharpSyntaxTree.ParseText("", path: filePath, cancellationToken: TestContext.Current.CancellationToken);
      var cancellationToken = CancellationToken.None;

      // Act
      var result = syntaxTree.IsGeneratedTree(cancellationToken);

      // Assert
      result.Should().BeTrue($"because extension matching should be case-insensitive for '{filePath}'");
   }
}
