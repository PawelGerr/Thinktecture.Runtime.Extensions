using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Moq;

namespace Thinktecture.AspNetCore.ModelBinding
{
   public class BindingContextHelper
   {
      public static DefaultModelMetadata CreateModelMetadata<T>()
      {
         var metadataDetailProvider = new Mock<ICompositeMetadataDetailsProvider>().Object;
         var modelMetadataProvider = new DefaultModelMetadataProvider(metadataDetailProvider);
         var details = new DefaultMetadataDetails(ModelMetadataIdentity.ForType(typeof(T)), ModelAttributes.GetAttributesForType(typeof(T)));

         return new DefaultModelMetadata(modelMetadataProvider, metadataDetailProvider, details);
      }
   }
}