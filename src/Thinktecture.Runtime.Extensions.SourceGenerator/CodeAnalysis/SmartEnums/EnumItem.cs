namespace Thinktecture.CodeAnalysis.SmartEnums;

public readonly struct EnumItem : IEquatable<EnumItem>, IHashCodeComputable
{
   public string Name { get; }
   public ArgumentName ArgumentName { get; }

   public EnumItem(IFieldSymbol field)
   {
      Name = field.Name;
      ArgumentName = ArgumentName.Create(field.Name);
   }

   public override bool Equals(object? obj)
   {
      return obj is EnumItem other && Equals(other);
   }

   public bool Equals(EnumItem other)
   {
      return Name == other.Name;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }
}
