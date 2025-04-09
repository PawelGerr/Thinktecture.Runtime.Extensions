using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class ProtoBufSmartEnumCodeGeneratorFactory : IKeyedSerializerCodeGeneratorFactory
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new ProtoBufSmartEnumCodeGeneratorFactory();

   public string CodeGeneratorName => "ProtoBuf-SmartEnum-CodeGenerator";

   private ProtoBufSmartEnumCodeGeneratorFactory()
   {
   }

   public bool MustGenerateCode(KeyedSerializerGeneratorState state)
   {
      return !state.AttributeInfo.HasProtoContractAttribute
             && (state.KeyMember is not null || state.AttributeInfo.DesiredFactories.Any(f => f.UseForSerialization.Has(SerializationFrameworks.ProtoBuf)));
   }

   public CodeGeneratorBase Create(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new KeyedProtoBufCodeGenerator(state, stringBuilder);
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
