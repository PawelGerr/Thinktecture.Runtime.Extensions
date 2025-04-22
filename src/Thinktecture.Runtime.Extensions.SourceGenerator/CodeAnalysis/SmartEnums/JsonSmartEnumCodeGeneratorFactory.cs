using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class JsonSmartEnumCodeGeneratorFactory : IKeyedSerializerCodeGeneratorFactory
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new JsonSmartEnumCodeGeneratorFactory();

   public string CodeGeneratorName => "SystemTextJson-SmartEnum-CodeGenerator";

   private JsonSmartEnumCodeGeneratorFactory()
   {
   }

   public bool MustGenerateCode(KeyedSerializerGeneratorState state)
   {
      return !state.AttributeInfo.HasJsonConverterAttribute
             && state.SerializationFrameworks.HasFlag(SerializationFrameworks.SystemTextJson)
             && (state.KeyMember is not null || state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization.Has(SerializationFrameworks.SystemTextJson)));
   }

   public CodeGeneratorBase Create(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new KeyedJsonCodeGenerator(state, stringBuilder);
   }

   public bool Equals(IKeyedSerializerCodeGeneratorFactory other)
   {
      return ReferenceEquals(this, other);
   }

   public bool Equals(ICodeGeneratorFactory<KeyedSerializerGeneratorState> other)
   {
      return ReferenceEquals(this, other);
   }
}
