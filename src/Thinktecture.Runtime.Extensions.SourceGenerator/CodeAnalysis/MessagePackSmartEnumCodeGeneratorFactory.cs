using System.Text;

namespace Thinktecture.CodeAnalysis;

public abstract class MessagePackKeyedSerializerCodeGeneratorFactoryBase : IKeyedSerializerCodeGeneratorFactory
{
   public abstract string CodeGeneratorName { get; }

   public bool MustGenerateCode(KeyedSerializerGeneratorState state)
   {
      return !state.AttributeInfo.HasMessagePackFormatterAttribute
             && state.SerializationFrameworks.HasFlag(SerializationFrameworks.MessagePack)
             && (state.KeyMember is not null || state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization.Has(SerializationFrameworks.MessagePack)));
   }

   public CodeGeneratorBase Create(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new KeyedMessagePackCodeGenerator(state, stringBuilder);
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
