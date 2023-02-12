using System.Linq.Expressions;

namespace Thinktecture.Internal;

/// <summary>
/// For internal use only.
/// </summary>
public sealed class ValueObjectMetadata
{
   /// <summary>
   /// The type of the Value Object or a Smart Enum.
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
   /// An expression for conversion of values of type <see cref="KeyType"/> to type <see cref="Type"/> using the constructor.
   /// This delegate is for use with value objects when the key-value comes from a valid source.
   /// </summary>
   public LambdaExpression? ConvertFromKeyExpressionViaConstructor { get; }

   /// <summary>
   /// A delegate for conversion of values of type <see cref="Type"/> to type <see cref="KeyType"/>.
   /// </summary>
   public Delegate ConvertToKey { get; }

   /// <summary>
   /// An expression for conversion of values of type <see cref="Type"/> to type <see cref="KeyType"/>.
   /// </summary>
   public LambdaExpression ConvertToKeyExpression { get; }

   /// <summary>
   /// An indication whether the type implements <see cref="IEnum{TKey}"/> or <see cref="IValidatableEnum{TKey}"/>.
   /// </summary>
   public bool IsEnumeration { get; }

   /// <summary>
   /// An indication whether the type implements <see cref="IValidatableEnum{TKey}"/>.
   /// </summary>
   public bool IsValidatableEnum { get; }

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectMetadata"/>.
   /// </summary>
   /// <param name="type">The type of the value object or a smart enum.</param>
   /// <param name="keyType">The type of the key property.</param>
   /// <param name="isEnumeration">An indication whether the type implements <see cref="IEnum{TKey}"/> or <see cref="IValidatableEnum{TKey}"/>.</param>
   /// <param name="isValidatableEnum">An indication whether the type implements <see cref="IValidatableEnum{TKey}"/>.</param>
   /// <param name="convertFromKey">A delegate for conversion of values of type <paramref name="keyType"/> to type <paramref name="type"/>.</param>
   /// <param name="convertFromKeyExpression">An expression for conversion of values of type <paramref name="keyType"/> to type <paramref name="type"/>.</param>
   /// <param name="convertFromKeyExpressionViaConstructor">
   /// An expression for conversion of values of type <see cref="KeyType"/> to type <see cref="Type"/> using the constructor.
   /// This delegate is for use with value objects when the key-value comes from a valid source.
   /// </param>
   /// <param name="convertToKey">A delegate for conversion of values of type <paramref name="type"/> to type <paramref name="keyType"/>.</param>
   /// <param name="convertToKeyExpression">An expression for conversion of values of type <paramref name="type"/> to type <paramref name="keyType"/>.</param>
   public ValueObjectMetadata(
      Type type,
      Type keyType,
      bool isEnumeration,
      bool isValidatableEnum,
      Delegate convertFromKey,
      LambdaExpression convertFromKeyExpression,
      LambdaExpression? convertFromKeyExpressionViaConstructor,
      Delegate convertToKey,
      LambdaExpression convertToKeyExpression)
   {
      Type = type ?? throw new ArgumentNullException(nameof(type));
      KeyType = keyType ?? throw new ArgumentNullException(nameof(keyType));
      IsEnumeration = isEnumeration;
      IsValidatableEnum = isValidatableEnum;
      ConvertFromKey = convertFromKey ?? throw new ArgumentNullException(nameof(convertFromKey));
      ConvertFromKeyExpression = convertFromKeyExpression ?? throw new ArgumentNullException(nameof(convertFromKeyExpression));
      ConvertFromKeyExpressionViaConstructor = convertFromKeyExpressionViaConstructor;
      ConvertToKey = convertToKey ?? throw new ArgumentNullException(nameof(convertToKey));
      ConvertToKeyExpression = convertToKeyExpression ?? throw new ArgumentNullException(nameof(convertToKeyExpression));
   }
}
