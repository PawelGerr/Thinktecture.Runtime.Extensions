using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class ComparableCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Default = new ComparableCodeGenerator(null);

   private readonly string? _comparerAccessor;

   public string CodeGeneratorName => "Comparable-CodeGenerator";
   public string FileNameSuffix => ".Comparable";

   public ComparableCodeGenerator(string? comparerAccessor)
   {
      _comparerAccessor = comparerAccessor;
   }

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      sb.Append(@"
   global::System.IComparable,
   global::System.IComparable<").Append(type.TypeFullyQualified).Append(">");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      sb.Append(@"
   /// <inheritdoc />
   public int CompareTo(object? obj)
   {
      if(obj is null)
         return 1;

      if(obj is not ").Append(type.TypeFullyQualified).Append(@" item)
         throw new global::System.ArgumentException(""Argument must be of type \""").Append(type.TypeMinimallyQualified).Append(@"\""."", nameof(obj));

      return this.CompareTo(item);
   }

   /// <inheritdoc />
   public int CompareTo(").Append(type.TypeFullyQualifiedNullAnnotated).Append(@" obj)
   {");

      if (type.IsReferenceType)
      {
         sb.Append(@"
      if(obj is null)
         return 1;
");
      }

      if (_comparerAccessor is null)
      {
         sb.Append(@"
      return this.").Append(keyMember.Name).Append(".CompareTo(obj.").Append(keyMember.Name).Append(");");
      }
      else
      {
         sb.Append(@"
      return ").Append(_comparerAccessor).Append(".Comparer.Compare(this.").Append(keyMember.Name).Append(", obj.").Append(keyMember.Name).Append(");");
      }

      sb.Append(@"
   }");
   }
}
