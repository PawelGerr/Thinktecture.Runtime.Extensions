namespace Thinktecture.CodeAnalysis;

public class TypedMemberStateFactory
{
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
   }

   private static TypedMemberStates CreateStates(Compilation compilation, SpecialType specialType)
   {
      var type = compilation.GetSpecialType(specialType);
      var nullableType = type.WithNullableAnnotation(NullableAnnotation.Annotated);

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
         _ => (TypedMemberStates?)null
      };

      if (cachedStates is null)
         return new TypedMemberState(type);

      return type.NullableAnnotation == NullableAnnotation.Annotated
                ? cachedStates.Value.Nullable
                : cachedStates.Value.NotNullable;
   }

   public static TypedMemberStateFactory Create(Compilation compilation)
   {
      return new TypedMemberStateFactory(compilation);
   }

   private readonly record struct TypedMemberStates(ITypedMemberState NotNullable, ITypedMemberState Nullable);
}
