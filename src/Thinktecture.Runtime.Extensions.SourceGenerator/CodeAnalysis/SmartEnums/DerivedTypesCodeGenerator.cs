using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public class DerivedTypesCodeGenerator : CodeGeneratorBase
{
   private readonly SmartEnumDerivedTypes _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "DerivedTypes-CodeGenerator";
   public override string FileNameSuffix => ".DerivedTypes";

   public DerivedTypesCodeGenerator(SmartEnumDerivedTypes state, StringBuilder stringBuilder)
   {
      _state = state;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      _sb.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (_state.Namespace is not null)
      {
         _sb.Append(@"
namespace ").Append(_state.Namespace).Append(@";
");
      }

      _sb.RenderContainingTypesStart(_state.ContainingTypes);

      _sb.Append(@"
partial ").Append(_state.IsReferenceType ? "class" : "struct").Append(" ").Append(_state.Name).Append(@"
{
   [global::System.Runtime.CompilerServices.ModuleInitializer]
   internal static void DerivedTypesModuleInit()
   {
      var enumType = typeof(").AppendTypeFullyQualified(_state).Append(");");

      foreach (var derivedType in _state.DerivedTypesFullyQualified)
      {
         _sb.Append(@"
      global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddDerivedType(enumType, typeof(").Append(derivedType).Append("));");
      }

      _sb.Append(@"
   }
}");

      _sb.RenderContainingTypesEnd(_state.ContainingTypes)
         .Append(@"
");
   }
}
