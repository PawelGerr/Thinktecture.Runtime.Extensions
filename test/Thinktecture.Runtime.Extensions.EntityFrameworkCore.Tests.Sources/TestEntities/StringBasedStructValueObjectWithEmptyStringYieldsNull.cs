namespace Thinktecture.Runtime.Tests.TestEntities;

// String-keyed STRUCT value object with EmptyStringInFactoryMethodsYieldsNull = true.
// The setting is ignored for structs, so the generated ConvertFromKeyExpression must stay Func<string, T>
// and must not become Func<string, Nullable<T>>. Otherwise ThinktectureValueConverterFactory throws an
// InvalidCastException when it casts the expression on the factory read path (useConstructorForRead: false).
[ValueObject<string>(EmptyStringInFactoryMethodsYieldsNull = true)]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial struct StringBasedStructValueObjectWithEmptyStringYieldsNull;
