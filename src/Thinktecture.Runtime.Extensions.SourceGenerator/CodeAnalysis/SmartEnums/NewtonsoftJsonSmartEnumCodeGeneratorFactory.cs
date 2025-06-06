using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class NewtonsoftJsonSmartEnumCodeGeneratorFactory : IKeyedSerializerCodeGeneratorFactory
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new NewtonsoftJsonSmartEnumCodeGeneratorFactory();

   public string CodeGeneratorName => "NewtonsoftJson-SmartEnum-CodeGenerator";

   private NewtonsoftJsonSmartEnumCodeGeneratorFactory()
   {
   }

   public bool MustGenerateCode(KeyedSerializerGeneratorState state)
   {
      return !state.AttributeInfo.HasNewtonsoftJsonConverterAttribute
             && state.SerializationFrameworks.HasFlag(SerializationFrameworks.NewtonsoftJson)
             && (state.KeyMember is not null || state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization.Has(SerializationFrameworks.NewtonsoftJson)));
   }

   public CodeGeneratorBase Create(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new KeyedNewtonsoftJsonCodeGenerator(state, stringBuilder);
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
