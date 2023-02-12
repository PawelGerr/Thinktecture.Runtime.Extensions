using System.Reflection;

namespace Thinktecture;

/// <summary>
/// Common interface of complex value objects.
/// </summary>
public interface IComplexValueObject
{
   /// <summary>
   /// Gets assignable members.
   /// </summary>
   /// <returns>Assignable members.</returns>
   static abstract IReadOnlyList<MemberInfo> GetAssignableMembers();
}
