using System.Reflection;

namespace Thinktecture.Internal;

/// <summary>
/// For internal use only.
/// </summary>
public sealed class ComplexValueObjectMetadata
{
   /// <summary>
   /// The type of the value object.
   /// </summary>
   public Type Type { get; }

   /// <summary>
   /// Assignable members of the value object.
   /// </summary>
   public IReadOnlyList<MemberInfo> AssignableMembers { get; }

   /// <summary>
   /// Initializes new instance of <see cref="ComplexValueObjectMetadata"/>.
   /// </summary>
   /// <param name="type">The type of the value object.</param>
   /// <param name="assignableMembers">Assignable members of the value object.</param>
   public ComplexValueObjectMetadata(
      Type type,
      IReadOnlyList<MemberInfo> assignableMembers)
   {
      Type = type;
      AssignableMembers = assignableMembers;
   }
}
