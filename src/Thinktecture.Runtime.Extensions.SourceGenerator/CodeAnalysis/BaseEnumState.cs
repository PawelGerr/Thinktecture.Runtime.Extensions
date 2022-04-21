using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public sealed class BaseEnumState<TExtension> : IBaseEnumState<TExtension>, IEquatable<BaseEnumState<TExtension>>
   where TExtension : IEquatable<TExtension>
{
   private readonly INamedTypeSymbol _type;

   public bool IsSameAssembly => false;

   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }

   public string? NullableQuestionMark => _type.IsReferenceType ? "?" : null;

   public IReadOnlyList<IMemberState> ConstructorArguments { get; }
   public IReadOnlyList<IMemberState> Items { get; }

   public TExtension Extension { get; }
   public EnumSettings Settings { get; }

   public BaseEnumState(INamedTypeSymbol type, TExtension extension)
   {
      _type = type;
      Extension = extension;

      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

      ConstructorArguments = GetConstructorArguments(type);
      Items = _type.EnumerateEnumItems().Select(DefaultMemberState.CreateFrom).ToList();

      Settings = new EnumSettings(type.FindEnumGenerationAttribute());
   }

   private static IReadOnlyList<DefaultMemberState> GetConstructorArguments(INamedTypeSymbol type)
   {
      var ctor = type.Constructors
                     .Where(c => c.MethodKind == MethodKind.Constructor && c.DeclaredAccessibility == Accessibility.Protected)
                     .OrderBy(c => c.Parameters.Length)
                     .FirstOrDefault();

      if (ctor is null)
         throw new Exception($"'{type.Name}' doesn't have a protected constructor.");

      var ctorAttrArgs = GetCtorParameterNames(type, ctor);

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

   private static IReadOnlyList<TypedConstant> GetCtorParameterNames(INamedTypeSymbol type, IMethodSymbol ctor)
   {
      var ctorAttr = type.FindValueObjectConstructorAttribute();

      if (ctorAttr is null)
         throw new Exception($"'{type.Name}' doesn't have an 'ValueObjectConstructorAttribute'.");

      if (ctorAttr.ConstructorArguments.Length != 1)
         throw new Exception($"'ValueObjectConstructorAttribute' of '{type.Name}' must have exactly 1 argument.");

      var ctorAttrArgs = ctorAttr.ConstructorArguments[0].Values;

      if (ctorAttrArgs.Length != ctor.Parameters.Length)
         throw new Exception($"'ValueObjectConstructorAttribute' of '{type.Name}' specifies {ctorAttrArgs.Length} parameters but the constructor takes {ctor.Parameters.Length} arguments.");

      return ctorAttrArgs;
   }

   public override bool Equals(object? obj)
   {
      return obj is BaseEnumState<TExtension> other && Equals(other);
   }

   public bool Equals(IBaseEnumState<TExtension>? obj)
   {
      return obj is BaseEnumState<TExtension> other && Equals(other);
   }

   public bool Equals(BaseEnumState<TExtension>? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && _type.IsReferenceType.Equals(other._type.IsReferenceType)
             && Equals(Extension, other.Extension)
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
         hashCode = (hashCode * 397) ^ (Extension?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ ConstructorArguments.ComputeHashCode();
         hashCode = (hashCode * 397) ^ Items.ComputeHashCode();

         return hashCode;
      }
   }
}
