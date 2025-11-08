using System.Text;

namespace Thinktecture.CodeAnalysis;

public abstract class JsonKeyedSerializerCodeGeneratorFactoryBase : IKeyedSerializerCodeGeneratorFactory
{
   private readonly bool _isForObjectFactories;

   public abstract string CodeGeneratorName { get; }

   protected JsonKeyedSerializerCodeGeneratorFactoryBase(
      bool isForObjectFactories)
   {
      _isForObjectFactories = isForObjectFactories;
   }

   public bool MustGenerateCode(KeyedSerializerGeneratorState state)
   {
      if (state.AttributeInfo.HasJsonConverterAttribute
          || !state.SerializationFrameworks.HasSerializationFramework(SerializationFrameworks.SystemTextJson))
         return false;

      var hasObjectFactory = state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.SystemTextJson));

      if (_isForObjectFactories)
         return hasObjectFactory;

      return state.KeyMember is not null && !hasObjectFactory;
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
