using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Thinktecture.EntityFrameworkCore.Query.Expressions
{
   /// <summary>
   /// Expression for translation of <see cref="Enum{TEnum,TKey}"/>.
   /// </summary>
   public class EnumExpression : Expression
   {
      private readonly Expression _key;

      /// <inheritdoc />
      public override Type Type { get; }

      /// <inheritdoc />
      public override ExpressionType NodeType => ExpressionType.Extension;

      /// <summary>
      /// Initializes new instance of <see cref="EnumExpression"/>.
      /// </summary>
      /// <param name="type">The type of the enum.</param>
      /// <param name="key">The key of the enum value.</param>
      public EnumExpression([NotNull] Type type, [NotNull] Expression key)
      {
         Type = type ?? throw new ArgumentNullException(nameof(type));
         _key = key ?? throw new ArgumentNullException(nameof(key));
      }

      /// <inheritdoc />
      [NotNull]
      protected override Expression Accept(ExpressionVisitor visitor)
      {
         var visitedKey = visitor.Visit(_key);
         return visitedKey == _key ? this : new EnumExpression(Type, _key);
      }
   }
}
