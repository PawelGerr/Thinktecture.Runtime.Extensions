namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// Default max length strategy that calculates max length from string-based smart enum items and rounds up to the next multiple of 10.
/// Does not overwrite existing max length configurations.
/// </summary>
/// <remarks>
/// For string keys: uses the longest string value among all items.
/// For other types: returns null (no max length).
/// </remarks>
public sealed class DefaultSmartEnumMaxLengthStrategy
   : ISmartEnumMaxLengthStrategy,
     IEquatable<DefaultSmartEnumMaxLengthStrategy>
{
   /// <summary>
   /// Instance of the default strategy.
   /// </summary>
   public static readonly ISmartEnumMaxLengthStrategy Instance = new DefaultSmartEnumMaxLengthStrategy();

   /// <inheritdoc />
   public bool OverwriteExistingMaxLength => false;

   /// <inheritdoc />
   public MaxLengthChange GetMaxLength(Type type, Type keyType, IReadOnlyList<ISmartEnumItem> items)
   {
      ArgumentNullException.ThrowIfNull(type);
      ArgumentNullException.ThrowIfNull(keyType);
      ArgumentNullException.ThrowIfNull(items);

      if (items.Count == 0)
         return MaxLengthChange.None;

      // Only calculate max length for string-based keys
      if (keyType != typeof(string))
         return MaxLengthChange.None;

      var maxLength = 0;

      foreach (var item in items)
      {
         if (item.Key is string stringKey)
         {
            if (stringKey.Length > maxLength)
               maxLength = stringKey.Length;
         }
      }

      if (maxLength == 0)
         return MaxLengthChange.None;

      var modulo = maxLength % 10;

      return modulo == 0 ? maxLength : maxLength + (10 - modulo);
   }

   /// <inheritdoc />
   public bool Equals(DefaultSmartEnumMaxLengthStrategy? other)
   {
      return other is not null;
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return obj is DefaultSmartEnumMaxLengthStrategy;
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return typeof(DefaultSmartEnumMaxLengthStrategy).GetHashCode();
   }
}
