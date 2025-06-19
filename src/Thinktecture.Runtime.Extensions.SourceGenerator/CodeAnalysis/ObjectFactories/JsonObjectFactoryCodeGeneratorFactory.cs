namespace Thinktecture.CodeAnalysis.ObjectFactories;

public sealed class JsonObjectFactoryCodeGeneratorFactory : JsonKeyedSerializerCodeGeneratorFactoryBase
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new JsonObjectFactoryCodeGeneratorFactory();

   public override string CodeGeneratorName => "SystemTextJson-ObjectFactory-CodeGenerator";

   private JsonObjectFactoryCodeGeneratorFactory()
      : base(true)
   {
   }
}
