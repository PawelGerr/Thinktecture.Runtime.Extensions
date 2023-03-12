namespace Thinktecture.CodeAnalysis;

public interface IKeyedSerializerCodeGeneratorFactory : ICodeGeneratorFactory<KeyedSerializerGeneratorState>, IEquatable<IKeyedSerializerCodeGeneratorFactory>
{
   bool MustGenerateCode(AttributeInfo attributeInfo);
}
