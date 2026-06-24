using System.Linq.Expressions;

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
   public sealed class AdHocUnion : Metadata
   {
      /// <summary>
      /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
      /// the same compatibility standards as public APIs. It may be changed or removed without notice in
      /// any release. You should only use it directly in your code with extreme caution and knowing that
      /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
      /// </summary>
      public required IReadOnlyList<Type> MemberTypes { get; init; }

      /// <summary>
      /// Typed delegate (<c>Func&lt;TUnion, TValue&gt;</c>) for retrieving the current value of the union.
      /// </summary>
      public required Delegate ConvertToValue { get; init; }

      /// <summary>
      /// An expression <c>(TUnion u) =&gt; u.Value</c> for retrieving the current value of the union.
      /// </summary>
      public required LambdaExpression ConvertToValueExpression { get; init; }

      /// <summary>
      /// Gets the current (boxed) value of the union. The result is <c>null</c> when the active member is a null reference.
      /// </summary>
      public required Func<object, object?> GetValue { get; init; }

      /// <summary>
      /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
      /// the same compatibility standards as public APIs. It may be changed or removed without notice in
      /// any release. You should only use it directly in your code with extreme caution and knowing that
      /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
      /// </summary>
      public AdHocUnion(Type type)
         : base(type)
      {
      }
   }
}
