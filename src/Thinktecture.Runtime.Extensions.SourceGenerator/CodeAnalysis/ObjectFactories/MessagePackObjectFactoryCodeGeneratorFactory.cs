namespace Thinktecture.CodeAnalysis.ObjectFactories;

public sealed class MessagePackObjectFactoryCodeGeneratorFactory : MessagePackKeyedSerializerCodeGeneratorFactoryBase
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new MessagePackObjectFactoryCodeGeneratorFactory();

   public override string CodeGeneratorName => "MessagePack-ObjectFactory-CodeGenerator";

   private MessagePackObjectFactoryCodeGeneratorFactory()
      : base(true)
   {
   }
}
