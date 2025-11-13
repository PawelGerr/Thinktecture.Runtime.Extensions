using System.Text;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class GenerateStructLayoutAttributeIfRequired
{
   [Fact]
   public void Should_append_for_value_type_without_existing_attribute()
   {
      var sb = new StringBuilder();
      sb.GenerateStructLayoutAttributeIfRequired(isReferenceType: false, hasStructLayoutAttribute: false);
      sb.ToString().Should().Be(@"
   [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]");
   }

   [Theory]
   [InlineData(true, false)]
   [InlineData(false, true)]
   [InlineData(true, true)]
   public void Should_not_append_for_reference_type_or_when_attribute_exists(bool isReferenceType, bool hasAttribute)
   {
      var sb = new StringBuilder();
      sb.GenerateStructLayoutAttributeIfRequired(isReferenceType, hasAttribute);
      sb.ToString().Should().BeEmpty();
   }
}
