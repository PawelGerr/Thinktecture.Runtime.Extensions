using System;

namespace Thinktecture
{
   /// <summary>
   /// Settings to be used by enum source generator.
   /// </summary>
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
   public class EnumGenerationAttribute : Attribute
   {
      /// <summary>
      /// The static member name containing the key equality comparer.
      /// </summary>
      public string? KeyComparer { get; set; }

      private string? _keyPropertyName;

      /// <summary>
      /// The name of the key property. Default name is 'Key'.
      /// </summary>
      public string KeyPropertyName
      {
         get => _keyPropertyName ?? "Key";
         set => _keyPropertyName = value;
      }

      /// <summary>
      /// Indication whether the enumeration should be to derive from.
      /// This feature comes with multiple restrictions, use it only if necessary!
      /// </summary>
      public bool IsExtensible { get; set; }
   }
}
