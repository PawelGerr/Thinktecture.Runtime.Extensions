using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class NewtonsoftJsonValueObjectCodeGeneratorFactory : NewtonsoftJsonKeyedSerializerCodeGeneratorFactoryBase, IValueObjectSerializerCodeGeneratorFactory
{
   public static readonly IValueObjectSerializerCodeGeneratorFactory Instance = new NewtonsoftJsonValueObjectCodeGeneratorFactory();

   public override string CodeGeneratorName => "NewtonsoftJson-ValueObject-CodeGenerator";

   private NewtonsoftJsonValueObjectCodeGeneratorFactory()
      : base(false)
   {
   }

   public bool MustGenerateCode(ComplexSerializerGeneratorState<ComplexValueObjectSourceGeneratorState> state)
   {
      return !state.AttributeInfo.HasNewtonsoftJsonConverterAttribute
             && state.SerializationFrameworks.HasSerializationFramework(SerializationFrameworks.NewtonsoftJson)
             && !state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.NewtonsoftJson));
   }

   public CodeGeneratorBase Create(ComplexSerializerGeneratorState<ComplexValueObjectSourceGeneratorState> state, StringBuilder stringBuilder)
   {
      return new ComplexValueObjectNewtonsoftJsonCodeGenerator<ComplexValueObjectSourceGeneratorState>(state.Type, state.AssignableInstanceFieldsAndProperties, stringBuilder);
   }

   public bool Equals(IValueObjectSerializerCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public bool Equals(ICodeGeneratorFactory<ComplexSerializerGeneratorState<ComplexValueObjectSourceGeneratorState>> other) => ReferenceEquals(this, other);
   public bool Equals(IComplexSerializerCodeGeneratorFactory other) => ReferenceEquals(this, other);
}
