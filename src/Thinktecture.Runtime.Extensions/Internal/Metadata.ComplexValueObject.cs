using System.Reflection;

namespace Thinktecture.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public abstract partial class Metadata
{
   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public sealed class ComplexValueObject : Metadata
   {
      /// <summary>
      /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
      /// the same compatibility standards as public APIs. It may be changed or removed without notice in
      /// any release. You should only use it directly in your code with extreme caution and knowing that
      /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
      /// </summary>
      public required IReadOnlyList<MemberInfo> AssignableMembers { get; init; }

      /// <summary>
      /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
      /// the same compatibility standards as public APIs. It may be changed or removed without notice in
      /// any release. You should only use it directly in your code with extreme caution and knowing that
      /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
      /// </summary>
      public ComplexValueObject(Type type)
         : base(type)
      {
      }
   }
}
