using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class KeyedNewtonsoftJsonCodeGenerator : CodeGeneratorBase
{
   private readonly KeyedSerializerGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "Keyed-NewtonsoftJson-CodeGenerator";
   public override string FileNameSuffix => ".NewtonsoftJson";

   public KeyedNewtonsoftJsonCodeGenerator(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      var customFactory = _state.AttributeInfo
                                .ObjectFactories
                                .FirstOrDefault(f => f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.NewtonsoftJson));
      var keyType = customFactory?.TypeFullyQualified ?? _state.KeyMember?.TypeFullyQualified;

      if (keyType is null)
         return;

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
[global::Newtonsoft.Json.JsonConverterAttribute(typeof(");

      if (hasGenerics)
      {
         _sb.Append("ValueObjectNewtonsoftJsonConverterFactory");
      }
      else
      {
         _sb.Append("global::Thinktecture.Json.ThinktectureNewtonsoftJsonConverter<").AppendTypeFullyQualified(_state.Type).Append(", ").Append(keyType).Append(", ").AppendTypeFullyQualified(_state.AttributeInfo.ValidationError).Append(">");
      }

      _sb.Append(@"))]
");

      _sb.Append("partial ").AppendTypeKind(_state.Type).Append(" ").Append(_state.Name).AppendGenericTypeParameters(_state).Append(@"
{
}");

      _sb.RenderContainingTypesEnd(_state.ContainingTypes);

      if (hasGenerics)
         GenerateFactory(keyType);

      _sb.Append(@"
");
   }

   private void GenerateFactory(string keyType)
   {
      _sb.Append(@"

file class ValueObjectNewtonsoftJsonConverterFactory : global::Newtonsoft.Json.JsonConverter
{
   private static readonly global::System.Collections.Concurrent.ConcurrentDictionary<global::System.Type, global::Newtonsoft.Json.JsonConverter> _converterByType = new();

   public override bool CanConvert(global::System.Type objectType)
   {
      objectType = global::System.Nullable.GetUnderlyingType(objectType) ?? objectType;
");

      if (!_state.GenericParameters.IsDefaultOrEmpty)
      {
         _sb.Append(@"
      if (!objectType.IsGenericType || objectType.IsGenericTypeDefinition)
         return false;

      return typeof(").AppendTypeFullyQualifiedWithoutGenerics(_state, _state.ContainingTypes).AppendGenericTypeParameters(_state, constructOpenGeneric: true).Append(@") == objectType.GetGenericTypeDefinition();");
      }

      _sb.Append(@"
   }

   public override object? ReadJson(global::Newtonsoft.Json.JsonReader reader, global::System.Type objectType, object? existingValue, global::Newtonsoft.Json.JsonSerializer serializer)
   {
      return _converterByType.GetOrAdd(objectType, CreateConverter).ReadJson(reader, objectType, existingValue, serializer);
   }

   public override void WriteJson(global::Newtonsoft.Json.JsonWriter writer, object? value, global::Newtonsoft.Json.JsonSerializer serializer)
   {
      if (value is null)
      {
         writer.WriteNull();
      }
      else
      {
         _converterByType.GetOrAdd(value.GetType(), CreateConverter).WriteJson(writer, value, serializer);
      }
   }

   private static global::Newtonsoft.Json.JsonConverter CreateConverter(global::System.Type objectType)
   {
      if (objectType is null)
         throw new global::System.ArgumentNullException(nameof(objectType));

      objectType = global::System.Nullable.GetUnderlyingType(objectType) ?? objectType;

      var converterType = typeof(global::Thinktecture.Json.ThinktectureNewtonsoftJsonConverter<,,>).MakeGenericType(objectType, typeof(").Append(keyType).Append("), typeof(").AppendTypeFullyQualified(_state.AttributeInfo.ValidationError).Append(@"));

      return (global::Newtonsoft.Json.JsonConverter?)global::System.Activator.CreateInstance(converterType)
         ?? throw new global::System.Exception($""Could not create an instance of json converter of type \""{converterType.FullName}\""."");
   }
}");
   }
}
