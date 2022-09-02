namespace Thinktecture.CodeAnalysis.SmartEnums;

public class NewtonsoftJsonSmartEnumCodeGenerator : CodeGeneratorBase
{
   private readonly EnumSourceGeneratorState _state;

   public override string FileNameSuffix => ".NewtonsoftJson";

   public NewtonsoftJsonSmartEnumCodeGenerator(EnumSourceGeneratorState state)
   {
      _state = state;
   }

   public override string? Generate()
   {
      if (_state.AttributeInfo.HasNewtonsoftJsonConverterAttribute)
         return null;

      var ns = _state.Namespace;

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial {(_state.IsReferenceType ? "class" : "struct")} {_state.Name}
   {{
      public class ValueObjectNewtonsoftJsonConverter : global::Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<{_state.EnumTypeFullyQualified}, {_state.KeyProperty.TypeFullyQualifiedWithNullability}>
      {{
         public ValueObjectNewtonsoftJsonConverter()
            : base({_state.EnumTypeFullyQualified}.Get, static obj => obj.{_state.KeyProperty.Name})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
