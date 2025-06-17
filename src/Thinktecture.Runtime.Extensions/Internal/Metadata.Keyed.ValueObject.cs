using System.Linq.Expressions;

namespace Thinktecture.Internal;

public abstract partial class Metadata
{
   public abstract partial class Keyed
   {
      /// <summary>
      /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
      /// the same compatibility standards as public APIs. It may be changed or removed without notice in
      /// any release. You should only use it directly in your code with extreme caution and knowing that
      /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
      /// </summary>
      public sealed class ValueObject : Keyed
      {
         /// <summary>
         /// An expression for conversion of values of type <see cref="KeyType"/> to type <see cref="Type"/> using the constructor.
         /// This delegate is for use with Value Objects when the key value comes from a valid source.
         /// </summary>
         public required LambdaExpression ConvertFromKeyExpressionViaConstructor { get; init; }

         /// <summary>
         /// Typed delegate for conversion of values of type <see cref="KeyType"/> to type <see cref="Type"/>.
         /// </summary>
         public required Delegate? ConvertFromKey { get; init; }

         /// <summary>
         /// An expression for conversion of values of type <see cref="KeyType"/> to type <see cref="Type"/>.
         /// </summary>
         public required LambdaExpression? ConvertFromKeyExpression { get; init; }

         /// <summary>
         /// Converts a key to a Value Object item and validates the key.
         /// </summary>
         public required TryGetFromKey? TryGetFromKey { get; init; }

         /// <summary>
         /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
         /// the same compatibility standards as public APIs. It may be changed or removed without notice in
         /// any release. You should only use it directly in your code with extreme caution and knowing that
         /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
         /// </summary>
         public ValueObject(Type type)
            : base(type)
         {
         }
      }
   }
}
