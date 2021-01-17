using System.Text.Json;

namespace Thinktecture.Text.Json.Serialization
{
   /// <summary>
   /// Value read from JSON.
   /// </summary>
   /// <typeparam name="T">Type of the value.</typeparam>
   public readonly struct ValueTypeJsonValue<T>
   {
      private readonly T _value;
      private readonly bool _isRead;

      /// <summary>
      /// Initializes new instance of <see cref="ValueTypeJsonValue{T}"/>.
      /// </summary>
      /// <param name="value">Read value.</param>
      public ValueTypeJsonValue(T value)
      {
         _value = value;
         _isRead = true;
      }

      /// <summary>
      /// Gets previously read value.
      /// </summary>
      /// <param name="typeName">The type name of the value type.</param>
      /// <param name="memberName">Name of the member being deserialized.</param>
      /// <returns>Previously read value.</returns>
      /// <exception cref="JsonException">If the value was missing in the JSON message.</exception>
      public T GetValue(string typeName, string memberName)
      {
         if (_isRead)
            return _value;

         throw new JsonException($"Unable to deserialize '{typeName}'. The field/property {memberName} is missing in the JSON message.");
      }
   }
}
