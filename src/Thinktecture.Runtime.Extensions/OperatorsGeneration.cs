namespace Thinktecture;

/// <summary>
/// Describes whether the operators should be generated or not.
/// </summary>
public enum OperatorsGeneration
{
   /// <summary>
   /// Operators won't be generated.
   /// </summary>
   None = -1,

   /// <summary>
   /// Operators for Smart Enum/Value Object will be generated.
   /// </summary>
   Default = 0,

   /// <summary>
   /// Operators for Smart Enum/Value Object will be generated.
   /// Additionally, overloads with the key member type are generated as well.
   /// </summary>
   DefaultWithKeyTypeOverloads = 1
}
