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
partial ").AppendTypeKind(_state).Append(" ").Append(_state.Name).AppendGenericTypeParameters(_state).Append(" : global::Thinktecture.Internal.IObjectFactoryOwner");

      for (var i = 0; i < _state.AttributeInfo.ObjectFactories.Length; i++)
      {
         var factory = _state.AttributeInfo.ObjectFactories[i];

         _sb.Append(@",
      global::Thinktecture.IObjectFactory<").AppendTypeFullyQualified(_state).Append(", ").AppendTypeFullyQualified(factory).Append(", ").AppendTypeFullyQualified(_state.AttributeInfo.ValidationError).Append(">");

         if (factory.UseForSerialization != SerializationFrameworks.None
             || factory.UseWithEntityFramework)
         {
            _sb.Append(@",
      global::Thinktecture.IConvertible<").AppendTypeFullyQualified(factory).Append(">");
         }
      }

      _sb.Append(@"
{");

      GenerateMetadata();

      _sb.Append(@"
}");
   }

   private void GenerateMetadata()
   {
      _sb.Append(@"
   static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Internal.ObjectFactoryMetadata> global::Thinktecture.Internal.IObjectFactoryOwner.ObjectFactories { get; } =
      new global::System.Collections.Generic.List<global::Thinktecture.Internal.ObjectFactoryMetadata>()
      {");

      for (var i = 0; i < _state.AttributeInfo.ObjectFactories.Length; i++)
      {
         var objectFactory = _state.AttributeInfo.ObjectFactories[i];

         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
         new global::Thinktecture.Internal.ObjectFactoryMetadata()
         {
            ValueType = typeof(").AppendTypeFullyQualified(objectFactory).Append(@"),
            ValidationErrorType = typeof(").AppendTypeFullyQualified(_state.AttributeInfo.ValidationError).Append(@"),
            UseForSerialization = global::Thinktecture.SerializationFrameworks.").Append(objectFactory.UseForSerialization).Append(@",
            UseWithEntityFramework = ").Append(objectFactory.UseWithEntityFramework ? "true" : "false").Append(@",
            UseForModelBinding = ").Append(objectFactory.UseForModelBinding ? "true" : "false").Append(@",
            ConvertFromKeyExpressionViaConstructor = ").AppendConvertFromKeyExpressionViaConstructor(_state, objectFactory).Append(@",
         }");
      }

      _sb.Append(@"
      }.AsReadOnly();");
   }
}

file static class StringBuilderExtensions
{
   public static StringBuilder AppendConvertFromKeyExpressionViaConstructor(
      this StringBuilder sb,
      ObjectFactorySourceGeneratorState state,
      ObjectFactoryState factoryState)
   {
      if (!factoryState.HasCorrespondingConstructor)
      {
         sb.Append("null");
         return sb;
      }

      return sb.Append("static ").AppendTypeFullyQualified(state).Append(" (").AppendTypeFullyQualified(factoryState).Append(" @value) => new ").AppendTypeFullyQualified(state).Append("(@value)");
   }
}
