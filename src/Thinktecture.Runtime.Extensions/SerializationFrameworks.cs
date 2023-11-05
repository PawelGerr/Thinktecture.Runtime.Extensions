namespace Thinktecture;

/// <summary>
/// Points to serialization frameworks.
/// </summary>
[Flags]
public enum SerializationFrameworks
{
   /// <summary>
   /// No serialization frameworks
   /// </summary>
   None = 0,

   /// <summary>
   /// Points to System.Text.Json
   /// </summary>
   SystemTextJson = 1 << 0,

   /// <summary>
   /// Points to Newtonsoft.Json
   /// </summary>
   NewtonsoftJson = 1 << 1,

   /// <summary>
   /// Points to System.Text.Json and Newtonsoft.Json
   /// </summary>
   Json = SystemTextJson | NewtonsoftJson,

   /// <summary>
   /// Points to MessagePack
   /// </summary>
   MessagePack = 1 << 2,

   /// <summary>
   /// Points to System.Text.Json, Newtonsoft.Json and MessagePack
   /// </summary>
   All = Json | MessagePack
}
