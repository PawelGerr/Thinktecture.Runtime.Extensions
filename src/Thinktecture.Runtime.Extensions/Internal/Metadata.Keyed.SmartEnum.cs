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
      public sealed class SmartEnum : Keyed
      {
         /// <summary>
         /// Indicates whether span-based JSON conversion is disabled for this Smart Enum.
         /// </summary>
         public bool DisableSpanBasedJsonConversion { get; init; }

         /// <summary>
         /// A collection of items available in the current Smart Enum.
         /// </summary>
         public required Lazy<IReadOnlyList<SmartEnumItemMetadata>> Items { get; init; }

         /// <summary>
         /// Typed delegate for conversion of values of type <see cref="KeyType"/> to type <see cref="Type"/>.
         /// </summary>
         public required Delegate ConvertFromKey { get; init; }

         /// <summary>
         /// An expression for conversion of values of type <see cref="KeyType"/> to type <see cref="Type"/>.
         /// </summary>
         public required LambdaExpression ConvertFromKeyExpression { get; init; }

         /// <summary>
         /// Converts a key to a Smart Enum item and validates the key.
         /// </summary>
         public required TryGetFromKey TryGetFromKey { get; init; }

         /// <summary>
         /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
         /// the same compatibility standards as public APIs. It may be changed or removed without notice in
         /// any release. You should only use it directly in your code with extreme caution and knowing that
         /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
         /// </summary>
         public SmartEnum(Type type)
            : base(type)
         {
         }
      }
   }
}
