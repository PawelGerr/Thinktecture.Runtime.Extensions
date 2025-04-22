using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class JsonValueObjectCodeGeneratorFactory : IValueObjectSerializerCodeGeneratorFactory
{
   public static readonly IValueObjectSerializerCodeGeneratorFactory Instance = new JsonValueObjectCodeGeneratorFactory();

   public string CodeGeneratorName => "SystemTextJson-ValueObject-CodeGenerator";

   private JsonValueObjectCodeGeneratorFactory()
   {
   }

   public bool MustGenerateCode(KeyedSerializerGeneratorState state)
   {
      return !state.AttributeInfo.HasJsonConverterAttribute
             && state.SerializationFrameworks.HasFlag(SerializationFrameworks.SystemTextJson)
             && (state.KeyMember is not null || state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization.Has(SerializationFrameworks.SystemTextJson)));
   }

   public bool MustGenerateCode(ComplexSerializerGeneratorState state)
   {
      return !state.AttributeInfo.HasJsonConverterAttribute
             && state.SerializationFrameworks.HasFlag(SerializationFrameworks.SystemTextJson)
             && !state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization.Has(SerializationFrameworks.SystemTextJson));
   }

   public CodeGeneratorBase Create(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new KeyedJsonCodeGenerator(state, stringBuilder);
   }

   public CodeGeneratorBase Create(ComplexSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new ComplexValueObjectJsonCodeGenerator(state.Type, state.AssignableInstanceFieldsAndProperties, stringBuilder);
   }

   public bool Equals(IValueObjectSerializerCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public bool Equals(ICodeGeneratorFactory<KeyedSerializerGeneratorState> other) => ReferenceEquals(this, other);
   public bool Equals(ICodeGeneratorFactory<ComplexSerializerGeneratorState> other) => ReferenceEquals(this, other);
   public bool Equals(IKeyedSerializerCodeGeneratorFactory other) => ReferenceEquals(this, other);
   public bool Equals(IComplexSerializerCodeGeneratorFactory other) => ReferenceEquals(this, other);
}
