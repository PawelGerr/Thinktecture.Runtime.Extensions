using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public class EnumMemberSettings : IEquatable<EnumMemberSettings>
{
   public static readonly EnumMemberSettings None = new();

   private readonly AttributeData? _attributeData;

   public string? MappedMemberName { get; }

   private EnumMemberSettings()
   {
   }

   private EnumMemberSettings(AttributeData attributeData)
   {
      _attributeData = attributeData;
      MappedMemberName = attributeData.FindMapsToMember();
   }

   public static EnumMemberSettings Create(ISymbol member)
   {
      var attr = member.FindAttribute(static type => type.Name == "EnumGenerationMemberAttribute" && type.ContainingNamespace is { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true });

      return attr is null ? None : new EnumMemberSettings(attr);
   }

   public Location? GetAttributeLocationOrNull(CancellationToken cancellationToken)
   {
      return _attributeData?.ApplicationSyntaxReference?.GetSyntax(cancellationToken).GetLocation();
   }

   public override bool Equals(object? obj)
   {
      return obj is EnumMemberSettings other && Equals(other);
   }

   public bool Equals(EnumMemberSettings? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return MappedMemberName == other.MappedMemberName;
   }

   public override int GetHashCode()
   {
      return MappedMemberName?.GetHashCode() ?? 0;
   }
}
