using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendBackingFieldName
{
   private static string Render(string name, string propertyName)
   {
      var sb = new StringBuilder();
      sb.AppendBackingFieldName(BackingFieldName.Create(name, propertyName));
      return sb.ToString();
   }

   [Fact]
   public void Should_handle_empty_string_without_collision()
   {
      Render("", "Prop").Should().Be("");
   }

   [Theory]
   [InlineData("", "", "_")]           // collision: result "" equals propertyName ""
   [InlineData("_", "_", "__")]        // collision: result "_" equals propertyName "_"
   [InlineData("_name", "_name", "__name")] // collision after render
   [InlineData("Name", "_name", "__name")]  // typical collision case
   public void Should_prefix_when_result_equals_property_name(string input, string propertyName, string expected)
   {
      Render(input, propertyName).Should().Be(expected);
   }

   [Theory]
   [InlineData("A", "_a")]
   [InlineData("a", "_a")]
   [InlineData("_", "_")]
   [InlineData("1", "_1")]
   [InlineData("Ä", "_ä")]
   [InlineData("ä", "_ä")]
   public void Should_handle_single_character_inputs(string input, string expected)
   {
      Render(input, "Prop").Should().Be(expected);
   }

   [Theory]
   [InlineData("Name", "_name")]
   [InlineData("NamE", "_namE")]
   [InlineData("NaME", "_naME")]
   [InlineData("MyName", "_myName")]
   [InlineData("URL", "_url")]
   [InlineData("ID", "_id")]
   [InlineData("A1", "_a1")]
   [InlineData("My1Name", "_my1Name")]
   [InlineData("URLValue", "_urlValue")]
   [InlineData("IPAddress", "_ipAddress")]
   [InlineData("ICode", "_iCode")]
   public void Should_camel_case_pascal_or_allcaps_inputs(string input, string expected)
   {
      Render(input, "Prop").Should().Be(expected);
   }

   [Theory]
   [InlineData("my", "_my")]
   [InlineData("myName", "_myName")]
   [InlineData("camelCase", "_camelCase")]
   [InlineData("my_name", "_my_name")] // underscores inside are kept
   public void Should_keep_already_camel_case_inputs(string input, string expected)
   {
      Render(input, "Prop").Should().Be(expected);
   }

   [Theory]
   [InlineData("_name", "_name")]
   [InlineData("__name", "__name")]
   [InlineData("__", "__")]
   [InlineData("___", "___")]
   public void Should_handle_leading_underscores(string input, string expected)
   {
      Render(input, "Prop").Should().Be(expected);
   }

   [Theory]
   [InlineData("_Name", "_name")]
   [InlineData("_AValue", "_aValue")]
   [InlineData("$Name", "_$name")]
   public void Should_handle_non_letter_prefixes_and_leading_underscore(string input, string expected)
   {
      Render(input, "Prop").Should().Be(expected);
   }

   [Theory]
   [InlineData("1Value", "_1value")]
   [InlineData("9name", "_9name")]
   public void Should_handle_leading_digit(string input, string expected)
   {
      Render(input, "Prop").Should().Be(expected);
   }
}
