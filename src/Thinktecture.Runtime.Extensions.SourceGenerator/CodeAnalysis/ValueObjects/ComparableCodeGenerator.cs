using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComparableCodeGenerator : IInterfaceCodeGenerator
{
   private readonly string? _comparerAccessor;

   public ComparableCodeGenerator(string? comparerAccessor)
   {
      _comparerAccessor = comparerAccessor;
   }

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type)
   {
      sb.Append($"global::System.IComparable, global::System.IComparable<{type.TypeFullyQualified}>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState member)
   {
      sb.Append($@"

      /// <inheritdoc />
      public int CompareTo(object? obj)
      {{
         if(obj is null)
            return 1;

         if(obj is not {type.TypeFullyQualified} valueObject)
            throw new global::System.ArgumentException(""Argument must be of type \""{type.TypeMinimallyQualified}\""."", nameof(obj));

         return this.CompareTo(valueObject);
      }}

      /// <inheritdoc />
      public int CompareTo({type.TypeFullyQualifiedNullAnnotated} obj)
      {{");

      if (type.IsReferenceType)
      {
         sb.Append(@"
         if(obj is null)
            return 1;
");
      }

      if (_comparerAccessor is null)
      {
         if (member.IsReferenceType)
         {
            sb.Append($@"
         if(this.{member.Name} is null)
            return obj.{member.Name} is null ? 0 : -1;
");
         }

         sb.Append($@"
         return this.{member.Name}.CompareTo(obj.{member.Name});");
      }
      else
      {
         sb.Append($@"
         return {_comparerAccessor}.Comparer.Compare(this.{member.Name}, obj.{member.Name});");
      }

      sb.Append(@"
      }");
   }
}
