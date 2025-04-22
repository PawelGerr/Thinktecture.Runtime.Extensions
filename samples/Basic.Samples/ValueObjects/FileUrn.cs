using System;

namespace Thinktecture.ValueObjects;

/// <summary>
/// Represents a file location combining a file store identifier and a store-specific URN.
/// </summary>
[ComplexValueObject(
   ConstructorAccessModifier = AccessModifier.Public,
   DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)] // (de)serialization to/from string
public partial class FileUrn
{
   public string FileStore { get; }
   public string Urn { get; }

   static partial void ValidateFactoryArguments(
      ref ValidationError? validationError,
      ref string fileStore,
      ref string urn)
   {
      if (string.IsNullOrWhiteSpace(fileStore))
      {
         validationError = new ValidationError("FileStore cannot be empty");
         return;
      }

      if (string.IsNullOrWhiteSpace(urn))
      {
         validationError = new ValidationError("Urn cannot be empty");
         return;
      }

      fileStore = fileStore.Trim();
      urn = urn.Trim();
   }

   /// <summary>
   /// Construction of a <see cref="FileUrn"/> from <see cref="string"/>.
   /// </summary>
   public static ValidationError? Validate(
      string? value,
      IFormatProvider? provider,
      out FileUrn? item)
   {
      if (string.IsNullOrWhiteSpace(value))
      {
         item = null;
         return null;
      }

      // Format: "fileStore:urn"
      var separatorIndex = value.IndexOf(':');

      if (separatorIndex <= 0 || separatorIndex == value.Length - 1)
      {
         item = null;
         return new ValidationError("Invalid FileUrn format. Expected 'fileStore:urn'");
      }

      var fileStore = value[..separatorIndex];
      var urn = value[(separatorIndex + 1)..];

      return Validate(fileStore, urn, out item);
   }

   /// <summary>
   /// Conversion/serialization to <see cref="string"/>.
   /// </summary>
   public string ToValue()
   {
      return $"{FileStore}:{Urn}";
   }
}
