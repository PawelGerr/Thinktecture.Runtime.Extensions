using System.Text;

namespace Thinktecture.CodeAnalysis.ObjectFactories;

public sealed class ObjectFactoryCodeGenerator : CodeGeneratorBase
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
   ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
   [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
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
            UseForSerialization = ").AppendSerializationFrameworks(objectFactory.UseForSerialization).Append(@",
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
   // All single-bit members of the enum. Enum.GetValues returns them sorted by their binary value,
   // so the composed output keeps the ascending flag order. Aliases like Json and All are filtered
   // out, so a composed value renders as its individual flags. A future enum member is picked up
   // automatically.
   private static readonly SerializationFrameworks[] _singleBitFrameworks
      = ((SerializationFrameworks[])Enum.GetValues(typeof(SerializationFrameworks)))
        .Where(v => v != 0 && (v & (v - 1)) == 0)
        .ToArray();

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

   public static StringBuilder AppendSerializationFrameworks(
      this StringBuilder sb,
      SerializationFrameworks frameworks)
   {
      const string prefix = "global::Thinktecture.SerializationFrameworks.";

      // A value that matches a defined member (including the composite aliases None, Json and All)
      // renders as that single member name. An unnamed combination (for example SystemTextJson | MessagePack)
      // must be composed from its individual flags with " | ", because the [Flags] enum's ToString()
      // would emit a comma-separated list that is not valid C# inside the object initializer.
      if (Enum.IsDefined(typeof(SerializationFrameworks), frameworks))
         return sb.Append(prefix).Append(frameworks.ToString());

      var first = true;

      foreach (var framework in _singleBitFrameworks)
      {
         if ((frameworks & framework) == 0)
            continue;

         if (!first)
            sb.Append(" | ");

         sb.Append(prefix).Append(framework.ToString());
         first = false;
      }

      // Bits that match no defined member are dropped. Without this guard nothing would be appended
      // at all, which would render as "UseForSerialization = ," and not compile.
      if (first)
         sb.Append(prefix).Append(nameof(SerializationFrameworks.None));

      return sb;
   }
}
