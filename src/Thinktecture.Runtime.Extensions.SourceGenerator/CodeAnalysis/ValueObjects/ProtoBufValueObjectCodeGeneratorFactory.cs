using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ProtoBufValueObjectCodeGeneratorFactory : IValueObjectSerializerCodeGeneratorFactory
{
   public static readonly IValueObjectSerializerCodeGeneratorFactory Instance = new ProtoBufValueObjectCodeGeneratorFactory();

   public string CodeGeneratorName => "ProtoBuf-ValueObject-CodeGenerator";

   private ProtoBufValueObjectCodeGeneratorFactory()
   {
   }

   public bool MustGenerateCode(KeyedSerializerGeneratorState state)
   {
      return !state.AttributeInfo.HasProtoContractAttribute
             && (state.KeyMember is not null || state.AttributeInfo.DesiredFactories.Any(f => f.UseForSerialization.Has(SerializationFrameworks.ProtoBuf)));
   }

   public bool MustGenerateCode(ComplexSerializerGeneratorState state)
   {
      return false;
   }

   public CodeGeneratorBase Create(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new KeyedProtoBufCodeGenerator(state, stringBuilder);
   }

   public CodeGeneratorBase Create(ComplexSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      throw new NotSupportedException();
   }

   public bool Equals(IValueObjectSerializerCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public bool Equals(ICodeGeneratorFactory<KeyedSerializerGeneratorState> other) => ReferenceEquals(this, other);
   public bool Equals(ICodeGeneratorFactory<ComplexSerializerGeneratorState> other) => ReferenceEquals(this, other);
   public bool Equals(IKeyedSerializerCodeGeneratorFactory other) => ReferenceEquals(this, other);
   public bool Equals(IComplexSerializerCodeGeneratorFactory other) => ReferenceEquals(this, other);
}
