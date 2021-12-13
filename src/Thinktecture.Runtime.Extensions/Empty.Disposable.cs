using System;
using System.Threading.Tasks;

namespace Thinktecture;

public partial class Empty
{
   /// <summary>
   /// Creates a new new <see cref="IDisposable"/> with empty method body.
   /// </summary>
   /// <returns>An <see cref="IDisposable"/> with empty method body.</returns>
   public static IDisposable Disposable()
   {
      return new EmptyDisposable();
   }

   private readonly struct EmptyDisposable : IDisposable
   {
      public void Dispose()
      {
      }
   }

   /// <summary>
   /// Creates a new new <see cref="IAsyncDisposable"/> with empty method body.
   /// </summary>
   /// <returns>An <see cref="IAsyncDisposable"/> with empty method body.</returns>
   public static IAsyncDisposable AsyncDisposable()
   {
      return new EmptyAsyncDisposable();
   }

   private struct EmptyAsyncDisposable : IAsyncDisposable
   {
      public ValueTask DisposeAsync()
      {
         return default;
      }
   }
}