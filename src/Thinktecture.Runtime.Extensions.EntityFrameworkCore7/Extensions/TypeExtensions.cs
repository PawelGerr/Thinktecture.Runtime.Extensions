using System.Reflection;

namespace Thinktecture;

internal static class TypeExtensions
{
   public static bool TryGetAssignableMembers(this Type type, out IReadOnlyList<MemberInfo> members)
   {
      if (!typeof(IComplexValueObject).IsAssignableFrom(type))
      {
         members = Array.Empty<MemberInfo>();
         return false;
      }

      var interfaceMap = type.GetInterfaceMap(typeof(IComplexValueObject));

      for (var i = 0; i < interfaceMap.InterfaceMethods.Length; i++)
      {
         var interfaceMethod = interfaceMap.InterfaceMethods[i];

         if (interfaceMethod.Name == nameof(IComplexValueObject.GetAssignableMembers))
         {
            members = (IReadOnlyList<MemberInfo>?)interfaceMap.TargetMethods[i].Invoke(null, Array.Empty<object>())
                      ?? throw new Exception($"The method '{nameof(IComplexValueObject.GetAssignableMembers)}' of type '{type.FullName}' must not return null.");

            return true;
         }
      }

      members = Array.Empty<MemberInfo>();
      return false;
   }
}
