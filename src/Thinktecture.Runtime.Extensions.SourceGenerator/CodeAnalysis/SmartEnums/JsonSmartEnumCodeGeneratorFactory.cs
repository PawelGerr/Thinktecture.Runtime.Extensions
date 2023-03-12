using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class JsonSmartEnumCodeGeneratorFactory : IKeyedSerializerCodeGeneratorFactory
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new JsonSmartEnumCodeGeneratorFactory();

   private JsonSmartEnumCodeGeneratorFactory()
   {
   }

   public bool MustGenerateCode(AttributeInfo attributeInfo)
   {
      return !attributeInfo.HasJsonConverterAttribute;
   }

   public CodeGeneratorBase Create(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      return new KeyedJsonCodeGenerator(state.Type, state.KeyMember, stringBuilder);
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
