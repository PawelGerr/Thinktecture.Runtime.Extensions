namespace Thinktecture;

/// <summary>
/// Signals a desire for an (additional) factory for creation of value objects from a value of type <typeparamref name="TFrom"/>.
/// </summary>
/// <typeparam name="TFrom">Type of the value to be able to create a value object from.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class ValueObjectFactoryAttribute<TFrom> : Attribute
{
}
