#if COMPLEX_TYPES

using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Thinktecture;

internal static class MutableTypeBaseExtensions
{
   public static IMutableComplexProperty? FindComplexPropertyFix(
      this IMutableTypeBase type,
      MemberInfo memberInfo)
   {
      return type switch
      {
         IReadOnlyEntityType entityType => (IMutableComplexProperty?)entityType.FindComplexProperty(memberInfo),
         IReadOnlyComplexType complexType => (IMutableComplexProperty?)complexType.FindComplexProperty(memberInfo),
         _ => type.FindComplexProperty(memberInfo)
      };
   }
}

#endif
