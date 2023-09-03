using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using NSubstitute;

namespace Thinktecture.Runtime.Tests.AspNetCore.ModelBinding;

public class BindingContextHelper
{
   public static DefaultModelMetadata CreateModelMetadata<T>()
   {
      var metadataDetailProvider = Substitute.For<ICompositeMetadataDetailsProvider>();
      var modelMetadataProvider = new DefaultModelMetadataProvider(metadataDetailProvider);
      var details = new DefaultMetadataDetails(ModelMetadataIdentity.ForType(typeof(T)), ModelAttributes.GetAttributesForType(typeof(T)));

      return new DefaultModelMetadata(modelMetadataProvider, metadataDetailProvider, details);
   }
}
