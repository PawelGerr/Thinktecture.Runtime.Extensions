namespace Thinktecture;

/// <summary>
/// The provided value doesn't match any enumeration item.
/// </summary>
public class UnknownSmartEnumIdentifierException : KeyNotFoundException
{
   /// <summary>
   /// Type of the enumeration.
   /// </summary>
   public Type EnumType { get; }

   /// <summary>
   /// Provided value.
   /// </summary>
   public object Value { get; }

   /// <summary>
   /// Initializes new instance of <see cref="UnknownSmartEnumIdentifierException"/>.
   /// </summary>
   /// <param name="enumType">The type of the enumeration.</param>
   /// <param name="value">Provided value.</param>
   public UnknownSmartEnumIdentifierException(Type enumType, object value)
      : base($"There is no item of type '{enumType.Name}' with the identifier '{value}'.")
   {
      EnumType = enumType;
      Value = value;
   }
}
