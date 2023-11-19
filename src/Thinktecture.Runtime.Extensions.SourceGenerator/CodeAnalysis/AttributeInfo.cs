namespace Thinktecture.CodeAnalysis;

public readonly struct AttributeInfo : IEquatable<AttributeInfo>
{
   public bool HasStructLayoutAttribute { get; }
   public bool HasJsonConverterAttribute { get; }
   public bool HasNewtonsoftJsonConverterAttribute { get; }
   public bool HasMessagePackFormatterAttribute { get; }
   public ImmutableArray<DesiredFactory> DesiredFactories { get; }
   public ValidationErrorState ValidationError { get; }

   public AttributeInfo(
      bool hasStructLayoutAttribute,
      bool hasJsonConverterAttribute,
      bool hasNewtonsoftJsonConverterAttribute,
      bool hasMessagePackFormatterAttribute,
      ImmutableArray<DesiredFactory> desiredFactories,
      ValidationErrorState validationError)
   {
      HasStructLayoutAttribute = hasStructLayoutAttribute;
      HasJsonConverterAttribute = hasJsonConverterAttribute;
      HasNewtonsoftJsonConverterAttribute = hasNewtonsoftJsonConverterAttribute;
      HasMessagePackFormatterAttribute = hasMessagePackFormatterAttribute;
      DesiredFactories = desiredFactories;
      ValidationError = validationError;
   }

   public static string? TryCreate(INamedTypeSymbol type, out AttributeInfo info)
   {
      var hasStructLayoutAttribute = false;
      var hasJsonConverterAttribute = false;
      var hasNewtonsoftJsonConverterAttribute = false;
      var hasMessagePackFormatterAttribute = false;
      var validationError = ValidationErrorState.Default;
      var valueObjectFactories = ImmutableArray<DesiredFactory>.Empty;
      var numberOfSourceGenAttributes = 0;

      foreach (var attribute in type.GetAttributes())
      {
         if (attribute.AttributeClass is not { } attributeClass || attributeClass.TypeKind == TypeKind.Error)
            continue;

         if (attribute.AttributeClass.IsStructLayoutAttribute())
         {
            hasStructLayoutAttribute = true;
         }
         else if (attribute.AttributeClass.IsJsonConverterAttribute())
         {
            hasJsonConverterAttribute = true;
         }
         else if (attribute.AttributeClass.IsNewtonsoftJsonConverterAttribute())
         {
            hasNewtonsoftJsonConverterAttribute = true;
         }
         else if (attribute.AttributeClass.IsMessagePackFormatterAttribute())
         {
            hasMessagePackFormatterAttribute = true;
         }
         else if (attribute.AttributeClass.IsValueObjectFactoryAttribute())
         {
            var useForSerialization = attribute.FindUseForSerialization();
            var desiredFactory = new DesiredFactory(attribute.AttributeClass.TypeArguments[0], useForSerialization);

            valueObjectFactories = valueObjectFactories.RemoveAll(static (f, fullTypeName) => f.TypeFullyQualified == fullTypeName, desiredFactory.TypeFullyQualified);
            valueObjectFactories = valueObjectFactories.Add(desiredFactory);
         }
         else if (attribute.AttributeClass.IsValueObjectValidationErrorAttribute())
         {
            validationError = new ValidationErrorState(attribute.AttributeClass.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
         }
         else if (attribute.AttributeClass.IsSmartEnumAttribute())
         {
            ++numberOfSourceGenAttributes;
         }
         else if (attribute.AttributeClass.IsValueObjectAttribute())
         {
            ++numberOfSourceGenAttributes;
         }

         if (numberOfSourceGenAttributes > 1)
         {
            info = default;
            return "Multiple ValueObject/SmartEnum-attributes found";
         }
      }

      info = new AttributeInfo(hasStructLayoutAttribute,
                               hasJsonConverterAttribute,
                               hasNewtonsoftJsonConverterAttribute,
                               hasMessagePackFormatterAttribute,
                               valueObjectFactories,
                               validationError);
      return null;
   }

   public override bool Equals(object? obj)
   {
      return obj is AttributeInfo other && Equals(other);
   }

   public bool Equals(AttributeInfo other)
   {
      return HasStructLayoutAttribute == other.HasStructLayoutAttribute
             && HasJsonConverterAttribute == other.HasJsonConverterAttribute
             && HasNewtonsoftJsonConverterAttribute == other.HasNewtonsoftJsonConverterAttribute
             && HasMessagePackFormatterAttribute == other.HasMessagePackFormatterAttribute
             && DesiredFactories.SequenceEqual(other.DesiredFactories)
             && ValidationError.Equals(other.ValidationError);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = HasStructLayoutAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ HasJsonConverterAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ HasNewtonsoftJsonConverterAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ HasMessagePackFormatterAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ DesiredFactories.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();

         return hashCode;
      }
   }
}
