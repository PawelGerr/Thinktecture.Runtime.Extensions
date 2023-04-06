using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis;

public readonly record struct LoggingOptions(string FilePath, bool FilePathMustBeUnique, LogLevel Level, int InitialBufferSize);
