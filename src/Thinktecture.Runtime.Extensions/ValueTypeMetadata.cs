using System;
using System.Linq.Expressions;

namespace Thinktecture
{
   /// <summary>
   /// Value type metadata.
   /// </summary>
   public sealed class ValueTypeMetadata
   {
      /// <summary>
      /// The type of the value type.
      /// </summary>
      public Type Type { get; }

      /// <summary>
      /// The type of the key property.
      /// </summary>
      public Type KeyType { get; }

      /// <summary>
      /// A delegate for conversion of values of type <see cref="KeyType"/> to type <see cref="Type"/>.
      /// </summary>
      public Delegate ConvertFromKey { get; }

      /// <summary>
      /// An expression for conversion of values of type <see cref="KeyType"/> to type <see cref="Type"/>.
      /// </summary>
      public LambdaExpression ConvertFromKeyExpression { get; }

      /// <summary>
      /// A delegate for conversion of values of type <see cref="Type"/> to type <see cref="KeyType"/>.
      /// </summary>
      public Delegate ConvertToKey { get; }

      /// <summary>
      /// An expression for conversion of values of type <see cref="Type"/> to type <see cref="KeyType"/>.
      /// </summary>
      public LambdaExpression ConvertToKeyExpression { get; }

      /// <summary>
      /// Initializes new instance of <see cref="ValueTypeMetadata"/>.
      /// </summary>
      /// <param name="type">The type of the value type.</param>
      /// <param name="keyType">The type of the key property.</param>
      /// <param name="convertFromKey">A delegate for conversion of values of type <paramref name="keyType"/> to type <paramref name="type"/>.</param>
      /// <param name="convertFromKeyExpression">An expression for conversion of values of type <paramref name="keyType"/> to type <paramref name="type"/>.</param>
      /// <param name="convertToKey">A delegate for conversion of values of type <paramref name="type"/> to type <paramref name="keyType"/>.</param>
      /// <param name="convertToKeyExpression">An expression for conversion of values of type <paramref name="type"/> to type <paramref name="keyType"/>.</param>
      public ValueTypeMetadata(
         Type type,
         Type keyType,
         Delegate convertFromKey,
         LambdaExpression convertFromKeyExpression,
         Delegate convertToKey,
         LambdaExpression convertToKeyExpression)
      {
         Type = type ?? throw new ArgumentNullException(nameof(type));
         KeyType = keyType ?? throw new ArgumentNullException(nameof(keyType));
         ConvertFromKey = convertFromKey ?? throw new ArgumentNullException(nameof(convertFromKey));
         ConvertFromKeyExpression = convertFromKeyExpression ?? throw new ArgumentNullException(nameof(convertFromKeyExpression));
         ConvertToKey = convertToKey ?? throw new ArgumentNullException(nameof(convertToKey));
         ConvertToKeyExpression = convertToKeyExpression ?? throw new ArgumentNullException(nameof(convertToKeyExpression));
      }
   }
}
