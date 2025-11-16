using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class KeyedJsonCodeGenerator : CodeGeneratorBase
{
   private readonly KeyedSerializerGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "Keyed-SystemTextJson-CodeGenerator";
   public override string FileNameSuffix => ".Json";

   public KeyedJsonCodeGenerator(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      var customFactory = _state.AttributeInfo
                                .ObjectFactories
                                .FirstOrDefault(f => f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.SystemTextJson));

      var keyType = customFactory?.TypeFullyQualified ?? _state.KeyMember?.TypeFullyQualified;

      if (keyType is null)
         return;

      var isString = customFactory is null
                        ? _state.KeyMember?.SpecialType == SpecialType.System_String
                        : customFactory.SpecialType == SpecialType.System_String;

      _sb.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (_state.Namespace is not null)
      {
         _sb.Append(@"
namespace ").Append(_state.Namespace).Append(@";
");
      }

      _sb.RenderContainingTypesStart(_state.ContainingTypes);

      var hasGenerics = !_state.GenericParameters.IsDefaultOrEmpty;

      _sb.Append(@"
[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(");

      if (hasGenerics)
      {
         _sb.Append("ValueObjectJsonConverterFactory");
      }
      else
      {
         _sb.Append("global::Thinktecture.Text.Json.Serialization.ThinktectureJsonConverterFactory<").AppendTypeFullyQualified(_state.Type).Append(", ");

         if (!isString)
            _sb.Append(keyType).Append(", ");

         _sb.AppendTypeFullyQualified(_state.AttributeInfo.ValidationError).Append(">");
      }

      _sb.Append(@"))]
");

      _sb.Append("partial ").AppendTypeKind(_state.Type).Append(" ").Append(_state.Name).AppendGenericTypeParameters(_state).Append(@"
{
}");

      _sb.RenderContainingTypesEnd(_state.ContainingTypes);

      if (hasGenerics)
         GenerateFactory(keyType, isString);

      _sb.Append(@"
");
   }

   private void GenerateFactory(string keyType, bool isString)
   {
      _sb.Append(@"

file class ValueObjectJsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory
{
   public override bool CanConvert(global::System.Type typeToConvert)
   {");

      if (!_state.GenericParameters.IsDefaultOrEmpty)
      {
         _sb.Append(@"
      if (!typeToConvert.IsGenericType || typeToConvert.IsGenericTypeDefinition)
         return false;

      return typeof(").AppendTypeFullyQualifiedWithoutGenerics(_state, _state.ContainingTypes).AppendGenericTypeParameters(_state, constructOpenGeneric: true).Append(@") == typeToConvert.GetGenericTypeDefinition();");
      }

      _sb.Append(@"
   }

   public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
   {
      if (typeToConvert is null)
         throw new global::System.ArgumentNullException(nameof(typeToConvert));

      if (options is null)
         throw new global::System.ArgumentNullException(nameof(options));
");

      if (!_state.GenericParameters.IsDefaultOrEmpty)
      {
         _sb.Append(@"
      var converterType = typeof(global::Thinktecture.Text.Json.Serialization.");

         if (isString)
         {
            _sb.Append("ThinktectureJsonConverter<,>).MakeGenericType(typeToConvert, typeof(").AppendTypeFullyQualified(_state.AttributeInfo.ValidationError).Append("))");
         }
         else
         {
            _sb.Append("ThinktectureJsonConverter<,,>).MakeGenericType(typeToConvert, typeof(").Append(keyType).Append("), typeof(").AppendTypeFullyQualified(_state.AttributeInfo.ValidationError).Append("))");
         }

         _sb.Append(@";

      return (global::System.Text.Json.Serialization.JsonConverter?)global::System.Activator.CreateInstance(converterType, options)
         ?? throw new global::System.Exception($""Could not create an instance of json converter of type \""{converterType.FullName}\""."");");
      }

      _sb.Append(@"
   }
}");
   }
}
