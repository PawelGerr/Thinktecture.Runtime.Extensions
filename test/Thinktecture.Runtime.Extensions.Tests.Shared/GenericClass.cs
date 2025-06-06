using MessagePack;

namespace Thinktecture.Runtime.Tests;

[MessagePackObject]
public record GenericClass<T>(
   [property: Key(0)] T Property);
