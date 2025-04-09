namespace Thinktecture;

/// <summary>
/// Serialization frameworks.
/// </summary>
[Flags]
public enum SerializationFrameworks
{
   /// <summary>
   /// No serialization frameworks
   /// </summary>
   None = 0,

   /// <summary>
   /// System.Text.Json
   /// </summary>
   SystemTextJson = 1 << 0,

   /// <summary>
   /// Newtonsoft.Json
   /// </summary>
   NewtonsoftJson = 1 << 1,

   /// <summary>
   /// System.Text.Json and Newtonsoft.Json
   /// </summary>
   Json = SystemTextJson | NewtonsoftJson,

   /// <summary>
   /// MessagePack
   /// </summary>
   MessagePack = 1 << 2,

   /// <summary>
   /// ProtoBuf
   /// </summary>
   ProtoBuf = 1 << 3,

   /// <summary>
   /// System.Text.Json, Newtonsoft.Json, MessagePack and ProtoBuf
   /// </summary>
   All = Json | MessagePack | ProtoBuf
}
