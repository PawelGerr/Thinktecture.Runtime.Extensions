namespace Thinktecture.Unions;

/// <summary>
/// API response union demonstrating stateless types for memory optimization.
/// NotFound and Unauthorized are stateless types that carry no instance data.
/// </summary>
[Union<SuccessResponse, NotFound, Unauthorized>(
   T1Name = "Success",
   T2Name = "NotFound", T2IsStateless = true,
   T3Name = "Unauthorized", T3IsStateless = true)]
public partial class ApiResponseWithStateless;

public sealed class SuccessResponse
{
   public required string Data { get; init; }
}

/// <summary>
/// Stateless type for "not found" state.
/// No backing field is allocated - only the discriminator index is stored.
/// Using a struct avoids null-handling complexity since default(NotFound) is a valid value.
/// </summary>
public readonly record struct NotFound;

/// <summary>
/// Stateless type for "unauthorized" state.
/// No backing field is allocated - only the discriminator index is stored.
/// Using a struct avoids null-handling complexity since default(Unauthorized) is a valid value.
/// </summary>
public readonly record struct Unauthorized;
