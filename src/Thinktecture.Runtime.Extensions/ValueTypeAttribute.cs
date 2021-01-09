using System;

namespace Thinktecture
{
   /// <summary>
   /// Marks the type as a value type.
   /// </summary>
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
   public class ValueTypeAttribute : Attribute
   {
   }
}
