using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class BaseEnumState : IBaseEnumState, IEquatable<BaseEnumState>
{
   private readonly INamedTypeSymbol _type;

   public bool IsSameAssembly => false;

   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }

   public string? NullableQuestionMark => _type.IsReferenceType ? "?" : null;

   public IReadOnlyList<IMemberState> ConstructorArguments { get; }
   public IReadOnlyList<IMemberState> Items { get; }

   public EnumSettings Settings { get; }
   public TypeMemberInfo InnerTypesInfo { get; }

   public BaseEnumState(INamedTypeSymbol type)
   {
      _type = type;

      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

      Settings = new EnumSettings(type.FindEnumGenerationAttribute());
      InnerTypesInfo = new TypeMemberInfo(type);

      ConstructorArguments = GetConstructorArguments(type);
      Items = _type.EnumerateEnumItems().Select(DefaultMemberState.CreateFrom).ToList();
   }

   private static IReadOnlyList<DefaultMemberState> GetConstructorArguments(INamedTypeSymbol type)
   {
      var ctorAttrArgs = GetCtorParameterNames(type);

      var ctor = type.Constructors
                     .FirstOrDefault(c => c.MethodKind == MethodKind.Constructor
                                          && c.DeclaredAccessibility == Accessibility.Protected
                                          && c.Parameters.Length == ctorAttrArgs.Count);

      if (ctor is null)
         throw new Exception($"'{type.Name}' doesn't have a protected constructor with {ctorAttrArgs.Count} arguments.");

      return ctor.Parameters
                 .Select((p, i) =>
                         {
                            var memberName = ctorAttrArgs[i];

                            if (memberName.Value is not string name)
                               throw new Exception($"The parameter '{memberName.Value}' of the 'ValueObjectConstructorAttribute' of '{type.Name}' at index {i} must be a string.");

                            return new DefaultMemberState(name, p.Type, p.Name, false);
                         })
                 .ToList();
   }

   private static IReadOnlyList<TypedConstant> GetCtorParameterNames(INamedTypeSymbol type)
   {
      var ctorAttr = type.FindValueObjectConstructorAttribute();

      if (ctorAttr is null)
         throw new Exception($"'{type.Name}' doesn't have an 'ValueObjectConstructorAttribute'.");

      if (ctorAttr.ConstructorArguments.Length != 1)
         throw new Exception($"'ValueObjectConstructorAttribute' of '{type.Name}' must have exactly 1 argument.");

      return ctorAttr.ConstructorArguments[0].Values;
   }

   public override bool Equals(object? obj)
   {
      return obj is BaseEnumState other && Equals(other);
   }

   public bool Equals(IBaseEnumState? obj)
   {
      return obj is BaseEnumState other && Equals(other);
   }

   public bool Equals(BaseEnumState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && _type.IsReferenceType.Equals(other._type.IsReferenceType)
             && InnerTypesInfo.Equals(other.InnerTypesInfo)
             && Settings.Equals(other.Settings)
             && ConstructorArguments.EqualsTo(other.ConstructorArguments)
             && Items.EqualsTo(other.Items);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ _type.IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ InnerTypesInfo.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ ConstructorArguments.ComputeHashCode();
         hashCode = (hashCode * 397) ^ Items.ComputeHashCode();

         return hashCode;
      }
   }
}
