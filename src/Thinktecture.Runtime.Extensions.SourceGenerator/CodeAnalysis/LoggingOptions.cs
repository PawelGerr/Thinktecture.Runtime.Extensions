using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis;

public record struct LoggingOptions(string FilePath, LogLevel Level, int InitialBufferSize);
