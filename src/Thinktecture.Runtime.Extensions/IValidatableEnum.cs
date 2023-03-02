namespace Thinktecture;

/// <summary>
/// Interface of Smart Enum.
/// </summary>
public interface IValidatableEnum
{
   /// <summary>
   /// Indication whether the current enumeration item is valid or not.
   /// </summary>
   bool IsValid => throw new NotImplementedException("This property will be implemented by the source generator.");

   /// <summary>
   /// Checks whether current enumeration item is valid.
   /// </summary>
   /// <exception cref="InvalidOperationException">The enumeration item is not valid.</exception>
   void EnsureValid() => throw new NotImplementedException("This method will be implemented by the source generator.");
}

/// <summary>
/// Interface of Smart Enum.
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
public interface IValidatableEnum<TKey> : IEnum<TKey>, IValidatableEnum
   where TKey : notnull
{
}
