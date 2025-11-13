using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class SerializationFrameworksExtensions
{
   public static bool HasSerializationFramework(this SerializationFrameworks value, SerializationFrameworks serializationFrameworkToCheckFor)
   {
      return serializationFrameworkToCheckFor != SerializationFrameworks.None
             && (value & serializationFrameworkToCheckFor) == serializationFrameworkToCheckFor;
   }
}
