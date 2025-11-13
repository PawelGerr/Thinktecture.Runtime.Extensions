using System.Runtime.CompilerServices;

namespace Thinktecture.Internal
{
   /// <summary>
   /// Minimal, allocation-free write-once box for an int optimized for fast read access.
   /// </summary>
   public sealed class WriteOnceInt
   {
      // -1 => not set; >= 0 => set
      private int _value = -1;

      /// <summary>
      /// Fast non-volatile read for hot path. Returns -1 if not yet set.
      /// </summary>
      public int Value
      {
         [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _value;
      }

      /// <summary>
      /// Volatile read for slow path to ensure visibility after publication.
      /// </summary>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int ReadVolatile() => Volatile.Read(ref _value);

      /// <summary>
      /// Sets the value if it has not been set previously.
      /// </summary>
      /// <param name="value">The value to set. It will be assigned only if the value has not been set before.</param>
      /// <exception cref="InvalidOperationException">Thrown if the value has already been set.</exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Set(int value)
      {
         if (Interlocked.CompareExchange(ref _value, value, -1) != -1)
            throw new InvalidOperationException("Value was already set.");
      }
   }
}
