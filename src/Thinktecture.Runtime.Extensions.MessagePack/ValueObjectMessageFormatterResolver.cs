using System;
using MessagePack;
using MessagePack.Formatters;
using Thinktecture.Formatters;
using Thinktecture.Internal;

namespace Thinktecture;

/// <summary>
/// MessagePack formatter resolver for value objects.
/// </summary>
public class ValueObjectMessageFormatterResolver : IFormatterResolver
{
   /// <summary>
   /// Singleton.
   /// </summary>
   public static readonly IFormatterResolver Instance = new ValueObjectMessageFormatterResolver();

   /// <inheritdoc />
   public IMessagePackFormatter<T>? GetFormatter<T>()
   {
      var formatter = Cache<T>.Formatter;

      if (formatter != null)
         return formatter;

      if (Cache<T>.InitError != null)
         throw new Exception(Cache<T>.InitError);

      return null;
   }

   private static class Cache<T>
   {
      public static readonly IMessagePackFormatter<T>? Formatter;

      // ReSharper disable once StaticMemberInGenericType
      public static readonly string? InitError;

#pragma warning disable CA1810
      static Cache()
      {
         var type = typeof(T);
         var metadata = ValueObjectMetadataLookup.Find(type);

         if (metadata is null)
            return;

         var formatterType = typeof(ValueObjectMessagePackFormatter<,>).MakeGenericType(metadata.Type, metadata.KeyType);
         var formatter = Activator.CreateInstance(formatterType, metadata.ConvertFromKey, metadata.ConvertToKey);

         if (formatter is null)
         {
            InitError = $"The formatter of '{formatterType.Name}' could not be instantiated.";
            return;
         }

         Formatter = (IMessagePackFormatter<T>)formatter;
      }
   }
}