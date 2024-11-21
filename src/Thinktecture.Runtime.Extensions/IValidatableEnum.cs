namespace Thinktecture;

/// <summary>
/// Interface of Smart Enum.
/// </summary>
public interface IValidatableEnum
{
   /// <summary>
   /// Indication whether the current enumeration item is valid or not.
   /// </summary>
   bool IsValid { get; }

   /// <summary>
   /// Checks whether current enumeration item is valid.
   /// </summary>
   /// <exception cref="InvalidOperationException">The enumeration item is not valid.</exception>
   void EnsureValid();
}
