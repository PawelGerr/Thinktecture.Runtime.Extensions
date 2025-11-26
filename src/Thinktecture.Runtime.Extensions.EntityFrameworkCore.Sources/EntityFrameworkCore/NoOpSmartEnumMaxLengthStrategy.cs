namespace Thinktecture.EntityFrameworkCore;

/// <summary>
/// No-op max length strategy for smart enums that never sets a max length.
/// Use this to completely disable automatic max length configuration.
/// </summary>
public sealed class NoOpSmartEnumMaxLengthStrategy
   : ISmartEnumMaxLengthStrategy,
     IEquatable<NoOpSmartEnumMaxLengthStrategy>
{
   /// <summary>
   /// Instance of the no-op strategy.
   /// </summary>
   public static readonly ISmartEnumMaxLengthStrategy Instance = new NoOpSmartEnumMaxLengthStrategy();

   /// <inheritdoc />
   public bool OverwriteExistingMaxLength => false;

   /// <inheritdoc />
   public MaxLengthChange GetMaxLength(Type type, Type keyType, IReadOnlyList<ISmartEnumItem> items)
   {
      return MaxLengthChange.None;
   }

   /// <inheritdoc />
   public bool Equals(NoOpSmartEnumMaxLengthStrategy? other)
   {
      return other is not null;
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      return obj is NoOpSmartEnumMaxLengthStrategy;
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return typeof(NoOpSmartEnumMaxLengthStrategy).GetHashCode();
   }
}
