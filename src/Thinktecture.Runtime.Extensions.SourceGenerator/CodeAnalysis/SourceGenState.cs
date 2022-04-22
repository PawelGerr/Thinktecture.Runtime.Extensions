namespace Thinktecture.CodeAnalysis;

public record struct SourceGenState<T>(T? State, Exception? Exception)
   where T : IEquatable<T>;
