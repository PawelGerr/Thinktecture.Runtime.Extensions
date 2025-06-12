using System.Text;

namespace Thinktecture.CodeAnalysis;

public abstract class JsonKeyedSerializerCodeGeneratorFactoryBase : IKeyedSerializerCodeGeneratorFactory
{
   public abstract string CodeGeneratorName { get; }

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
