namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class MessagePackSmartEnumCodeGeneratorFactory : MessagePackKeyedSerializerCodeGeneratorFactoryBase
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new MessagePackSmartEnumCodeGeneratorFactory();

   public override string CodeGeneratorName => "MessagePack-SmartEnum-CodeGenerator";

   private MessagePackSmartEnumCodeGeneratorFactory()
   {
   }
}
