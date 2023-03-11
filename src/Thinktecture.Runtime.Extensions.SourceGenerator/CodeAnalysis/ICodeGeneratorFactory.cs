using System.Text;

namespace Thinktecture.CodeAnalysis;

public interface ICodeGeneratorFactory<T> : IEquatable<ICodeGeneratorFactory<T>>
{
   CodeGeneratorBase Create(T state, StringBuilder stringBuilder);
}
