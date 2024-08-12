namespace Thinktecture;

/// <summary>
/// Describes whether the methods Switch/Map should be generated or not.
/// </summary>
public enum SwitchMapMethodsGeneration
{
   /// <summary>
   /// The methods won't be generated.
   /// </summary>
   None = -1,

   /// <summary>
   /// Method overloads with full item coverage will be generated.
   /// </summary>
   Default = 0,

   /// <summary>
   /// Method overloads with full and partial item coverage will be generated.
   /// </summary>
   DefaultWithPartialOverloads = 1
}
