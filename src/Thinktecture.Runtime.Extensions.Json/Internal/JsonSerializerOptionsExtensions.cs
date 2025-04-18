using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public static class JsonSerializerOptionsExtensions
{
   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public static JsonConverter GetCustomMemberConverter(this JsonSerializerOptions options, Type memberType)
   {
      if (options.NumberHandling != JsonNumberHandling.Strict)
      {
         var numberConverter = GetNumberConverter(memberType);

         if (numberConverter is not null)
            return numberConverter;
      }

      return options.GetConverter(memberType);
   }

   private static JsonConverter? GetNumberConverter(Type memberType)
   {
      if (memberType == typeof(byte))
         return ByteKeyConverter.Instance;

      if (memberType == typeof(sbyte))
         return SByteKeyConverter.Instance;

      if (memberType == typeof(short))
         return ShortKeyConverter.Instance;

      if (memberType == typeof(ushort))
         return UShortKeyConverter.Instance;

      if (memberType == typeof(int))
         return IntKeyConverter.Instance;

      if (memberType == typeof(uint))
         return UIntKeyConverter.Instance;

      if (memberType == typeof(long))
         return LongKeyConverter.Instance;

      if (memberType == typeof(ulong))
         return ULongKeyConverter.Instance;

      if (memberType == typeof(float))
         return SingleKeyConverter.Instance;

      if (memberType == typeof(double))
         return DoubleKeyConverter.Instance;

      if (memberType == typeof(decimal))
         return DecimalKeyConverter.Instance;

      return null;
   }
}
