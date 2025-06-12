namespace Thinktecture.CodeAnalysis.AdHocUnions;

public sealed class JsonAdHocUnionCodeGeneratorFactory : JsonKeyedSerializerCodeGeneratorFactoryBase
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new JsonAdHocUnionCodeGeneratorFactory();

   public override string CodeGeneratorName => "SystemTextJson-AdHocUnion-CodeGenerator";

   private JsonAdHocUnionCodeGeneratorFactory()
   {
   }
}
