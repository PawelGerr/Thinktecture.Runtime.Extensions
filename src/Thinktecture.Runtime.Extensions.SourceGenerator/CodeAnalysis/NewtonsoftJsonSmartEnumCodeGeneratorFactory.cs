using System.Text;

namespace Thinktecture.CodeAnalysis;

public abstract class NewtonsoftJsonKeyedSerializerCodeGeneratorFactoryBase : IKeyedSerializerCodeGeneratorFactory
{
   private readonly bool _isForObjectFactories;

   public abstract string CodeGeneratorName { get; }

   protected NewtonsoftJsonKeyedSerializerCodeGeneratorFactoryBase(
      bool isForObjectFactories)
   {
      _isForObjectFactories = isForObjectFactories;
   }

   public bool MustGenerateCode(KeyedSerializerGeneratorState state)
   {
      if (state.AttributeInfo.HasNewtonsoftJsonConverterAttribute
          || !state.SerializationFrameworks.HasSerializationFramework(SerializationFrameworks.NewtonsoftJson))
         return false;

      var hasObjectFactory = state.AttributeInfo.ObjectFactories.Any(static f => f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.NewtonsoftJson));

      if (_isForObjectFactories)
         return hasObjectFactory;

      return state.KeyMember is not null && !hasObjectFactory;
   }

   public CodeGeneratorBase Create(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new KeyedNewtonsoftJsonCodeGenerator(state, stringBuilder);
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
