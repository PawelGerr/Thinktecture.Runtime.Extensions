using System;
using System.Linq.Expressions;

namespace Thinktecture
{
   /// <summary>
   /// Metadata of an implementation of <see cref="IEnum{TKey}"/>.
   /// </summary>
   public sealed class EnumMetadata
   {
      /// <summary>
      /// The type of the enumeration.
      /// </summary>
      public Type EnumType { get; }

      /// <summary>
      /// The type of the key property.
      /// </summary>
      public Type KeyType { get; }

      /// <summary>
      /// A delegate for conversion of values of type <see cref="KeyType"/> to enumeration of type <see cref="EnumType"/>.
      /// </summary>
      public Delegate ConvertFromKey { get; }

      /// <summary>
      /// An expression for conversion of values of type <see cref="KeyType"/> to enumeration of type <see cref="EnumType"/>.
      /// </summary>
      public LambdaExpression ConvertFromKeyExpression { get; }

      /// <summary>
      /// Initializes new instance of <see cref="EnumMetadata"/>.
      /// </summary>
      /// <param name="enumType">The type of the enumeration.</param>
      /// <param name="keyType">The type of the key property.</param>
      /// <param name="convertFromKey">A delegate for conversion of values of type <paramref name="keyType"/> to enumeration of type <paramref name="enumType"/>.</param>
      /// <param name="convertFromKeyExpression">An expression for conversion of values of type <paramref name="keyType"/> to enumeration of type <paramref name="enumType"/>.</param>
      public EnumMetadata(
         Type enumType,
         Type keyType,
         Delegate convertFromKey,
         LambdaExpression convertFromKeyExpression)
      {
         EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
         KeyType = keyType ?? throw new ArgumentNullException(nameof(keyType));
         ConvertFromKey = convertFromKey ?? throw new ArgumentNullException(nameof(convertFromKey));
         ConvertFromKeyExpression = convertFromKeyExpression ?? throw new ArgumentNullException(nameof(convertFromKeyExpression));
      }
   }
}
