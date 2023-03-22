using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis;

public readonly record struct LoggingOptions(string FilePath, LogLevel Level, int InitialBufferSize);
