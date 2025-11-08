namespace Thinktecture;

/// <summary>
/// Provides extension methods for the <see cref="SerializationFrameworks"/> enum.
/// </summary>
public static class SerializationFrameworksExtensions
{
   /// <summary>
   /// Checks whether the specified <see cref="SerializationFrameworks"/> value includes a given serialization framework.
   /// </summary>
   /// <param name="value">The <see cref="SerializationFrameworks"/> value to be checked.</param>
   /// <param name="serializationFrameworkToCheckFor">The serialization framework to check for in the value.</param>
   /// <returns>True if the specified framework is included in the provided value; otherwise, false.</returns>
   public static bool HasSerializationFramework(
      this SerializationFrameworks value,
      SerializationFrameworks serializationFrameworkToCheckFor)
   {
      return (value & serializationFrameworkToCheckFor) == serializationFrameworkToCheckFor;
   }
}
