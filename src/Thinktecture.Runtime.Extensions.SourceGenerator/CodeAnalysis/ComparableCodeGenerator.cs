using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class ComparableCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Default = new ComparableCodeGenerator(null);

   private readonly string? _comparerAccessor;

   public ComparableCodeGenerator(string? comparerAccessor)
   {
      _comparerAccessor = comparerAccessor;
   }

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberState keyMember)
   {
      sb.Append(@$"
      global::System.IComparable,
      global::System.IComparable<{type.TypeFullyQualified}>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState keyMember)
   {
      sb.Append($@"

      /// <inheritdoc />
      public int CompareTo(object? obj)
      {{
         if(obj is null)
            return 1;

         if(obj is not {type.TypeFullyQualified} item)
            throw new global::System.ArgumentException(""Argument must be of type \""{type.TypeMinimallyQualified}\""."", nameof(obj));

         return this.CompareTo(item);
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
         sb.Append($@"
         return this.{keyMember.Name}.CompareTo(obj.{keyMember.Name});");
      }
      else
      {
         sb.Append($@"
         return {_comparerAccessor}.Comparer.Compare(this.{keyMember.Name}, obj.{keyMember.Name});");
      }

      sb.Append(@"
      }");
   }
}
