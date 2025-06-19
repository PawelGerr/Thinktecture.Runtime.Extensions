namespace Thinktecture.CodeAnalysis.ObjectFactories;

public sealed class NewtonsoftJsonObjectFactoryCodeGeneratorFactory : NewtonsoftJsonKeyedSerializerCodeGeneratorFactoryBase
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new NewtonsoftJsonObjectFactoryCodeGeneratorFactory();

   public override string CodeGeneratorName => "NewtonsoftJson-ObjectFactory-CodeGenerator";

   private NewtonsoftJsonObjectFactoryCodeGeneratorFactory()
      : base(true)
   {
   }
}
