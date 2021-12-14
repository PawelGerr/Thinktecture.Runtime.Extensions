namespace Thinktecture;

/// <summary>
/// Base class for enum-like classes.
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
public interface IEnum<out TKey>
   where TKey : notnull
{
   /// <summary>
   /// Gets the key of the item.
   /// </summary>
   TKey GetKey()
   {
      throw new NotImplementedException("This method will be implemented by the source generator.");
   }
}
