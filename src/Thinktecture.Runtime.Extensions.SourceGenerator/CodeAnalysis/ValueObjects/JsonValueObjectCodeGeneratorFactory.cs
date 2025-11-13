using System.Runtime.CompilerServices;
using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class JsonValueObjectCodeGeneratorFactory
   : JsonKeyedSerializerCodeGeneratorFactoryBase,
     IValueObjectSerializerCodeGeneratorFactory,
     IEquatable<JsonValueObjectCodeGeneratorFactory>
{
   public static readonly IValueObjectSerializerCodeGeneratorFactory Instance = new JsonValueObjectCodeGeneratorFactory();

   public override string CodeGeneratorName => "SystemTextJson-ValueObject-CodeGenerator";

   private JsonValueObjectCodeGeneratorFactory()
      : base(false)
   {
   }

   public bool MustGenerateCode(ComplexSerializerGeneratorState<ComplexValueObjectSourceGeneratorState> state)
   {
      return !state.AttributeInfo.HasJsonConverterAttribute
             && state.SerializationFrameworks.HasSerializationFramework(SerializationFrameworks.SystemTextJson)
             && !state.AttributeInfo.ObjectFactories.Any(static f => f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.SystemTextJson));
   }

   public CodeGeneratorBase Create(ComplexSerializerGeneratorState<ComplexValueObjectSourceGeneratorState> state, StringBuilder stringBuilder)
   {
      return new ComplexValueObjectJsonCodeGenerator<ComplexValueObjectSourceGeneratorState>(state.Type, state.AssignableInstanceFieldsAndProperties, stringBuilder);
   }

   public bool Equals(IValueObjectSerializerCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public bool Equals(ICodeGeneratorFactory<ComplexSerializerGeneratorState<ComplexValueObjectSourceGeneratorState>> other) => ReferenceEquals(this, other);
   public bool Equals(IComplexSerializerCodeGeneratorFactory other) => ReferenceEquals(this, other);
   public bool Equals(JsonValueObjectCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public override bool Equals(object? obj) => ReferenceEquals(this, obj);

   public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
}
