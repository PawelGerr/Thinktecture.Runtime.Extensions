namespace Thinktecture;

/// <summary>
/// Marks a partial method of a Smart Enum to be implemented through a delegate.
/// The source generator will automatically generate a private delegate field and wire it up through the constructor.
/// </summary>
/// <example>
/// <code>
/// [SmartEnum]
/// public partial class ImporterType
/// {
///     public static readonly ImporterType Default = new(ProcessDefault);
///
///     [UseDelegateFromConstructor]
///     public partial string Process(int input);
///
///     private static string ProcessDefault(int input) => input.ToString();
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class UseDelegateFromConstructorAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the custom name for the delegate type.
    /// If provided, a custom delegate type will always be generated regardless of parameter ref-kinds.
    /// </summary>
    public string? DelegateName { get; set; }
}
