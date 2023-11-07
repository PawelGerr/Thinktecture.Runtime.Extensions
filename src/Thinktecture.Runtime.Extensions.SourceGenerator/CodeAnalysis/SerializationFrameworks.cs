namespace Thinktecture.CodeAnalysis;

[Flags]
public enum SerializationFrameworks
{
   None = 0,
   SystemTextJson = 1 << 0,
   NewtonsoftJson = 1 << 1,
   Json = SystemTextJson | NewtonsoftJson,
   MessagePack = 1 << 2,
   All = Json | MessagePack
}
