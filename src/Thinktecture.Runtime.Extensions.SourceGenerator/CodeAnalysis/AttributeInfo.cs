namespace Thinktecture.CodeAnalysis;

public readonly struct AttributeInfo : IEquatable<AttributeInfo>
{
   public bool HasStructLayoutAttribute { get; }
   public bool HasJsonConverterAttribute { get; }
   public bool HasNewtonsoftJsonConverterAttribute { get; }
   public bool HasMessagePackFormatterAttribute { get; }
   public ImmutableArray<DesiredFactory> DesiredFactories { get; }

   public AttributeInfo(INamedTypeSymbol type)
   {
      HasStructLayoutAttribute = default;
      HasJsonConverterAttribute = default;
      HasNewtonsoftJsonConverterAttribute = default;
      HasMessagePackFormatterAttribute = default;
      var valueObjectFactories = ImmutableArray<DesiredFactory>.Empty;

      foreach (var attribute in type.GetAttributes())
      {
         if (attribute.AttributeClass is not { } attributeClass || attributeClass.TypeKind == TypeKind.Error)
            continue;

         if (attribute.AttributeClass.IsStructLayoutAttribute())
         {
            HasStructLayoutAttribute = true;
         }
         else if (attribute.AttributeClass.IsJsonConverterAttribute())
         {
            HasJsonConverterAttribute = true;
         }
         else if (attribute.AttributeClass.IsNewtonsoftJsonConverterAttribute())
         {
            HasNewtonsoftJsonConverterAttribute = true;
         }
         else if (attribute.AttributeClass.IsMessagePackFormatterAttribute())
         {
            HasMessagePackFormatterAttribute = true;
         }
         else if (attribute.AttributeClass.IsValueObjectFactoryAttribute())
         {
            var useForSerialization = attribute.FindUseForSerialization();
            var desiredFactory = new DesiredFactory(attribute.AttributeClass.TypeArguments[0], useForSerialization);

            valueObjectFactories = valueObjectFactories.RemoveAll(static (f, fullTypeName) => f.TypeFullyQualified == fullTypeName, desiredFactory.TypeFullyQualified);
            valueObjectFactories = valueObjectFactories.Add(desiredFactory);
         }
      }

      DesiredFactories = valueObjectFactories;
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
             && DesiredFactories.SequenceEqual(other.DesiredFactories);
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

         return hashCode;
      }
   }
}
