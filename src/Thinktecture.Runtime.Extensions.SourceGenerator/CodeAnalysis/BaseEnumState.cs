using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class BaseEnumState : IBaseEnumState
{
   public bool IsSameAssembly => false;
   public INamedTypeSymbol Type { get; }
   public string? NullableQuestionMark => Type.IsReferenceType ? "?" : null;

   private IReadOnlyList<ISymbolState>? _items;

   public IReadOnlyList<ISymbolState> Items => _items ??= Type.EnumerateEnumItems().Select(DefaultSymbolState.CreateFrom).ToList();

   private IReadOnlyList<ISymbolState>? _ctorExtraArgs;

   public IReadOnlyList<ISymbolState> ConstructorArguments
   {
      get
      {
         if (_ctorExtraArgs is null)
         {
            var ctor = Type.Constructors
                           .Where(c => c.MethodKind == MethodKind.Constructor && c.DeclaredAccessibility == Accessibility.Protected)
                           .OrderBy(c => c.Parameters.Length)
                           .FirstOrDefault();

            if (ctor is null)
               throw new Exception($"'{Type.Name}' doesn't have a protected constructor.");

            var ctorAttrArgs = GetCtorParameterNames(ctor);

            _ctorExtraArgs = ctor.Parameters
                                 .Select((p, i) =>
                                         {
                                            var memberName = ctorAttrArgs[i];

                                            if (memberName.Value is not string name)
                                               throw new Exception($"The parameter '{memberName.Value}' of the 'ValueObjectConstructorAttribute' of '{Type.Name}' at index {i} must be a string.");

                                            return new DefaultSymbolState(name, p.Type, p.Name, false);
                                         })
                                 .ToList();
         }

         return _ctorExtraArgs;
      }
   }

   private IReadOnlyList<TypedConstant> GetCtorParameterNames(IMethodSymbol ctor)
   {
      var ctorAttr = Type.FindValueObjectConstructorAttribute();

      if (ctorAttr is null)
         throw new Exception($"'{Type.Name}' doesn't have an 'ValueObjectConstructorAttribute'.");

      if (ctorAttr.ConstructorArguments.Length != 1)
         throw new Exception($"'ValueObjectConstructorAttribute' of '{Type.Name}' must have exactly 1 argument.");

      var ctorAttrArgs = ctorAttr.ConstructorArguments[0].Values;

      if (ctorAttrArgs.Length != ctor.Parameters.Length)
         throw new Exception($"'ValueObjectConstructorAttribute' of '{Type.Name}' specifies {ctorAttrArgs.Length} parameters but the constructor takes {ctor.Parameters.Length} arguments.");

      return ctorAttrArgs;
   }

   public BaseEnumState(INamedTypeSymbol type)
   {
      Type = type;
   }
}