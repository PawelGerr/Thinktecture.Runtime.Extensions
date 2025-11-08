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

   public bool MustGenerateCode(ComplexSerializerGeneratorState state)
   {
      return !state.AttributeInfo.HasNewtonsoftJsonConverterAttribute
             && state.SerializationFrameworks.HasSerializationFramework(SerializationFrameworks.NewtonsoftJson)
             && !state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.NewtonsoftJson));
   }

   public CodeGeneratorBase Create(ComplexSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new ComplexValueObjectNewtonsoftJsonCodeGenerator(state.Type, state.AssignableInstanceFieldsAndProperties, stringBuilder);
   }

   public bool Equals(IValueObjectSerializerCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public bool Equals(ICodeGeneratorFactory<ComplexSerializerGeneratorState> other) => ReferenceEquals(this, other);
   public bool Equals(IComplexSerializerCodeGeneratorFactory other) => ReferenceEquals(this, other);
}
