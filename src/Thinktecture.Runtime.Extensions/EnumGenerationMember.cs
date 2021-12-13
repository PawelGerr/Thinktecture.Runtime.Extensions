namespace Thinktecture;

/// <summary>
/// Settings to be used by the enum source generator.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class EnumGenerationMemberAttribute : Attribute
{
   /// <summary>
   /// The name of other member which is <c>public</c> and yields the same value/behavior as the current member.
   /// </summary>
   public string? MapsToMember { get; set; }
}