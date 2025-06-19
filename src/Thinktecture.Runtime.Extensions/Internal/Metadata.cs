using System.Linq.Expressions;
using System.Reflection;

namespace Thinktecture.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
[Union]
public abstract partial class Metadata
{
   /// <summary>
   /// The type of the metadata belongs to.
   /// </summary>
   public Type Type { get; }

   private readonly Lazy<IReadOnlyList<ObjectFactoryMetadata>> _objectFactories;

   /// <summary>
   /// Gets the object factories metadata.
   /// </summary>
   public IReadOnlyList<ObjectFactoryMetadata> ObjectFactories => _objectFactories.Value;

   private Metadata(Type type)
   {
      Type = type;
      _objectFactories = new(type.FindObjectFactoryMetadata);
   }

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   [Union]
   public abstract partial class Keyed : Metadata
   {
      /// <summary>
      /// The type of the key property.
      /// </summary>
      public required Type KeyType { get; init; }

      /// <summary>
      /// The type of the validation error.
      /// </summary>
      public required Type ValidationErrorType { get; init; }

      /// <summary>
      /// Typed delegate for conversion of values of type <see cref="Type"/> to type <see cref="KeyType"/>.
      /// </summary>
      public required Delegate ConvertToKey { get; init; }

      /// <summary>
      /// An expression for conversion of values of type <see cref="Type"/> to type <see cref="KeyType"/>.
      /// </summary>
      public required LambdaExpression ConvertToKeyExpression { get; init; }

      /// <summary>
      /// Gets the key of a keyed object.
      /// </summary>
      public required Func<object, object> GetKey { get; init; }

      private Keyed(Type type)
         : base(type)
      {
      }
   }
}
