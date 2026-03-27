namespace Thinktecture;

/// <summary>
/// Controls whether factory methods are generated for ad-hoc union members.
/// </summary>
public enum FactoryMethodGeneration
{
   /// <summary>
   /// Auto-detect: generate factory methods when any member triggers it
   /// (e.g., type parameter member, interface, <c>object</c>, or duplicate type).
   /// </summary>
   Default = 0,

   /// <summary>
   /// Suppress all factory method generation, even when triggers are present.
   /// </summary>
   None = 1,

   /// <summary>
   /// Generate factory methods for all members regardless of triggers.
   /// </summary>
   Always = 2
}
