using System.Text;

namespace Thinktecture.CodeAnalysis;

public interface ICodeGeneratorFactory
{
   string CodeGeneratorName { get; }
}

public interface ICodeGeneratorFactory<T> : ICodeGeneratorFactory, IEquatable<ICodeGeneratorFactory<T>>
{
   CodeGeneratorBase Create(T state, StringBuilder stringBuilder);
}
