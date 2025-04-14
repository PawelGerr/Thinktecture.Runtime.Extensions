using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Thinktecture.Internal;

namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Provider for creation of model binders for Smart Enums and for Value Objects with a key member.
/// </summary>
public sealed class ValueObjectModelBinderProvider : IModelBinderProvider
{
   private readonly bool _skipBindingFromBody;

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectModelBinderProvider"/>.
   /// </summary>
   /// <param name="skipBindingFromBody">Indication whether to skip model binding if the raw value comes from request body.</param>
   public ValueObjectModelBinderProvider(bool skipBindingFromBody = true)
   {
      _skipBindingFromBody = skipBindingFromBody;
   }

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectModelBinderProvider"/>.
   /// </summary>
   /// <param name="trimStringBasedEnums">Indication whether to trim string-values before parsing them.</param>
   /// <param name="skipBindingFromBody">Indication whether to skip model binding if the raw value comes from request body.</param>
   [Obsolete("Use constructor without 'trimStringBasedEnums' parameter instead")]
   public ValueObjectModelBinderProvider(
      bool trimStringBasedEnums,
      bool skipBindingFromBody = true)
      : this(skipBindingFromBody)
   {
   }

   /// <inheritdoc />
   public IModelBinder? GetBinder(ModelBinderProviderContext context)
   {
      ArgumentNullException.ThrowIfNull(context);

      // Skip model binding from body so BodyModelBinder incl. JSON serialization takes over
      if (SkipModelBinding(context))
         return null;

      // ModelType could be derived type (like nested Smart Enum)
      var metadata = KeyedValueObjectMetadataLookup.Find(context.Metadata.ModelType);

      var type = metadata?.Type ?? context.Metadata.ModelType;
      Type keyType;

      if (typeof(IValueObjectFactory<string>).IsAssignableFrom(type))
      {
         keyType = typeof(string);
      }
      else if (metadata is not null)
      {
         keyType = metadata.KeyType;
      }
      else
      {
         return null;
      }

      var validationErrorType = type.GetCustomAttribute<ValueObjectValidationErrorAttribute>()?.Type ?? typeof(ValidationError);

      var modelBinderType = typeof(ValueObjectModelBinder<,,>).MakeGenericType(type, keyType, validationErrorType);
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
