namespace Thinktecture;

/// <summary>
/// Converts value object to an instance of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type to convert the value object to.</typeparam>
/// <remarks>
/// Don't use this interface directly. It will be used by a source generator.
/// </remarks>
[Obsolete("Use 'IValueObjectConvertible<T>' instead.")]
public interface IValueObjectConvertable<out T> : IValueObjectConvertible<T>
   where T : notnull;

/// <summary>
/// Converts value object to an instance of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type to convert the value object to.</typeparam>
/// <remarks>
/// Don't use this interface directly. It will be used by a source generator.
/// </remarks>
public interface IValueObjectConvertible<out T>
   where T : notnull
{
   /// <summary>
   /// Converts the value object to type <typeparamref name="T"/>.
   /// </summary>
   T ToValue();
}
