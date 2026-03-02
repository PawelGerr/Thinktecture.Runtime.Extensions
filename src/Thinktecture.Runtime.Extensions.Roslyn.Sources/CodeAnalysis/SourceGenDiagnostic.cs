using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

public readonly struct SourceGenDiagnostic(
   TypeDeclarationSyntax node,
   DiagnosticDescriptor descriptor,
   object[] args)
   : IEquatable<SourceGenDiagnostic>
{
   public Location IdentifierLocation { get; } = node.Identifier.GetLocation();
   public DiagnosticDescriptor Descriptor { get; } = descriptor;
   public object[] Args { get; } = args;

   public bool Equals(SourceGenDiagnostic other)
   {
      return IdentifierLocation.Equals(other.IdentifierLocation)
             && Descriptor.Equals(other.Descriptor)
             && Args.SequenceEqual(other.Args);
   }

   public override bool Equals(object? obj)
   {
      return obj is SourceGenDiagnostic other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = IdentifierLocation.GetHashCode();
         hashCode = (hashCode * 397) ^ Descriptor.GetHashCode();
         hashCode = (hashCode * 397) ^ ComputeHashCode(Args);
         return hashCode;
      }
   }

   private static int ComputeHashCode(object[] collection)
   {
      var hashCode = 0;
      var count = collection.Length;

      for (var i = 0; i < count; i++)
      {
         hashCode = unchecked(hashCode * 397) ^ collection[i].GetHashCode();
      }

      return hashCode;
   }

   public static bool operator ==(SourceGenDiagnostic diagnostic, SourceGenDiagnostic other)
   {
      return diagnostic.Equals(other);
   }

   public static bool operator !=(SourceGenDiagnostic diagnostic, SourceGenDiagnostic other)
   {
      return !(diagnostic == other);
   }
}
