using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Thinktecture.Internal;

namespace Thinktecture.AspNetCore.ModelBinding
{
   /// <summary>
   /// Provider for creation of model binders implementing <see cref="IEnum{TKey}"/> or for value types with a key member.
   /// </summary>
   public class ValueTypeModelBinderProvider : IModelBinderProvider
   {
      /// <inheritdoc />
      public IModelBinder? GetBinder(ModelBinderProviderContext context)
      {
         if (context is null)
            throw new ArgumentNullException(nameof(context));

         var metadata = ValueTypeMetadataLookup.Find(context.Metadata.ModelType);

         if (metadata is null)
            return null;

         var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
         var modelBinderType = typeof(ValueTypeModelBinder<,>).MakeGenericType(metadata.Type, metadata.KeyType);
         var modelBinder = Activator.CreateInstance(modelBinderType, loggerFactory, metadata.Validate) ?? throw new Exception($"Could not create an instance of type '{modelBinderType.Name}'.");

         return (IModelBinder)modelBinder;
      }
   }
}
