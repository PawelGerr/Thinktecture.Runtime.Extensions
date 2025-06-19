namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class JsonSmartEnumCodeGeneratorFactory : JsonKeyedSerializerCodeGeneratorFactoryBase
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new JsonSmartEnumCodeGeneratorFactory();

   public override string CodeGeneratorName => "SystemTextJson-SmartEnum-CodeGenerator";

   private JsonSmartEnumCodeGeneratorFactory()
      : base(false)
   {
   }
}
