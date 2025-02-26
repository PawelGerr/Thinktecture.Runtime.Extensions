namespace Thinktecture;

/// <summary>
/// Marks a partial method in of a Smart Enum or keyed Value Object to be implemented through a delegate.
/// The source generator will automatically generate a private delegate field and wire it up through the constructor.
/// </summary>
/// <example>
/// <code>
/// [SmartEnum]
/// public partial class ImporterType
/// {
///     public static readonly ImporterType Default = new(ProcessDefault);
///
///     [GenerateDelegate]
///     public partial string Process(object input);
///
///     private static string ProcessDefault(object input) => input.ToString();
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class GenerateDelegateAttribute : Attribute;
