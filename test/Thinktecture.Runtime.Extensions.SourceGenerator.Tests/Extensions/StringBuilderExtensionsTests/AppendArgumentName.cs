using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendArgumentName
{
   private static string Render(string name, bool renderAsIs = false)
   {
      var sb = new StringBuilder();
      sb.AppendArgumentName(ArgumentName.Create(name, renderAsIs));
      return sb.ToString();
   }

   [Theory]
   [InlineData("", "")]
   [InlineData("value", "value")]
   [InlineData("Value", "Value")]
   [InlineData("_Value", "_Value")]
   [InlineData("URL", "URL")]
   public void Should_render_as_is_when_flag_true(string input, string expected)
   {
      Render(input, renderAsIs: true).Should().Be(expected);
   }

   [Fact]
   public void Should_handle_empty_string()
   {
      Render("").Should().Be("");
   }

   [Theory]
   [InlineData("A", "a")]
   [InlineData("a", "a")]
   [InlineData("_", "_")]
   [InlineData("1", "1")]
   [InlineData("Ä", "ä")]
   [InlineData("ä", "ä")]
   public void Should_handle_single_character_inputs(string input, string expected)
   {
      Render(input).Should().Be(expected);
   }

   [Theory]
   [InlineData("Name", "name")]
   [InlineData("NamE", "namE")]
   [InlineData("NaME", "naME")]
   [InlineData("MyName", "myName")]
   [InlineData("URL", "url")]
   [InlineData("ID", "id")]
   [InlineData("A1", "a1")]
   [InlineData("My1Name", "my1Name")]
   [InlineData("URLValue", "urlValue")]
   [InlineData("IPAddress", "ipAddress")]
   [InlineData("ICode", "iCode")]
   public void Should_camel_case_pascal_or_allcaps_inputs(string input, string expected)
   {
      Render(input).Should().Be(expected);
   }

   [Theory]
   [InlineData("my", "my")]
   [InlineData("myName", "myName")]
   [InlineData("camelCase", "camelCase")]
   [InlineData("my_name", "my_name")] // underscores inside are kept
   public void Should_keep_already_camel_case_inputs(string input, string expected)
   {
      Render(input).Should().Be(expected);
   }

   [Theory]
   [InlineData("_name", "name")]
   [InlineData("__name", "name")]
   [InlineData("__", "__")]
   [InlineData("___", "___")]
   public void Should_handle_leading_underscores(string input, string expected)
   {
      Render(input).Should().Be(expected);
   }

   [Theory]
   [InlineData("_Name", "name")]
   [InlineData("_AValue", "aValue")]
   [InlineData("$Name", "$name")]
   public void Should_handle_non_letter_prefixes_and_leading_underscore(string input, string expected)
   {
      Render(input).Should().Be(expected);
   }

   [Theory]
   [InlineData("1Value", "1value")]
   [InlineData("9name", "9name")]
   public void Should_handle_leading_digit(string input, string expected)
   {
      Render(input).Should().Be(expected);
   }
}
