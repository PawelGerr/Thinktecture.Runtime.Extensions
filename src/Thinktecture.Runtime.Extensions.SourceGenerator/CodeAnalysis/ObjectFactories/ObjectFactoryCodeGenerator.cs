using System.Text;

namespace Thinktecture.CodeAnalysis.ObjectFactories;

public class ObjectFactoryCodeGenerator : CodeGeneratorBase
{
   public override string CodeGeneratorName => "ObjectFactory-CodeGenerator";
   public override string FileNameSuffix => ".ObjectFactories";

   private readonly ObjectFactorySourceGeneratorState _state;
   private readonly StringBuilder _sb;

   public ObjectFactoryCodeGenerator(
      ObjectFactorySourceGeneratorState state,
      StringBuilder sb)
   {
      _state = state;
      _sb = sb;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      _sb.AppendLine(GENERATED_CODE_PREFIX);

      var hasNamespace = _state.Namespace is not null;

      if (hasNamespace)
      {
         _sb.Append(@"
namespace ").Append(_state.Namespace).Append(@";
");
      }

      _sb.RenderContainingTypesStart(_state.ContainingTypes);

      GenerateFactoryStubs();

      _sb.RenderContainingTypesEnd(_state.ContainingTypes).Append(@"
");
   }

   private void GenerateFactoryStubs()
   {
      _sb.Append(@"
[global::System.Diagnostics.CodeAnalysis.SuppressMessage(""ThinktectureRuntimeExtensionsAnalyzer"", ""TTRESG1000:Internal Thinktecture.Runtime.Extensions API usage"")]
partial ").AppendTypeKind(_state).Append(" ").Append(_state.Name).AppendGenericTypeParameters(_state).Append(" :");

      for (var i = 0; i < _state.ObjectFactories.Length; i++)
      {
         var factory = _state.ObjectFactories[i];

         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
      global::Thinktecture.IObjectFactory<").AppendTypeFullyQualified(_state).Append(", ").AppendTypeFullyQualified(factory).Append(", ").AppendTypeFullyQualified(_state.ValidationError).Append(">");

         if (factory.UseForSerialization != SerializationFrameworks.None)
         {
            _sb.Append(@",
      global::Thinktecture.IConvertible<").AppendTypeFullyQualified(factory).Append(">");
         }
      }

      _sb.Append(@"
{
}");
   }
}
