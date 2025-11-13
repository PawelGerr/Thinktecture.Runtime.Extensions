using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class MessagePackValueObjectCodeGeneratorFactory : MessagePackKeyedSerializerCodeGeneratorFactoryBase, IValueObjectSerializerCodeGeneratorFactory
{
   public static readonly IValueObjectSerializerCodeGeneratorFactory Instance = new MessagePackValueObjectCodeGeneratorFactory();

   public override string CodeGeneratorName => "MessagePack-ValueObject-CodeGenerator";

   private MessagePackValueObjectCodeGeneratorFactory()
      : base(false)
   {
   }

   public bool MustGenerateCode(ComplexSerializerGeneratorState<ComplexValueObjectSourceGeneratorState> state)
   {
      return !state.AttributeInfo.HasMessagePackFormatterAttribute
             && state.SerializationFrameworks.HasSerializationFramework(SerializationFrameworks.MessagePack)
             && !state.AttributeInfo.ObjectFactories.Any(static f => f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.MessagePack));
   }

   public CodeGeneratorBase Create(ComplexSerializerGeneratorState<ComplexValueObjectSourceGeneratorState> state, StringBuilder stringBuilder)
   {
      return new ComplexValueObjectMessagePackCodeGenerator<ComplexValueObjectSourceGeneratorState>(state.Type, state.AssignableInstanceFieldsAndProperties, stringBuilder);
   }

   public bool Equals(IValueObjectSerializerCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public bool Equals(ICodeGeneratorFactory<ComplexSerializerGeneratorState<ComplexValueObjectSourceGeneratorState>> other) => ReferenceEquals(this, other);
   public bool Equals(IComplexSerializerCodeGeneratorFactory other) => ReferenceEquals(this, other);
}
