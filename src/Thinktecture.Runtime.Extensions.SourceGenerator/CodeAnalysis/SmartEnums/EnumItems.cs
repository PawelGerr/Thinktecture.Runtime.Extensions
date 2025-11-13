namespace Thinktecture.CodeAnalysis.SmartEnums;

public readonly struct EnumItems : IEquatable<EnumItems>
{
   private readonly ImmutableArray<EnumItem> _items;

   public int Count => _items.Length;
   public EnumItem this[int index] => _items[index];

   public EnumItems(ImmutableArray<IFieldSymbol> items)
   {
      _items = ImmutableArray.CreateRange(items, i => new EnumItem(i));
   }

   public override bool Equals(object? obj)
   {
      return obj is EnumItems other && Equals(other);
   }

   public bool Equals(EnumItems other)
   {
      return _items.SequenceEqual(other._items);
   }

   public override int GetHashCode()
   {
      return _items.ComputeHashCode();
   }

   public static bool operator ==(EnumItems left, EnumItems right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(EnumItems left, EnumItems right)
   {
      return !(left == right);
   }
}
