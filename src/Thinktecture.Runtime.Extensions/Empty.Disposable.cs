namespace Thinktecture;

public partial class Empty
{
   /// <summary>
   /// Creates a new <see cref="IDisposable"/> with empty method body.
   /// </summary>
   /// <returns>An <see cref="IDisposable"/> with empty method body.</returns>
   public static IDisposable Disposable()
   {
      return EmptyDisposable.Instance;
   }

   private sealed class EmptyDisposable : IDisposable
   {
      public static readonly IDisposable Instance = new EmptyDisposable();

      public void Dispose()
      {
      }
   }

   /// <summary>
   /// Creates a new <see cref="IAsyncDisposable"/> with empty method body.
   /// </summary>
   /// <returns>An <see cref="IAsyncDisposable"/> with empty method body.</returns>
   public static IAsyncDisposable AsyncDisposable()
   {
      return EmptyAsyncDisposable.Instance;
   }

   private sealed class EmptyAsyncDisposable : IAsyncDisposable
   {
      public static readonly IAsyncDisposable Instance = new EmptyAsyncDisposable();

      public ValueTask DisposeAsync()
      {
         return default;
      }
   }
}
