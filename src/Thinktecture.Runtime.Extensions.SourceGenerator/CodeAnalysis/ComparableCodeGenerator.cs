using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class ComparableCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Default = new ComparableCodeGenerator(null, true);
   public static readonly IInterfaceCodeGenerator NonGeneric = new ComparableCodeGenerator(null, false);

   private readonly string? _comparerAccessor;
   private readonly bool _isKeyMemberGenericComparable;

   public string CodeGeneratorName => "Comparable-CodeGenerator";
   public string FileNameSuffix => ".Comparable";
   public bool CanAppendColon => true;

   public ComparableCodeGenerator(string? comparerAccessor, bool isKeyMemberGenericComparable = true)
   {
      _comparerAccessor = comparerAccessor;
      _isKeyMemberGenericComparable = isKeyMemberGenericComparable;
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
   ").Append(CodeGeneratorBase.GENERATED_CODE_ATTRIBUTE).Append(@"
   [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
   public int CompareTo(object? obj)
   {
      if(obj is null)
         return 1;

      if(obj is not ").AppendTypeFullyQualified(state.Type).Append(@" item)
         throw new global::System.ArgumentException(""Argument must be of type \""").AppendTypeMinimallyQualified(state.Type).Append(@"\""."", nameof(obj));

      return this.CompareTo(item);
   }

   /// <inheritdoc />
   ").Append(CodeGeneratorBase.GENERATED_CODE_ATTRIBUTE).Append(@"
   [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
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
         if (state.KeyMember.MayBeNull())
         {
            sb.Append(@"
      if(this.").Append(state.KeyMember.Name).Append(@" is null)
         return obj.").Append(state.KeyMember.Name).Append(@" is null ? 0 : -1;
      if(obj.").Append(state.KeyMember.Name).Append(@" is null)
         return 1;
");
         }

         if (_isKeyMemberGenericComparable)
         {
            sb.Append(@"
      return ((global::System.IComparable<").AppendTypeFullyQualified(state.KeyMember).Append(">)this.").Append(state.KeyMember.Name).Append(").CompareTo(obj.").Append(state.KeyMember.Name).Append(");");
         }
         else
         {
            // The key member type (e.g. an enum) implements only the non-generic System.IComparable.
            sb.Append(@"
      return ((global::System.IComparable)this.").Append(state.KeyMember.Name).Append(").CompareTo(obj.").Append(state.KeyMember.Name).Append(");");
         }
      }

      sb.Append(@"
   }");
   }
}
