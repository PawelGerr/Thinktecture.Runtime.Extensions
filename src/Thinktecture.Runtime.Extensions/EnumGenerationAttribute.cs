namespace Thinktecture;

/// <summary>
/// Settings to be used by enum source generator.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class EnumGenerationAttribute : Attribute
{
#if !NET7_0
   /// <summary>
   /// The static member name containing the key equality comparer.
   /// </summary>
   public string? KeyComparer { get; set; }
#endif

   private string? _keyPropertyName;

   /// <summary>
   /// The name of the key property. Default name is 'Key'.
   /// </summary>
   public string KeyPropertyName
   {
      get => _keyPropertyName ?? "Key";
      set => _keyPropertyName = value;
   }
}
