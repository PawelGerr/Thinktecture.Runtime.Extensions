using System.Text;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendConversionOperator
{
   [Fact]
   public void None_should_not_change_builder()
   {
      var sb = new StringBuilder();
      sb.AppendConversionOperator(Thinktecture.CodeAnalysis.ConversionOperatorsGeneration.None);
      sb.ToString().Should().BeEmpty();
   }

   [Fact]
   public void Implicit_should_append_keyword()
   {
      var sb = new StringBuilder();
      sb.AppendConversionOperator(Thinktecture.CodeAnalysis.ConversionOperatorsGeneration.Implicit);
      sb.ToString().Should().Be("implicit");
   }

   [Fact]
   public void Explicit_should_append_keyword()
   {
      var sb = new StringBuilder();
      sb.AppendConversionOperator(Thinktecture.CodeAnalysis.ConversionOperatorsGeneration.Explicit);
      sb.ToString().Should().Be("explicit");
   }
}
