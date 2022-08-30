using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public class ValueObjectSourceGeneratorState : ISourceGeneratorState, IEquatable<ValueObjectSourceGeneratorState>
{
   private readonly INamedTypeSymbol _type;

   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }

   public string? Namespace { get; }
   public string? NullableQuestionMark => _type.IsReferenceType ? "?" : null;
   public string Name => _type.Name;
   public bool IsReferenceType => _type.IsReferenceType;

   public string? FactoryValidationReturnType { get; }

   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public IReadOnlyList<EqualityInstanceMemberInfo> EqualityMembers { get; }

   [MemberNotNullWhen(true, nameof(KeyMember))]
   public bool HasKeyMember => EqualityMembers.Count == 1
                               && AssignableInstanceFieldsAndProperties.Count == 1
                               && EqualityMembers[0].Member.Equals(AssignableInstanceFieldsAndProperties[0]);

   public EqualityInstanceMemberInfo? KeyMember => HasKeyMember ? EqualityMembers[0] : null;

   public AttributeInfo AttributeInfo { get; }
   public ValueObjectSettings Settings { get; }

   public ValueObjectSourceGeneratorState(INamedTypeSymbol type, AttributeData valueObjectAttribute)
   {
      _type = type ?? throw new ArgumentNullException(nameof(type));

      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

      AssignableInstanceFieldsAndProperties = _type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(true).ToList();
      EqualityMembers = GetEqualityMembers();

      var factoryValidationReturnType = _type.GetMembers()
                                             .OfType<IMethodSymbol>()
                                             .FirstOrDefault(m => m.IsStatic && m.ReturnType.SpecialType != SpecialType.System_Void && m.Name == "ValidateFactoryArguments")?
                                             .ReturnType;

      if (factoryValidationReturnType is not null)
      {
         FactoryValidationReturnType = factoryValidationReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

         if (factoryValidationReturnType.NullableAnnotation == NullableAnnotation.Annotated)
            FactoryValidationReturnType += "?";
      }

      AttributeInfo = new AttributeInfo(type);
      Settings = new ValueObjectSettings(valueObjectAttribute);
   }

   public Location GetFirstLocation()
   {
      return _type.DeclaringSyntaxReferences.First().GetSyntax().GetLocation();
   }

   private IReadOnlyList<EqualityInstanceMemberInfo> GetEqualityMembers()
   {
      var members = AssignableInstanceFieldsAndProperties;

      if (members.Count == 0)
         return Array.Empty<EqualityInstanceMemberInfo>();

      List<EqualityInstanceMemberInfo>? equalityMembers = null;

      foreach (var member in members)
      {
         var settings = member.ValueObjectMemberSettings;

         if (settings.IsExplicitlyDeclared)
         {
            var equalityComparer = settings.EqualityComparer;
            var comparer = settings.Comparer;
            var equalityMember = new EqualityInstanceMemberInfo(member, equalityComparer, comparer);

            (equalityMembers ??= new List<EqualityInstanceMemberInfo>()).Add(equalityMember);
         }
      }

      return equalityMembers ?? members.Select(m => new EqualityInstanceMemberInfo(m, null, null)).ToList();
   }

   public override bool Equals(object? obj)
   {
      return obj is ValueObjectSourceGeneratorState other && Equals(other);
   }

   public bool Equals(ValueObjectSourceGeneratorState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsReferenceType == other.IsReferenceType
             && FactoryValidationReturnType == other.FactoryValidationReturnType
             && AttributeInfo.Equals(other.AttributeInfo)
             && Settings.Equals(other.Settings)
             && AssignableInstanceFieldsAndProperties.EqualsTo(other.AssignableInstanceFieldsAndProperties)
             && EqualityMembers.EqualsTo(other.EqualityMembers);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ FactoryValidationReturnType?.GetHashCode() ?? 0;
         hashCode = (hashCode * 397) ^ AttributeInfo.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ EqualityMembers.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();

         return hashCode;
      }
   }
}
