using System;
using System.Linq.Expressions;

namespace Thinktecture
{
   public class EnumMetadata
   {
      public Type EnumType { get; }
      public Type KeyType { get; }
      public Delegate ConvertFromKey { get; }
      public LambdaExpression ConvertFromKeyExpression { get; }

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
