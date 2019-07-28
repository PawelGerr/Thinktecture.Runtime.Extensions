using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore.Query.ExpressionTranslators;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   /// <summary>
   /// Extension methods for <see cref="DbContextOptionsBuilder"/>.
   /// </summary>
   public static class DbContextOptionsBuilderExtensions
   {
      /// <summary>
      /// Adds support for <see cref="Enum{TEnum,TKey}"/> to Entity Framework Core.
      /// </summary>
      /// <param name="builder">Options builder.</param>
      /// <typeparam name="T">Type of the <see cref="DbContext"/>.</typeparam>
      /// <returns>Options builder for chaining.</returns>
      /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
      [NotNull]
      public static DbContextOptionsBuilder<T> AddEnumSupport<T>([NotNull] this DbContextOptionsBuilder<T> builder)
         where T : DbContext
      {
         ((DbContextOptionsBuilder)builder).AddEnumSupport();
         return builder;
      }

      /// <summary>
      /// Adds support for <see cref="Enum{TEnum,TKey}"/> to Entity Framework Core.
      /// </summary>
      /// <param name="builder">Options builder.</param>
      /// <returns>Options builder for chaining.</returns>
      /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
      [NotNull]
      public static DbContextOptionsBuilder AddEnumSupport([NotNull] this DbContextOptionsBuilder builder)
      {
         if (builder == null)
            throw new ArgumentNullException(nameof(builder));

         return builder.AddExpressionFragmentTranslator(new EnumTranslator());
      }
   }
}
