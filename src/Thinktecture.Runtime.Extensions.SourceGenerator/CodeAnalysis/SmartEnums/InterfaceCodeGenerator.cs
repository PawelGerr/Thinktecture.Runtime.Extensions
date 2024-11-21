using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public class InterfaceCodeGenerator<TState> : CodeGeneratorBase
   where TState : ITypeInformationProvider
{
   private readonly IInterfaceCodeGenerator<TState> _codeGenerator;
   private readonly TState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => _codeGenerator.CodeGeneratorName;
   public override string FileNameSuffix => _codeGenerator.FileNameSuffix;

   public InterfaceCodeGenerator(
      IInterfaceCodeGenerator<TState> codeGenerator,
      TState state,
      StringBuilder stringBuilder)
   {
      _codeGenerator = codeGenerator;
      _state = state;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      _sb.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (_state.Type.Namespace is not null)
      {
         _sb.Append(@"
namespace ").Append(_state.Type.Namespace).Append(@";
");
      }

      _sb.RenderContainingTypesStart(_state.Type.ContainingTypes);

      _sb.Append(@"
partial ").Append(_state.Type.IsReferenceType ? "class" : "struct").Append(" ").Append(_state.Type.Name).Append(" :");

      _codeGenerator.GenerateBaseTypes(_sb, _state);

      _sb.Append(@"
{");

      _codeGenerator.GenerateImplementation(_sb, _state);

      _sb.Append(@"
}");

      _sb.RenderContainingTypesEnd(_state.Type.ContainingTypes)
         .Append(@"
");
   }
}
