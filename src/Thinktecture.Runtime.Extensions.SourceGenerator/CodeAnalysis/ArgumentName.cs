namespace Thinktecture.CodeAnalysis;

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
         return ((Name?.GetHashCode() ?? 0) * 397) ^ RenderAsIs.GetHashCode();
      }
   }

   public override string ToString()
   {
      return Name ?? string.Empty;
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
