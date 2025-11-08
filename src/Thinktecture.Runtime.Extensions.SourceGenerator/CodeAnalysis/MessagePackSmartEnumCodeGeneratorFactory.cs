using System.Text;

namespace Thinktecture.CodeAnalysis;

public abstract class MessagePackKeyedSerializerCodeGeneratorFactoryBase : IKeyedSerializerCodeGeneratorFactory
{
   private readonly bool _isForObjectFactories;
   public abstract string CodeGeneratorName { get; }

   protected MessagePackKeyedSerializerCodeGeneratorFactoryBase(
      bool isForObjectFactories)
   {
      _isForObjectFactories = isForObjectFactories;
   }

   public bool MustGenerateCode(KeyedSerializerGeneratorState state)
   {
      if (state.AttributeInfo.HasMessagePackFormatterAttribute
          || !state.SerializationFrameworks.HasSerializationFramework(SerializationFrameworks.MessagePack))
         return false;

      var hasObjectFactory = state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.MessagePack));

      if (_isForObjectFactories)
         return hasObjectFactory;

      return state.KeyMember is not null && !hasObjectFactory;
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
