using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Thinktecture.Internal;

namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Provider for creation of model binders implementing <see cref="IEnum{TKey}"/> or for Value Objects with a key member.
/// </summary>
public sealed class ValueObjectModelBinderProvider : IModelBinderProvider
{
   private readonly bool _trimStringBasedEnums;

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectModelBinderProvider"/>.
   /// </summary>
   /// <param name="trimStringBasedEnums">Indication whether to trim string-values before parsing them.</param>
   public ValueObjectModelBinderProvider(bool trimStringBasedEnums = true)
   {
      _trimStringBasedEnums = trimStringBasedEnums;
   }

   /// <inheritdoc />
   public IModelBinder? GetBinder(ModelBinderProviderContext context)
   {
      ArgumentNullException.ThrowIfNull(context);

      // Skip model binding from body so BodyModelBinder incl. JSON serialization takes over
      if (SkipModelBinding(context))
         return null;

      var metadata = KeyedValueObjectMetadataLookup.Find(context.Metadata.ModelType);

      if (metadata is null)
         return null;

      var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
      var modelBinderType = _trimStringBasedEnums && metadata.IsEnumeration && metadata.KeyType == typeof(string)
                               ? typeof(TrimmingSmartEnumModelBinder<>).MakeGenericType(metadata.Type)
                               : typeof(ValueObjectModelBinder<,>).MakeGenericType(metadata.Type, metadata.KeyType);
      var modelBinder = Activator.CreateInstance(modelBinderType, loggerFactory)
                        ?? throw new Exception($"Could not create an instance of type '{modelBinderType.Name}'.");

      return (IModelBinder)modelBinder;
   }

   private static bool SkipModelBinding(ModelBinderProviderContext context)
   {
      return context.BindingInfo.BindingSource != null &&
             (context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Body) || context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Services));
   }
}
