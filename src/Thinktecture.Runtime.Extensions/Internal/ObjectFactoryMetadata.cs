namespace Thinktecture.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public sealed class ObjectFactoryMetadata
{
   /// <summary>
   /// The type of the value.
   /// </summary>
   public required Type ValueType { get; init; }

   /// <summary>
   /// The type of the validation error.
   /// </summary>
   public required Type ValidationErrorType { get; init; }

   /// <summary>
   /// Serialization frameworks that should use the type <see cref="Type"/> for serialization and deserialization.
   /// </summary>
   public required SerializationFrameworks UseForSerialization { get; init; }

   /// <summary>
   /// Indication whether to use the corresponding factory with Entity Framework Core or not.
   /// </summary>
   public required bool UseWithEntityFramework { get; init; }

   /// <summary>
   /// Indication whether to use the corresponding factory with ASP.NET Core model binding or not.
   /// </summary>
   public required bool UseForModelBinding { get; init; }
}
