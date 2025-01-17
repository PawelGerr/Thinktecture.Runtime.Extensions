namespace Thinktecture;

/// <summary>
/// Defines whether and how the implicit conversion operators should be generated.
/// </summary>
public enum ConversionOperatorsGeneration
{
   /// <summary>
   /// No conversion operators will be generated.
   /// </summary>
   None = 0,

   /// <summary>
   /// Conversions will be generated as implicit operators.
   /// </summary>
   Implicit = 1,

   /// <summary>
   /// Conversions will be generated as explicit operators.
   /// </summary>
   Explicit = 2
}
