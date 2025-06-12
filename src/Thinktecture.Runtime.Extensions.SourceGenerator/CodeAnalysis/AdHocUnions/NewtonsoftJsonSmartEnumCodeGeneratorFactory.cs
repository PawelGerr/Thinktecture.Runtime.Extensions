namespace Thinktecture.CodeAnalysis.AdHocUnions;

public sealed class NewtonsoftJsonAdHocUnionCodeGeneratorFactory : NewtonsoftJsonKeyedSerializerCodeGeneratorFactoryBase
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new NewtonsoftJsonAdHocUnionCodeGeneratorFactory();

   public override string CodeGeneratorName => "NewtonsoftJson-AdHocUnion-CodeGenerator";

   private NewtonsoftJsonAdHocUnionCodeGeneratorFactory()
   {
   }
}
