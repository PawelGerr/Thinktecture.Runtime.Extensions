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
   global::System.IComparable<").AppendTypeFullyQualified(state.Type).Append(">");
   }

   public void GenerateImplementation(StringBuilder sb, InterfaceCodeGeneratorState state)
   {
      sb.Append(@"
   /// <inheritdoc />
   public int CompareTo(object? obj)
   {
      if(obj is null)
         return 1;

      if(obj is not ").AppendTypeFullyQualified(state.Type).Append(@" item)
         throw new global::System.ArgumentException(""Argument must be of type \""").AppendTypeMinimallyQualified(state.Type).Append(@"\""."", nameof(obj));

      return ((global::System.IComparable)this).CompareTo(item);
   }

   /// <inheritdoc />
   public int CompareTo(").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(@" obj)
   {");

      if (state.Type.IsReferenceType)
      {
         sb.Append(@"
      if(obj is null)
         return 1;
");
      }

      if (_comparerAccessor is not null)
      {
         sb.Append(@"
      return ").Append(_comparerAccessor).Append(".Comparer.Compare(this.").Append(state.KeyMember.Name).Append(", obj.").Append(state.KeyMember.Name).Append(");");
      }
      else if (state.KeyMember.IsString())
      {
         sb.Append(@"
      return global::System.StringComparer.OrdinalIgnoreCase.Compare(this.").Append(state.KeyMember.Name).Append(", obj.").Append(state.KeyMember.Name).Append(");");
      }
      else
      {
         sb.Append(@"
      return ((global::System.IComparable<").AppendTypeFullyQualified(state.KeyMember).Append(">)this.").Append(state.KeyMember.Name).Append(").CompareTo(obj.").Append(state.KeyMember.Name).Append(");");
      }

      sb.Append(@"
   }");
   }
}
