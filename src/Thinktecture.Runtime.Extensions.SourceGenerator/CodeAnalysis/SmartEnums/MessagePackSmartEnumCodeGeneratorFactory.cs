using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class MessagePackSmartEnumCodeGeneratorFactory : IKeyedSerializerCodeGeneratorFactory
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new MessagePackSmartEnumCodeGeneratorFactory();

   private MessagePackSmartEnumCodeGeneratorFactory()
   {
   }

   public bool MustGenerateCode(AttributeInfo attributeInfo)
   {
      return !attributeInfo.HasMessagePackFormatterAttribute;
   }

   public CodeGeneratorBase Create(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new KeyedMessagePackCodeGenerator(state.Type, state.KeyMember, stringBuilder);
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
