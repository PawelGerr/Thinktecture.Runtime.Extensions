namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// Root configuration for Thinktecture EF Core integration.
/// </summary>
public sealed class Configuration : IEquatable<Configuration>
{
   /// <summary>
   /// Default configuration with default smart enum and keyed value object settings.
   /// </summary>
   public static Configuration Default { get; } = new()
                                                  {
                                                     SmartEnums = SmartEnumConfiguration.Default,
                                                     KeyedValueObjects = KeyedValueObjectConfiguration.NoMaxLength
                                                  };

   /// <summary>
   /// Configuration that disables automatic max length configuration for smart enums.
   /// </summary>
   public static Configuration NoMaxLength { get; } = new()
                                                      {
                                                         SmartEnums = SmartEnumConfiguration.NoMaxLength,
                                                         KeyedValueObjects = KeyedValueObjectConfiguration.NoMaxLength
                                                      };

   /// <summary>
   /// Configuration for smart enums.
   /// Default is <see cref="SmartEnumConfiguration.Default"/>.
   /// </summary>
   public SmartEnumConfiguration SmartEnums { get; init; } = SmartEnumConfiguration.Default;

   /// <summary>
   /// Configuration for keyed value objects.
   /// Default is <see cref="KeyedValueObjectConfiguration.NoMaxLength"/>.
   /// </summary>
   public KeyedValueObjectConfiguration KeyedValueObjects { get; init; } = KeyedValueObjectConfiguration.NoMaxLength;

   /// <summary>
   /// Indicates whether constructors instead of factory methods should be used when reading entities from the database.
   /// Default is true.
   /// </summary>
   /// <remarks>
   /// For keyed value objects only.
   /// </remarks>
   public bool UseConstructorForRead { get; init; } = true;

   /// <inheritdoc />
   public bool Equals(Configuration? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return SmartEnums.Equals(other.SmartEnums)
             && KeyedValueObjects.Equals(other.KeyedValueObjects)
             && UseConstructorForRead == other.UseConstructorForRead;
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return obj is Configuration other && Equals(other);
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
#pragma warning disable CS0618 // Type or member is obsolete
      return HashCode.Combine(SmartEnums, KeyedValueObjects, UseConstructorForRead);
#pragma warning restore CS0618 // Type or member is obsolete
   }
}
