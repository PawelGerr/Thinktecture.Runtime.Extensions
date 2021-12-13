using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class SameAssemblyBaseEnumState : IBaseEnumState
{
   private readonly EnumSourceGeneratorState _baseState;

   public bool IsSameAssembly => true;
   public INamedTypeSymbol Type => _baseState.EnumType;
   public string? NullableQuestionMark => Type.IsReferenceType ? "?" : null;

   private IReadOnlyList<ISymbolState>? _ctorArgs;

   public IReadOnlyList<ISymbolState> ConstructorArguments
   {
      get
      {
         if (_ctorArgs is null)
         {
            var args = new List<ISymbolState>
                       {
                          new DefaultSymbolState(_baseState.KeyPropertyName, _baseState.KeyType, _baseState.KeyArgumentName, false)
                       };

            foreach (var member in _baseState.AssignableInstanceFieldsAndProperties)
            {
               var memberAttr = member.Symbol.FindEnumGenerationMemberAttribute();
               var mappedMemberName = memberAttr?.FindMapsToMember();

               if (mappedMemberName is not null)
               {
                  args.Add(new DefaultSymbolState(mappedMemberName, member.Type, mappedMemberName.MakeArgumentName(), member.IsStatic));
               }
               else
               {
                  args.Add(member);
               }
            }

            _ctorArgs = args;
         }

         return _ctorArgs;
      }
   }

   private IReadOnlyList<ISymbolState>? _items;
   public IReadOnlyList<ISymbolState> Items => _items ??= Type.EnumerateEnumItems().Select(InstanceMemberInfo.CreateFrom).ToList();

   public SameAssemblyBaseEnumState(EnumSourceGeneratorState baseState)
   {
      _baseState = baseState;
   }
}