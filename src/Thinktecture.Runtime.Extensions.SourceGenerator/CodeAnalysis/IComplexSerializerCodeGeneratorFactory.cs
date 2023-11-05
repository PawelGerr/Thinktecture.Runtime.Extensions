using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.CodeAnalysis;

public interface IComplexSerializerCodeGeneratorFactory : ICodeGeneratorFactory<ComplexSerializerGeneratorState>, IEquatable<IComplexSerializerCodeGeneratorFactory>
{
   bool MustGenerateCode(ComplexSerializerGeneratorState state);
}
