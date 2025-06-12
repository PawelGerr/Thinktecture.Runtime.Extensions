namespace Thinktecture.CodeAnalysis.AdHocUnions;

public sealed class MessagePackAdHocUnionCodeGeneratorFactory : MessagePackKeyedSerializerCodeGeneratorFactoryBase
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new MessagePackAdHocUnionCodeGeneratorFactory();

   public override string CodeGeneratorName => "MessagePack-AdHocUnion-CodeGenerator";

   private MessagePackAdHocUnionCodeGeneratorFactory()
   {
   }
}
