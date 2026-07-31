using System.Text;

namespace Thinktecture.CodeAnalysis;

public abstract class JsonKeyedSerializerCodeGeneratorFactoryBase(bool isForObjectFactories) : IKeyedSerializerCodeGeneratorFactory
{
   public abstract string CodeGeneratorName { get; }

   public bool MustGenerateCode(KeyedSerializerGeneratorState state)
   {
      if (state.AttributeInfo.HasJsonConverterAttribute
          || !state.SerializationFrameworks.HasSerializationFramework(SerializationFrameworks.SystemTextJson))
         return false;

      // A ref struct cannot be a generic argument of the generated converter. ReadOnlySpan<char> is the
      // exception: the span-based JSON converter handles it. Every other ref struct is ignored here.
      var hasObjectFactory = state.AttributeInfo.ObjectFactories.Any(static f => (!f.IsRefLike || f.IsReadOnlySpanOfChar) && f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.SystemTextJson));

      if (isForObjectFactories)
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
