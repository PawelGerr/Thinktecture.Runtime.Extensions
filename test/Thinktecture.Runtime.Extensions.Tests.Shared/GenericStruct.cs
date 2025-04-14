using MessagePack;

namespace Thinktecture.Runtime.Tests;

[MessagePackObject]
public record struct GenericStruct<T>(
   [property: Key(1)] T Property);
