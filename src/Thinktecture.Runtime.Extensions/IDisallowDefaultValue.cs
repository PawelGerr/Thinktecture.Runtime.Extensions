namespace Thinktecture;

/// <summary>
/// Marker interface for types whose default value must be rejected.
/// </summary>
/// <remarks>
/// This interface has an effect on value types only. On a reference type the default value is <c>null</c>,
/// which is handled by nullability, so implementing this interface on a class or an interface has no effect.
/// <para>
/// The source generator adds this interface automatically to keyed value objects and struct-based ad-hoc unions
/// that treat their default value as invalid. It can also be implemented manually to opt a custom value type into
/// the same handling.
/// </para>
/// <para>
/// The marker is read at compile time by the analyzer. It drives diagnostic TTRESG047, which reports a use of
/// <c>default</c> or <c>new()</c> for such a type. It also drives the rule that a member of a value object which
/// disallows default values must be marked as <c>required</c>, and the rule that forbids <c>AllowDefaultStructs = true</c>
/// together with this interface.
/// </para>
/// <para>
/// The marker is also read at runtime by the framework integrations to reject a default value during
/// deserialization or model binding. The five read sites are the MessagePack formatter for structs, both
/// System.Text.Json converters (the regular converter and the span-based converter), the Newtonsoft.Json converter,
/// and the ASP.NET Core model binder.
/// </para>
/// </remarks>
public interface IDisallowDefaultValue;
