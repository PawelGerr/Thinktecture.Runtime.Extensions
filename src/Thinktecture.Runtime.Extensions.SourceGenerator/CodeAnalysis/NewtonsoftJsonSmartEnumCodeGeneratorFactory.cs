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

      // A ref struct cannot be a generic argument of the generated formatter/converter types, so a
      // ref-struct factory (for example ReadOnlySpan<char> or ReadOnlySpan<byte>) is ignored here.
      var hasObjectFactory = state.AttributeInfo.ObjectFactories.Any(static f => !f.IsRefLike && f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.NewtonsoftJson));

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
