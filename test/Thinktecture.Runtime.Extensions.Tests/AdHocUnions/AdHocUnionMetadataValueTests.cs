#nullable enable
using System;
using System.Linq.Expressions;
using Thinktecture.Internal;

namespace Thinktecture.Runtime.Tests.AdHocUnions;

// Local test unions so the test is self-contained and independent of shared-type churn.
[Union<int, string>]
public partial class MetadataIntOrString;

[Union<int, string>]
public partial struct MetadataIntOrStringStruct;

public class AdHocUnionMetadataValueTests
{
   private static Metadata.AdHocUnion GetMetadata(Type type)
      => (Metadata.AdHocUnion)MetadataLookup.Find(type)!;

   [Fact]
   public void Should_expose_GetValue_returning_boxed_current_value()
   {
      var metadata = GetMetadata(typeof(MetadataIntOrString));
      MetadataIntOrString union = 42;

      metadata.GetValue(union).Should().Be(42);
   }

   [Fact]
   public void Should_expose_typed_ConvertToValue_delegate()
   {
      var metadata = GetMetadata(typeof(MetadataIntOrString));
      MetadataIntOrString union = "abc";

      metadata.ConvertToValue.Should().BeOfType<Func<MetadataIntOrString, object?>>();
      ((Func<MetadataIntOrString, object?>)metadata.ConvertToValue)(union).Should().Be("abc");
   }

   [Fact]
   public void Should_expose_ConvertToValueExpression()
   {
      var metadata = GetMetadata(typeof(MetadataIntOrString));

      metadata.ConvertToValueExpression.Should().BeAssignableTo<LambdaExpression>();
      metadata.ConvertToValueExpression.Parameters.Should().ContainSingle()
              .Which.Type.Should().Be(typeof(MetadataIntOrString));
   }

   [Fact]
   public void Should_throw_when_GetValue_called_on_default_struct_union()
   {
      var metadata = GetMetadata(typeof(MetadataIntOrStringStruct));

      var act = () => metadata.GetValue(default(MetadataIntOrStringStruct));

      act.Should().Throw<InvalidOperationException>();
   }
}
