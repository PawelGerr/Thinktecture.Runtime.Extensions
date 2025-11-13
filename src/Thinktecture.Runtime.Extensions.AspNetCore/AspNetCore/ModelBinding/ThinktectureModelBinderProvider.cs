using Microsoft.AspNetCore.Mvc.ModelBinding;
using Thinktecture.Internal;

namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Provider for creation of model binders for Smart Enums and for Value Objects with a key member.
/// </summary>
public class ThinktectureModelBinderProvider : IModelBinderProvider
{
   private readonly bool _skipBindingFromBody;

   /// <summary>
   /// Initializes new instance of <see cref="ThinktectureModelBinderProvider"/>.
   /// </summary>
   /// <param name="skipBindingFromBody">Indication whether to skip model binding if the raw value comes from request body.</param>
   public ThinktectureModelBinderProvider(bool skipBindingFromBody = true)
   {
      _skipBindingFromBody = skipBindingFromBody;
   }

   /// <inheritdoc />
   public IModelBinder? GetBinder(ModelBinderProviderContext context)
   {
      ArgumentNullException.ThrowIfNull(context);

      // Skip model binding from body so BodyModelBinder incl. JSON serialization takes over
      if (SkipModelBinding(context))
         return null;

      // ModelType could be a derived type (like nested Smart Enum)
      var metadata = MetadataLookup.FindMetadataForConversion(
         context.Metadata.ModelType,
         f => f.UseForModelBinding,
         _ => true);

      if (metadata is null)
         return null;

      var modelBinderType = typeof(ThinktectureModelBinder<,,>).MakeGenericType(metadata.Value.Type, metadata.Value.KeyType, metadata.Value.ValidationErrorType);
      var modelBinder = Activator.CreateInstance(modelBinderType)
                        ?? throw new Exception($"Could not create an instance of type '{modelBinderType.Name}'.");

      return (IModelBinder)modelBinder;
   }

   private bool SkipModelBinding(ModelBinderProviderContext context)
   {
      if (context.BindingInfo.BindingSource == null)
         return false;

      if (context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Services))
         return true;

      return _skipBindingFromBody && context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Body);
   }
}
