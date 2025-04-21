namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

[SmartEnum<string>]
public partial class EndpointKind
{
   public static readonly EndpointKind MinimalApi = new("MinimalApi");
   public static readonly EndpointKind Controller = new("Controller");
}
