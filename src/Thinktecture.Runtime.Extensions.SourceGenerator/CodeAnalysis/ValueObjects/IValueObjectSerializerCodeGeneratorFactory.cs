namespace Thinktecture.CodeAnalysis.ValueObjects;

public interface IValueObjectSerializerCodeGeneratorFactory
   : IKeyedSerializerCodeGeneratorFactory,
     IComplexSerializerCodeGeneratorFactory,
     IEquatable<IValueObjectSerializerCodeGeneratorFactory>
{
}
