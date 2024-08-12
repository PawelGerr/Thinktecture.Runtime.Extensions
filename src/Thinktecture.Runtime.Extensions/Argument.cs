namespace Thinktecture;

/// <summary>
/// An argument with an indication whether the value is set (provided explicitly) or not.
/// </summary>
/// <typeparam name="T">The type of the parameter.</typeparam>
public readonly ref struct Argument<T>
{
   /// <summary>
   /// Indication whether the argument is set (explicitly).
   /// </summary>
   public bool IsSet { get; }

   /// <summary>
   /// Provided value.
   /// </summary>
   public T Value { get; }

   /// <summary>
   /// Initializes new instance of <see cref="Argument{T}"/>.
   /// </summary>
   /// <param name="value">Provided value.</param>
   private Argument(T value)
   {
      Value = value;
      IsSet = true;
   }

   /// <summary>
   /// Implicit conversion from type <typeparamref name="T"/> to <see cref="Argument{T}"/>.
   /// </summary>
   /// <param name="value">Value to convert from.</param>
   /// <returns>A new instance of <see cref="Argument{T}"/> containing the provided <paramref name="value"/>.</returns>
   public static implicit operator Argument<T>(T value)
   {
      return new(value);
   }
}
