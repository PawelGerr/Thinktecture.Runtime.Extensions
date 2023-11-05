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

   public void GenerateBaseTypes(StringBuilder sb, InterfaceCodeGeneratorState state)
   {
      sb.Append(@"
   global::System.IComparable,
   global::System.IComparable<").Append(state.Type.TypeFullyQualified).Append(">");
   }

   public void GenerateImplementation(StringBuilder sb, InterfaceCodeGeneratorState state)
   {
      sb.Append(@"
   /// <inheritdoc />
   public int CompareTo(object? obj)
   {
      if(obj is null)
         return 1;

      if(obj is not ").Append(state.Type.TypeFullyQualified).Append(@" item)
         throw new global::System.ArgumentException(""Argument must be of type \""").Append(state.Type.TypeMinimallyQualified).Append(@"\""."", nameof(obj));

      return this.CompareTo(item);
   }

   /// <inheritdoc />
   public int CompareTo(").Append(state.Type.TypeFullyQualifiedNullAnnotated).Append(@" obj)
   {");

      if (state.Type.IsReferenceType)
      {
         sb.Append(@"
      if(obj is null)
         return 1;
");
      }

      if (_comparerAccessor is null)
      {
         sb.Append(@"
      return this.").Append(state.KeyMember.Name).Append(".CompareTo(obj.").Append(state.KeyMember.Name).Append(");");
      }
      else
      {
         sb.Append(@"
      return ").Append(_comparerAccessor).Append(".Comparer.Compare(this.").Append(state.KeyMember.Name).Append(", obj.").Append(state.KeyMember.Name).Append(");");
      }

      sb.Append(@"
   }");
   }
}
