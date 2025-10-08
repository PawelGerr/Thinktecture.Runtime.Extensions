namespace Thinktecture.CodeAnalysis;

public class TypedMemberStateFactory
{
   private const string _SYSTEM_RUNTIME_DLL = "System.Runtime.dll";
   private const string _SYSTEM_CORELIB_DLL = "System.Private.CoreLib.dll";

   private readonly TypedMemberStates _boolean;
   private readonly TypedMemberStates _char;
   private readonly TypedMemberStates _sByte;
   private readonly TypedMemberStates _byte;
   private readonly TypedMemberStates _int16;
   private readonly TypedMemberStates _uInt16;
   private readonly TypedMemberStates _int32;
   private readonly TypedMemberStates _uInt32;
   private readonly TypedMemberStates _int64;
   private readonly TypedMemberStates _uInt64;
   private readonly TypedMemberStates _decimal;
   private readonly TypedMemberStates _single;
   private readonly TypedMemberStates _double;
   private readonly TypedMemberStates _string;
   private readonly TypedMemberStates _dateTime;

   private readonly IReadOnlyDictionary<(string ModuleName, int MetadataToken), TypedMemberStates> _statesByTokens;

   private TypedMemberStateFactory(Compilation compilation)
   {
      _boolean = CreateStates(compilation, SpecialType.System_Boolean);
      _char = CreateStates(compilation, SpecialType.System_Char);
      _sByte = CreateStates(compilation, SpecialType.System_SByte);
      _byte = CreateStates(compilation, SpecialType.System_Byte);
      _int16 = CreateStates(compilation, SpecialType.System_Int16);
      _uInt16 = CreateStates(compilation, SpecialType.System_UInt16);
      _int32 = CreateStates(compilation, SpecialType.System_Int32);
      _uInt32 = CreateStates(compilation, SpecialType.System_UInt32);
      _int64 = CreateStates(compilation, SpecialType.System_Int64);
      _uInt64 = CreateStates(compilation, SpecialType.System_UInt64);
      _decimal = CreateStates(compilation, SpecialType.System_Decimal);
      _single = CreateStates(compilation, SpecialType.System_Single);
      _double = CreateStates(compilation, SpecialType.System_Double);
      _string = CreateStates(compilation, SpecialType.System_String);
      _dateTime = CreateStates(compilation, SpecialType.System_DateTime);

      var lookup = new Dictionary<(string, int), TypedMemberStates>();
      _statesByTokens = lookup;

      CreateAndAddStatesForSystemRuntime(compilation, "System.DateOnly", lookup);
      CreateAndAddStatesForSystemRuntime(compilation, "System.TimeOnly", lookup);
      CreateAndAddStatesForSystemRuntime(compilation, "System.TimeSpan", lookup);
      CreateAndAddStatesForSystemRuntime(compilation, "System.Guid", lookup);
   }

   private static TypedMemberStates CreateStates(Compilation compilation, SpecialType specialType)
   {
      var type = compilation.GetSpecialType(specialType);
      return CreateStates(compilation, type);
   }

   private static void CreateAndAddStatesForSystemRuntime(Compilation compilation, string fullName, Dictionary<(string, int), TypedMemberStates> lookup)
   {
      var types = compilation.GetTypesByMetadataName(fullName);

      if (types.IsDefaultOrEmpty)
         return;

      INamedTypeSymbol? type = null;

      if (types.Length == 1)
      {
         type = types[0];

         if (type.ContainingModule.MetadataName is not (_SYSTEM_RUNTIME_DLL or _SYSTEM_CORELIB_DLL))
            return;
      }
      else
      {
         for (var i = 0; i < types.Length; i++)
         {
            var candidate = types[i];

            if (candidate.ContainingModule.MetadataName is not (_SYSTEM_RUNTIME_DLL or _SYSTEM_CORELIB_DLL))
               continue;

            // duplicate?
            if (type is not null)
               return;

            type = candidate;
         }
      }

      if (type is null || type.TypeKind == TypeKind.Error)
         return;

      lookup.Add((type.ContainingModule.MetadataName, type.MetadataToken), CreateStates(compilation, type));
   }

   private static TypedMemberStates CreateStates(Compilation compilation, INamedTypeSymbol type)
   {
      var nullableType = type.IsReferenceType
                            ? type.WithNullableAnnotation(NullableAnnotation.Annotated)
                            : compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(type);

      var notNullable = new CachedTypedMemberState(new TypedMemberState(type));
      var nullable = new CachedTypedMemberState(new TypedMemberState(nullableType));

      return new TypedMemberStates(notNullable, nullable);
   }

   public ITypedMemberState Create(ITypeSymbol type)
   {
      var cachedStates = type.SpecialType switch
      {
         SpecialType.System_Boolean => _boolean,
         SpecialType.System_Char => _char,
         SpecialType.System_SByte => _sByte,
         SpecialType.System_Byte => _byte,
         SpecialType.System_Int16 => _int16,
         SpecialType.System_UInt16 => _uInt16,
         SpecialType.System_Int32 => _int32,
         SpecialType.System_UInt32 => _uInt32,
         SpecialType.System_Int64 => _int64,
         SpecialType.System_UInt64 => _uInt64,
         SpecialType.System_Decimal => _decimal,
         SpecialType.System_Single => _single,
         SpecialType.System_Double => _double,
         SpecialType.System_String => _string,
         SpecialType.System_DateTime => _dateTime,
         // Array types have no ContainingModule
         _ => type.ContainingModule is not null && _statesByTokens.TryGetValue((type.ContainingModule.MetadataName, type.MetadataToken), out var states) ? states : default
      };

      if (cachedStates == default)
         return new TypedMemberState(type);

      return type.NullableAnnotation == NullableAnnotation.Annotated
                ? cachedStates.Nullable
                : cachedStates.NotNullable;
   }

   public static TypedMemberStateFactory Create(Compilation compilation)
   {
      return new TypedMemberStateFactory(compilation);
   }

   private readonly record struct TypedMemberStates(ITypedMemberState NotNullable, ITypedMemberState Nullable);
}
