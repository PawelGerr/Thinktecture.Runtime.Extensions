using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Thinktecture.EntityFrameworkCore.Query.Expressions;

namespace Thinktecture.EntityFrameworkCore.Query.ExpressionTranslators
{
   /// <summary>
   /// Translates constant expression of type <see cref="Enum{TEnum,TKey}"/>.
   /// </summary>
   public class EnumTranslator : IExpressionFragmentTranslator
   {
      /// <inheritdoc />
      [CanBeNull]
      public Expression Translate(Expression expression)
      {
         // ReSharper disable once ConstantConditionalAccessQualifier
         if (expression?.NodeType == ExpressionType.Constant)
         {
            var constant = (ConstantExpression)expression;

            if (constant.Value is IEnum enumValue)
               return new EnumExpression(constant.Type, Expression.Constant(enumValue.Key));
         }

         return null;
      }
   }
}
