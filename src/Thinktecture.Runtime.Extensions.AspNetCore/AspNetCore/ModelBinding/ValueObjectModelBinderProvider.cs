using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Thinktecture.Internal;

namespace Thinktecture.AspNetCore.ModelBinding
{
   /// <summary>
   /// Provider for creation of model binders implementing <see cref="IEnum{TKey}"/> or for value objects with a key member.
   /// </summary>
   public class ValueObjectModelBinderProvider : IModelBinderProvider
   {
      /// <inheritdoc />
      public IModelBinder? GetBinder(ModelBinderProviderContext context)
      {
         if (context is null)
            throw new ArgumentNullException(nameof(context));

         // Skip model binding from body so BodyModelBinder incl. JSON serialization takes over
         if (SkipModelBinding(context))
            return null;

         var metadata = ValueObjectMetadataLookup.Find(context.Metadata.ModelType);

         if (metadata is null)
            return null;

         var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
         var modelBinderType = typeof(ValueObjectModelBinder<,>).MakeGenericType(metadata.Type, metadata.KeyType);
         var modelBinder = Activator.CreateInstance(modelBinderType, loggerFactory, metadata.Validate) ?? throw new Exception($"Could not create an instance of type '{modelBinderType.Name}'.");

         return (IModelBinder)modelBinder;
      }

      private static bool SkipModelBinding(ModelBinderProviderContext context)
      {
         return context.BindingInfo.BindingSource != null &&
                (context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Body) || context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Services));
      }
   }
}
