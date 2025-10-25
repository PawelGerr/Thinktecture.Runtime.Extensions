using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class SerializationFrameworksExtensions
{
   public static bool HasSerializationFramework(this SerializationFrameworks value, SerializationFrameworks flag)
   {
      return flag != SerializationFrameworks.None && (value & flag) == flag;
   }
}
