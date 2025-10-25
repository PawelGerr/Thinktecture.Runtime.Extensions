namespace Thinktecture;

public readonly struct ArgumentName : IEquatable<ArgumentName>
{
   public readonly string Name;
   public readonly bool RenderAsIs;

   private ArgumentName(string name, bool renderAsIs)
   {
      RenderAsIs = renderAsIs;
      Name = name;
   }

   public static ArgumentName Create(string name, bool renderAsIs = false)
   {
      return new ArgumentName(name, renderAsIs);
   }

   public bool Equals(string other)
   {
      return Name.Equals(other, StringComparison.OrdinalIgnoreCase);
   }

   public override bool Equals(object? obj)
   {
      return obj is ArgumentName other && Equals(other);
   }

   public bool Equals(ArgumentName other)
   {
      return Name == other.Name && RenderAsIs == other.RenderAsIs;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         return (Name.GetHashCode() * 397) ^ RenderAsIs.GetHashCode();
      }
   }

   public override string ToString()
   {
      return Name;
   }

   public static bool operator ==(ArgumentName left, ArgumentName right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(ArgumentName left, ArgumentName right)
   {
      return !(left == right);
   }
}
