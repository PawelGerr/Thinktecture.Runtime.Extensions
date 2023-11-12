namespace Thinktecture.CodeAnalysis.SmartEnums;

public readonly struct EnumItemNames : IEquatable<EnumItemNames>
{
   private readonly ImmutableArray<IFieldSymbol> _items;

   public int Count => _items.Length;
   public string this[int index] => _items[index].Name;

   public EnumItemNames(ImmutableArray<IFieldSymbol> items)
   {
      _items = items;
   }

   public override bool Equals(object? obj)
   {
      return obj is EnumItemNames other && Equals(other);
   }

   public bool Equals(EnumItemNames other)
   {
      return _items.SequenceEqual(other._items);
   }

   public override int GetHashCode()
   {
      return _items.ComputeHashCode(static f => f.Name);
   }
}
