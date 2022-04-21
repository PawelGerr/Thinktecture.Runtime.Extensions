using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

[Generator]
public class NewtonsoftJsonSmartEnumSourceGenerator : SmartEnumSourceGeneratorBase<NewtonsoftJsonEnumSourceGeneratorState, NewtonsoftJsonBaseEnumExtension>
{
   public NewtonsoftJsonSmartEnumSourceGenerator()
      : base("_NewtonsoftJson")
   {
   }

   protected override NewtonsoftJsonEnumSourceGeneratorState CreateState(INamedTypeSymbol type, INamedTypeSymbol enumInterface)
   {
      return new NewtonsoftJsonEnumSourceGeneratorState(type, enumInterface);
   }

   protected override string GenerateEnum(NewtonsoftJsonEnumSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      if (state.HasJsonConverterAttribute)
         return String.Empty;

      var ns = state.Namespace;
      var requiresNew = state.HasBaseEnum && (state.BaseEnum.IsSameAssembly || state.BaseEnum.Extension.HasValueObjectNewtonsoftJsonConverter);

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial {(state.IsReferenceType ? "class" : "struct")} {state.Name}
   {{
      public {(requiresNew ? "new " : null)}class ValueObjectNewtonsoftJsonConverter : global::Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<{state.EnumTypeFullyQualified}, {state.KeyProperty.TypeFullyQualifiedWithNullability}>
      {{
         public ValueObjectNewtonsoftJsonConverter()
            : base({state.EnumTypeFullyQualified}.Get, static obj => obj.{state.KeyProperty.Name})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
