namespace Thinktecture;

/// <summary>
/// Configures additional Switch and Map method overloads for discriminated unions.
/// These overloads will only handle the specified "stop at" types and their siblings,
/// but not the derived types of "stop at" types.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class UnionSwitchMapOverloadAttribute : Attribute
{
   /// <summary>
   /// The types at which to stop when generating Switch and Map overloads.
   /// The overload will handle these types and their siblings,
   /// but will not descend into the derived types of types defined in <see cref="StopAt"/>.
   /// </summary>
   public Type[] StopAt { get; set; } = [];
}
