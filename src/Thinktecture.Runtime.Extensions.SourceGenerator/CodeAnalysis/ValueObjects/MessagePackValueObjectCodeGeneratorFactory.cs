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

   public bool MustGenerateCode(ComplexSerializerGeneratorState state)
   {
      return !state.AttributeInfo.HasMessagePackFormatterAttribute
             && state.SerializationFrameworks.HasFlag(SerializationFrameworks.MessagePack)
             && !state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization.Has(SerializationFrameworks.MessagePack));
   }

   public CodeGeneratorBase Create(ComplexSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new ComplexValueObjectMessagePackCodeGenerator(state.Type, state.AssignableInstanceFieldsAndProperties, stringBuilder);
   }

   public bool Equals(IValueObjectSerializerCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public bool Equals(ICodeGeneratorFactory<ComplexSerializerGeneratorState> other) => ReferenceEquals(this, other);
   public bool Equals(IComplexSerializerCodeGeneratorFactory other) => ReferenceEquals(this, other);
}
