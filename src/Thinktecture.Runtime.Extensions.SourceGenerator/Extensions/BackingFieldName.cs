namespace Thinktecture;

public readonly struct BackingFieldName : IEquatable<BackingFieldName>
{
   public readonly string Name;
   public readonly string PropertyName;

   private BackingFieldName(string name, string propertyName)
   {
      PropertyName = propertyName;
      Name = name;
   }

   public static BackingFieldName Create(string name, string propertyName)
   {
      return new BackingFieldName(name, propertyName);
   }

   public override bool Equals(object? obj)
   {
      return obj is BackingFieldName other && Equals(other);
   }

   public bool Equals(BackingFieldName other)
   {
      return Name == other.Name && PropertyName == other.PropertyName;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         return (Name.GetHashCode() * 397) ^ PropertyName.GetHashCode();
      }
   }

   public override string ToString()
   {
      return Name;
   }

   public static bool operator ==(BackingFieldName left, BackingFieldName right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(BackingFieldName left, BackingFieldName right)
   {
      return !(left == right);
   }
}
