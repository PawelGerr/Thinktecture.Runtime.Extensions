namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class NewtonsoftJsonSmartEnumCodeGeneratorFactory : NewtonsoftJsonKeyedSerializerCodeGeneratorFactoryBase
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new NewtonsoftJsonSmartEnumCodeGeneratorFactory();

   public override string CodeGeneratorName => "NewtonsoftJson-SmartEnum-CodeGenerator";

   private NewtonsoftJsonSmartEnumCodeGeneratorFactory()
      : base(false)
   {
   }
}
