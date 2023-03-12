namespace Thinktecture.CodeAnalysis.ValueObjects;

public interface IValueObjectSerializerCodeGeneratorFactory
   : IKeyedSerializerCodeGeneratorFactory,
     ICodeGeneratorFactory<ComplexSerializerGeneratorState>,
     IEquatable<IValueObjectSerializerCodeGeneratorFactory>
{
}
