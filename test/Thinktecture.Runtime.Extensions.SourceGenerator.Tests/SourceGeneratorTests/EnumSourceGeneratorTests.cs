using System.Linq;
using Thinktecture.CodeAnalysis.SmartEnums;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class EnumSourceGeneratorTests : SourceGeneratorTestsBase
{
   private const string _COMPARISON_OPERATORS_INT_BASED = _GENERATED_HEADER + """

                                                                              namespace Thinktecture.Tests;

                                                                              partial class TestEnum :
                                                                                 global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestEnum, global::Thinktecture.Tests.TestEnum, bool>
                                                                              {
                                                                                 /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                                 public static bool operator <(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                                 {
                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                    return left.Key < right.Key;
                                                                                 }

                                                                                 /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                                 public static bool operator <=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                                 {
                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                    return left.Key <= right.Key;
                                                                                 }

                                                                                 /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                                 public static bool operator >(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                                 {
                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                    return left.Key > right.Key;
                                                                                 }

                                                                                 /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                                 public static bool operator >=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                                 {
                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                    return left.Key >= right.Key;
                                                                                 }
                                                                              }

                                                                              """;

   public EnumSourceGeneratorTests(ITestOutputHelper output)
      : base(output)
   {
   }

   private const string _GENERATED_HEADER = """
                                            // <auto-generated />
                                            #nullable enable

                                            """;

   private const string _MAIN_OUTPUT_STRING_BASED_CLASS = _GENERATED_HEADER + """

                                                                              namespace Thinktecture.Tests
                                                                              {
                                                                                 [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
                                                                                 partial class TestEnum : global::Thinktecture.IEnum<string, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                                                    global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                                                 {
                                                                                    [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                                                    internal static void ModuleInit()
                                                                                    {
                                                                                       var convertFromKey = new global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                                                       global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                                                       var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Key);
                                                                                       global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Key;

                                                                                       var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                                                       var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                                                       global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                                                    }

                                                                                    private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                                           = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                                                    private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                                           = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                                                    /// <summary>
                                                                                    /// Gets all valid items.
                                                                                    /// </summary>
                                                                                    public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                                                    /// <summary>
                                                                                    /// The identifier of this item.
                                                                                    /// </summary>
                                                                                    public string Key { get; }

                                                                                    private readonly int _hashCode;

                                                                                    private TestEnum(string key)
                                                                                    {
                                                                                       ValidateConstructorArguments(ref key);

                                                                                       if (key is null)
                                                                                          throw new global::System.ArgumentNullException(nameof(key));

                                                                                       this.Key = key;
                                                                                       this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(key));
                                                                                    }

                                                                                    static partial void ValidateConstructorArguments(ref string key);

                                                                                    /// <summary>
                                                                                    /// Gets the identifier of the item.
                                                                                    /// </summary>
                                                                                    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                                                    string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                                                    {
                                                                                       return this.Key;
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Gets an enumeration item for provided <paramref name="key"/>.
                                                                                    /// </summary>
                                                                                    /// <param name="key">The identifier to return an enumeration item for.</param>
                                                                                    /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                                                    /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
                                                                                    [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                                                    public static global::Thinktecture.Tests.TestEnum? Get(string? key)
                                                                                    {
                                                                                       if (key is null)
                                                                                          return default;

                                                                                       if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                                                       {
                                                                                          throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::Thinktecture.Tests.TestEnum), key);
                                                                                       }

                                                                                       return item;
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                                                    /// </summary>
                                                                                    /// <param name="key">The identifier to return an enumeration item for.</param>
                                                                                    /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                                                    /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                                                    public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                                                    {
                                                                                       if (key is null)
                                                                                       {
                                                                                          item = default;
                                                                                          return false;
                                                                                       }

                                                                                       return _itemsLookup.Value.TryGetValue(key, out item);
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                                                    /// </summary>
                                                                                    /// <param name="key">The identifier to return an enumeration item for.</param>
                                                                                    /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                                                    /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                                                    /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                                                    public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                                                    {
                                                                                       if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                                                       {
                                                                                          return null;
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                          return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                                                       }
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Implicit conversion to the type <see cref="string"/>.
                                                                                    /// </summary>
                                                                                    /// <param name="item">Item to covert.</param>
                                                                                    /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                                                    [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                                                    public static implicit operator string?(global::Thinktecture.Tests.TestEnum? item)
                                                                                    {
                                                                                       return item is null ? default : item.Key;
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Explicit conversion from the type <see cref="string"/>.
                                                                                    /// </summary>
                                                                                    /// <param name="key">Value to covert.</param>
                                                                                    /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                                                    [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                                                    public static explicit operator global::Thinktecture.Tests.TestEnum?(string? key)
                                                                                    {
                                                                                       return global::Thinktecture.Tests.TestEnum.Get(key);
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                                                    {
                                                                                       return global::System.Object.ReferenceEquals(this, other);
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public override bool Equals(object? other)
                                                                                    {
                                                                                       return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public override int GetHashCode()
                                                                                    {
                                                                                       return _hashCode;
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public override string ToString()
                                                                                    {
                                                                                       return this.Key.ToString();
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Executes an action depending on the current item.
                                                                                    /// </summary>
                                                                                    /// <param name="testEnum1">The item to compare to.</param>
                                                                                    /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                                                    /// <param name="testEnum2">The item to compare to.</param>
                                                                                    /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                                                    public void Switch(
                                                                                       TestEnum testEnum1, global::System.Action testEnumAction1,
                                                                                       TestEnum testEnum2, global::System.Action testEnumAction2)
                                                                                    {
                                                                                       if (this == testEnum1)
                                                                                       {
                                                                                          testEnumAction1();
                                                                                       }
                                                                                       else if (this == testEnum2)
                                                                                       {
                                                                                          testEnumAction2();
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                          throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                                                       }
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Executes an action depending on the current item.
                                                                                    /// </summary>
                                                                                    /// <param name="context">Context to be passed to the callbacks.</param>
                                                                                    /// <param name="testEnum1">The item to compare to.</param>
                                                                                    /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                                                    /// <param name="testEnum2">The item to compare to.</param>
                                                                                    /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                                                    public void Switch<TContext>(
                                                                                       TContext context,
                                                                                       TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                                                       TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                                                    {
                                                                                       if (this == testEnum1)
                                                                                       {
                                                                                          testEnumAction1(context);
                                                                                       }
                                                                                       else if (this == testEnum2)
                                                                                       {
                                                                                          testEnumAction2(context);
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                          throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                                                       }
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Executes a function depending on the current item.
                                                                                    /// </summary>
                                                                                    /// <param name="testEnum1">The item to compare to.</param>
                                                                                    /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                                                    /// <param name="testEnum2">The item to compare to.</param>
                                                                                    /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                                                    public TResult Switch<TResult>(
                                                                                       TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                                                       TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                                                    {
                                                                                       if (this == testEnum1)
                                                                                       {
                                                                                          return testEnumFunc1();
                                                                                       }
                                                                                       else if (this == testEnum2)
                                                                                       {
                                                                                          return testEnumFunc2();
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                          throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                                                       }
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Executes a function depending on the current item.
                                                                                    /// </summary>
                                                                                    /// <param name="context">Context to be passed to the callbacks.</param>
                                                                                    /// <param name="testEnum1">The item to compare to.</param>
                                                                                    /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                                                    /// <param name="testEnum2">The item to compare to.</param>
                                                                                    /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                                                    public TResult Switch<TContext, TResult>(
                                                                                       TContext context,
                                                                                       TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                                                       TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                                                    {
                                                                                       if (this == testEnum1)
                                                                                       {
                                                                                          return testEnumFunc1(context);
                                                                                       }
                                                                                       else if (this == testEnum2)
                                                                                       {
                                                                                          return testEnumFunc2(context);
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                          throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                                                       }
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                                                    /// </summary>
                                                                                    /// <param name="testEnum1">The item to compare to.</param>
                                                                                    /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                                                    /// <param name="testEnum2">The item to compare to.</param>
                                                                                    /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                                                    public TResult Map<TResult>(
                                                                                       TestEnum testEnum1, TResult other1,
                                                                                       TestEnum testEnum2, TResult other2)
                                                                                    {
                                                                                       if (this == testEnum1)
                                                                                       {
                                                                                          return other1;
                                                                                       }
                                                                                       else if (this == testEnum2)
                                                                                       {
                                                                                          return other2;
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                          throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                                                       }
                                                                                    }

                                                                                    private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                                                    {
                                                                                       var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(2, global::System.StringComparer.OrdinalIgnoreCase);

                                                                                       void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                                                       {
                                                                                          if (item is null)
                                                                                             throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                                                          if (item.Key is null)
                                                                                             throw new global::System.ArgumentException($"The \"Key\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                                                          if (lookup.ContainsKey(item.Key))
                                                                                             throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                                                          lookup.Add(item.Key, item);
                                                                                       }

                                                                                       AddItem(@Item1, nameof(@Item1));
                                                                                       AddItem(@Item2, nameof(@Item2));

                                                                              #if NET8_0_OR_GREATER
                                                                                       return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::System.StringComparer.OrdinalIgnoreCase);
                                                                              #else
                                                                                       return lookup;
                                                                              #endif
                                                                                    }
                                                                                 }
                                                                              }

                                                                              """;

   private const string _MAIN_OUTPUT_INT_BASED_CLASS = _GENERATED_HEADER + """

                                                                           namespace Thinktecture.Tests
                                                                           {
                                                                              [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, int, global::Thinktecture.ValidationError>))]
                                                                              partial class TestEnum : global::Thinktecture.IEnum<int, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                                                 global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                                              {
                                                                                 [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                                                 internal static void ModuleInit()
                                                                                 {
                                                                                    var convertFromKey = new global::System.Func<int, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                                                    global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                                                    var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, int>(static item => item.Key);
                                                                                    global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, int>> convertToKeyExpression = static item => item.Key;

                                                                                    var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                                                    var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(int), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                                                    global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                                                 }

                                                                                 private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<int, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                                        = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<int, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                                                 private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                                        = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                                                 /// <summary>
                                                                                 /// Gets all valid items.
                                                                                 /// </summary>
                                                                                 public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                                                 /// <summary>
                                                                                 /// The identifier of this item.
                                                                                 /// </summary>
                                                                                 public int Key { get; }

                                                                                 private readonly int _hashCode;

                                                                                 private TestEnum(int key)
                                                                                 {
                                                                                    ValidateConstructorArguments(ref key);

                                                                                    this.Key = key;
                                                                                    this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), key.GetHashCode());
                                                                                 }

                                                                                 static partial void ValidateConstructorArguments(ref int key);

                                                                                 /// <summary>
                                                                                 /// Gets the identifier of the item.
                                                                                 /// </summary>
                                                                                 [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                                                 int global::Thinktecture.IValueObjectConvertable<int>.ToValue()
                                                                                 {
                                                                                    return this.Key;
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Gets an enumeration item for provided <paramref name="key"/>.
                                                                                 /// </summary>
                                                                                 /// <param name="key">The identifier to return an enumeration item for.</param>
                                                                                 /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                                                 /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
                                                                                 public static global::Thinktecture.Tests.TestEnum Get(int key)
                                                                                 {
                                                                                    if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                                                    {
                                                                                       throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::Thinktecture.Tests.TestEnum), key);
                                                                                    }

                                                                                    return item;
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                                                 /// </summary>
                                                                                 /// <param name="key">The identifier to return an enumeration item for.</param>
                                                                                 /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                                                 /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                                                 public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] int key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                                                 {
                                                                                    return _itemsLookup.Value.TryGetValue(key, out item);
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                                                 /// </summary>
                                                                                 /// <param name="key">The identifier to return an enumeration item for.</param>
                                                                                 /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                                                 /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                                                 /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                                                 public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] int key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                                                 {
                                                                                    if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                                                    {
                                                                                       return null;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                       return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                                                    }
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Implicit conversion to the type <see cref="int"/>.
                                                                                 /// </summary>
                                                                                 /// <param name="item">Item to covert.</param>
                                                                                 /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                                                 [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                                                 public static implicit operator int(global::Thinktecture.Tests.TestEnum? item)
                                                                                 {
                                                                                    return item is null ? default : item.Key;
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Explicit conversion from the type <see cref="int"/>.
                                                                                 /// </summary>
                                                                                 /// <param name="key">Value to covert.</param>
                                                                                 /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                                                 [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                                                 public static explicit operator global::Thinktecture.Tests.TestEnum?(int key)
                                                                                 {
                                                                                    return global::Thinktecture.Tests.TestEnum.Get(key);
                                                                                 }

                                                                                 /// <inheritdoc />
                                                                                 public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                                                 {
                                                                                    return global::System.Object.ReferenceEquals(this, other);
                                                                                 }

                                                                                 /// <inheritdoc />
                                                                                 public override bool Equals(object? other)
                                                                                 {
                                                                                    return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                                                 }

                                                                                 /// <inheritdoc />
                                                                                 public override int GetHashCode()
                                                                                 {
                                                                                    return _hashCode;
                                                                                 }

                                                                                 /// <inheritdoc />
                                                                                 public override string ToString()
                                                                                 {
                                                                                    return this.Key.ToString();
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Executes an action depending on the current item.
                                                                                 /// </summary>
                                                                                 /// <param name="testEnum1">The item to compare to.</param>
                                                                                 /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                                                 /// <param name="testEnum2">The item to compare to.</param>
                                                                                 /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                                                 public void Switch(
                                                                                    TestEnum testEnum1, global::System.Action testEnumAction1,
                                                                                    TestEnum testEnum2, global::System.Action testEnumAction2)
                                                                                 {
                                                                                    if (this == testEnum1)
                                                                                    {
                                                                                       testEnumAction1();
                                                                                    }
                                                                                    else if (this == testEnum2)
                                                                                    {
                                                                                       testEnumAction2();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                       throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                                                    }
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Executes an action depending on the current item.
                                                                                 /// </summary>
                                                                                 /// <param name="context">Context to be passed to the callbacks.</param>
                                                                                 /// <param name="testEnum1">The item to compare to.</param>
                                                                                 /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                                                 /// <param name="testEnum2">The item to compare to.</param>
                                                                                 /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                                                 public void Switch<TContext>(
                                                                                    TContext context,
                                                                                    TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                                                    TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                                                 {
                                                                                    if (this == testEnum1)
                                                                                    {
                                                                                       testEnumAction1(context);
                                                                                    }
                                                                                    else if (this == testEnum2)
                                                                                    {
                                                                                       testEnumAction2(context);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                       throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                                                    }
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Executes a function depending on the current item.
                                                                                 /// </summary>
                                                                                 /// <param name="testEnum1">The item to compare to.</param>
                                                                                 /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                                                 /// <param name="testEnum2">The item to compare to.</param>
                                                                                 /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                                                 public TResult Switch<TResult>(
                                                                                    TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                                                    TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                                                 {
                                                                                    if (this == testEnum1)
                                                                                    {
                                                                                       return testEnumFunc1();
                                                                                    }
                                                                                    else if (this == testEnum2)
                                                                                    {
                                                                                       return testEnumFunc2();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                       throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                                                    }
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Executes a function depending on the current item.
                                                                                 /// </summary>
                                                                                 /// <param name="context">Context to be passed to the callbacks.</param>
                                                                                 /// <param name="testEnum1">The item to compare to.</param>
                                                                                 /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                                                 /// <param name="testEnum2">The item to compare to.</param>
                                                                                 /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                                                 public TResult Switch<TContext, TResult>(
                                                                                    TContext context,
                                                                                    TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                                                    TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                                                 {
                                                                                    if (this == testEnum1)
                                                                                    {
                                                                                       return testEnumFunc1(context);
                                                                                    }
                                                                                    else if (this == testEnum2)
                                                                                    {
                                                                                       return testEnumFunc2(context);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                       throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                                                    }
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                                                 /// </summary>
                                                                                 /// <param name="testEnum1">The item to compare to.</param>
                                                                                 /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                                                 /// <param name="testEnum2">The item to compare to.</param>
                                                                                 /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                                                 public TResult Map<TResult>(
                                                                                    TestEnum testEnum1, TResult other1,
                                                                                    TestEnum testEnum2, TResult other2)
                                                                                 {
                                                                                    if (this == testEnum1)
                                                                                    {
                                                                                       return other1;
                                                                                    }
                                                                                    else if (this == testEnum2)
                                                                                    {
                                                                                       return other2;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                       throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                                                    }
                                                                                 }

                                                                                 private static global::System.Collections.Generic.IReadOnlyDictionary<int, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                                                 {
                                                                                    var lookup = new global::System.Collections.Generic.Dictionary<int, global::Thinktecture.Tests.TestEnum>(2);

                                                                                    void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                                                    {
                                                                                       if (item is null)
                                                                                          throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                                                       if (lookup.ContainsKey(item.Key))
                                                                                          throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                                                       lookup.Add(item.Key, item);
                                                                                    }

                                                                                    AddItem(@Item1, nameof(@Item1));
                                                                                    AddItem(@Item2, nameof(@Item2));

                                                                           #if NET8_0_OR_GREATER
                                                                                    return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup);
                                                                           #else
                                                                                    return lookup;
                                                                           #endif
                                                                                 }
                                                                              }
                                                                           }

                                                                           """;

   private const string _COMPARABLE_OUTPUT_CLASS_STRING_BASED = _GENERATED_HEADER + """

                                                                                    namespace Thinktecture.Tests;

                                                                                    partial class TestEnum :
                                                                                       global::System.IComparable,
                                                                                       global::System.IComparable<global::Thinktecture.Tests.TestEnum>
                                                                                    {
                                                                                       /// <inheritdoc />
                                                                                       public int CompareTo(object? obj)
                                                                                       {
                                                                                          if(obj is null)
                                                                                             return 1;

                                                                                          if(obj is not global::Thinktecture.Tests.TestEnum item)
                                                                                             throw new global::System.ArgumentException("Argument must be of type \"TestEnum\".", nameof(obj));

                                                                                          return this.CompareTo(item);
                                                                                       }

                                                                                       /// <inheritdoc />
                                                                                       public int CompareTo(global::Thinktecture.Tests.TestEnum? obj)
                                                                                       {
                                                                                          if(obj is null)
                                                                                             return 1;

                                                                                          return global::System.StringComparer.OrdinalIgnoreCase.Compare(this.Key, obj.Key);
                                                                                       }
                                                                                    }

                                                                                    """;

   private const string _COMPARABLE_OUTPUT_CLASS_INT_BASED = _GENERATED_HEADER + """

                                                                                 namespace Thinktecture.Tests;

                                                                                 partial class TestEnum :
                                                                                    global::System.IComparable,
                                                                                    global::System.IComparable<global::Thinktecture.Tests.TestEnum>
                                                                                 {
                                                                                    /// <inheritdoc />
                                                                                    public int CompareTo(object? obj)
                                                                                    {
                                                                                       if(obj is null)
                                                                                          return 1;

                                                                                       if(obj is not global::Thinktecture.Tests.TestEnum item)
                                                                                          throw new global::System.ArgumentException("Argument must be of type \"TestEnum\".", nameof(obj));

                                                                                       return this.CompareTo(item);
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public int CompareTo(global::Thinktecture.Tests.TestEnum? obj)
                                                                                    {
                                                                                       if(obj is null)
                                                                                          return 1;

                                                                                       return this.Key.CompareTo(obj.Key);
                                                                                    }
                                                                                 }

                                                                                 """;

   private const string _PARSABLE_OUTPUT_CLASS_STRING_BASED = _GENERATED_HEADER + """

                                                                                  namespace Thinktecture.Tests;

                                                                                  partial class TestEnum :
                                                                                     global::System.IParsable<global::Thinktecture.Tests.TestEnum>
                                                                                  {
                                                                                     private static global::Thinktecture.ValidationError? Validate<T>(string key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestEnum? result)
                                                                                        where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>
                                                                                     {
                                                                                        return T.Validate(key, provider, out result);
                                                                                     }

                                                                                     /// <inheritdoc />
                                                                                     public static global::Thinktecture.Tests.TestEnum Parse(string s, global::System.IFormatProvider? provider)
                                                                                     {
                                                                                        var validationError = Validate<global::Thinktecture.Tests.TestEnum>(s, provider, out var result);

                                                                                        if(validationError is null)
                                                                                           return result!;

                                                                                        throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestEnum\".");
                                                                                     }

                                                                                     /// <inheritdoc />
                                                                                     public static bool TryParse(
                                                                                        string? s,
                                                                                        global::System.IFormatProvider? provider,
                                                                                        [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum result)
                                                                                     {
                                                                                        if(s is null)
                                                                                        {
                                                                                           result = default;
                                                                                           return false;
                                                                                        }

                                                                                        var validationError = Validate<global::Thinktecture.Tests.TestEnum>(s, provider, out result!);
                                                                                        return validationError is null;
                                                                                     }
                                                                                  }

                                                                                  """;

   private const string _EQUALITY_COMPARABLE_OPERATORS_CLASS = _GENERATED_HEADER + """

                                                                                   namespace Thinktecture.Tests;

                                                                                   partial class TestEnum :
                                                                                      global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestEnum, global::Thinktecture.Tests.TestEnum, bool>
                                                                                   {
                                                                                         /// <summary>
                                                                                         /// Compares two instances of <see cref="TestEnum"/>.
                                                                                         /// </summary>
                                                                                         /// <param name="obj">Instance to compare.</param>
                                                                                         /// <param name="other">Another instance to compare.</param>
                                                                                         /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                         public static bool operator ==(global::Thinktecture.Tests.TestEnum? obj, global::Thinktecture.Tests.TestEnum? other)
                                                                                         {
                                                                                            return global::System.Object.ReferenceEquals(obj, other);
                                                                                         }

                                                                                         /// <summary>
                                                                                         /// Compares two instances of <see cref="TestEnum"/>.
                                                                                         /// </summary>
                                                                                         /// <param name="obj">Instance to compare.</param>
                                                                                         /// <param name="other">Another instance to compare.</param>
                                                                                         /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                         public static bool operator !=(global::Thinktecture.Tests.TestEnum? obj, global::Thinktecture.Tests.TestEnum? other)
                                                                                         {
                                                                                            return !(obj == other);
                                                                                         }
                                                                                   }

                                                                                   """;

   private const string _EQUALITY_COMPARABLE_OPERATORS_STRUCT = _GENERATED_HEADER + """

                                                                                    namespace Thinktecture.Tests;

                                                                                    partial struct TestEnum :
                                                                                       global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestEnum, global::Thinktecture.Tests.TestEnum, bool>
                                                                                    {
                                                                                          /// <summary>
                                                                                          /// Compares two instances of <see cref="TestEnum"/>.
                                                                                          /// </summary>
                                                                                          /// <param name="obj">Instance to compare.</param>
                                                                                          /// <param name="other">Another instance to compare.</param>
                                                                                          /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                          public static bool operator ==(global::Thinktecture.Tests.TestEnum obj, global::Thinktecture.Tests.TestEnum other)
                                                                                          {
                                                                                             return obj.Equals(other);
                                                                                          }

                                                                                          /// <summary>
                                                                                          /// Compares two instances of <see cref="TestEnum"/>.
                                                                                          /// </summary>
                                                                                          /// <param name="obj">Instance to compare.</param>
                                                                                          /// <param name="other">Another instance to compare.</param>
                                                                                          /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                          public static bool operator !=(global::Thinktecture.Tests.TestEnum obj, global::Thinktecture.Tests.TestEnum other)
                                                                                          {
                                                                                             return !(obj == other);
                                                                                          }
                                                                                    }

                                                                                    """;

   private const string _EQUALITY_COMPARABLE_OPERATORS_CLASS_INT_BASED_WITH_KEY_OVERLOADS = _GENERATED_HEADER + """

                                                                                                                namespace Thinktecture.Tests;

                                                                                                                partial class TestEnum :
                                                                                                                   global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestEnum, global::Thinktecture.Tests.TestEnum, bool>,
                                                                                                                   global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestEnum, int, bool>
                                                                                                                {
                                                                                                                      /// <summary>
                                                                                                                      /// Compares two instances of <see cref="TestEnum"/>.
                                                                                                                      /// </summary>
                                                                                                                      /// <param name="obj">Instance to compare.</param>
                                                                                                                      /// <param name="other">Another instance to compare.</param>
                                                                                                                      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                                                      public static bool operator ==(global::Thinktecture.Tests.TestEnum? obj, global::Thinktecture.Tests.TestEnum? other)
                                                                                                                      {
                                                                                                                         return global::System.Object.ReferenceEquals(obj, other);
                                                                                                                      }

                                                                                                                      /// <summary>
                                                                                                                      /// Compares two instances of <see cref="TestEnum"/>.
                                                                                                                      /// </summary>
                                                                                                                      /// <param name="obj">Instance to compare.</param>
                                                                                                                      /// <param name="other">Another instance to compare.</param>
                                                                                                                      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                                                      public static bool operator !=(global::Thinktecture.Tests.TestEnum? obj, global::Thinktecture.Tests.TestEnum? other)
                                                                                                                      {
                                                                                                                         return !(obj == other);
                                                                                                                      }

                                                                                                                      private static bool Equals(global::Thinktecture.Tests.TestEnum? obj, int value)
                                                                                                                      {
                                                                                                                         if (obj is null)
                                                                                                                            return false;

                                                                                                                         return obj.Key.Equals(value);
                                                                                                                      }

                                                                                                                      /// <summary>
                                                                                                                      /// Compares an instance of <see cref="TestEnum"/> with <see cref="int"/>.
                                                                                                                      /// </summary>
                                                                                                                      /// <param name="obj">Instance to compare.</param>
                                                                                                                      /// <param name="value">Value to compare with.</param>
                                                                                                                      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                                                      public static bool operator ==(global::Thinktecture.Tests.TestEnum? obj, int value)
                                                                                                                      {
                                                                                                                         return Equals(obj, value);
                                                                                                                      }

                                                                                                                      /// <summary>
                                                                                                                      /// Compares an instance of <see cref="TestEnum"/> with <see cref="int"/>.
                                                                                                                      /// </summary>
                                                                                                                      /// <param name="value">Value to compare.</param>
                                                                                                                      /// <param name="obj">Instance to compare with.</param>
                                                                                                                      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                                                      public static bool operator ==(int value, global::Thinktecture.Tests.TestEnum? obj)
                                                                                                                      {
                                                                                                                         return Equals(obj, value);
                                                                                                                      }

                                                                                                                      /// <summary>
                                                                                                                      /// Compares an instance of <see cref="TestEnum"/> with <see cref="int"/>.
                                                                                                                      /// </summary>
                                                                                                                      /// <param name="obj">Instance to compare.</param>
                                                                                                                      /// <param name="value">Value to compare with.</param>
                                                                                                                      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                                                      public static bool operator !=(global::Thinktecture.Tests.TestEnum? obj, int value)
                                                                                                                      {
                                                                                                                         return !(obj == value);
                                                                                                                      }

                                                                                                                      /// <summary>
                                                                                                                      /// Compares an instance of <see cref="int"/> with <see cref="TestEnum"/>.
                                                                                                                      /// </summary>
                                                                                                                      /// <param name="value">Value to compare.</param>
                                                                                                                      /// <param name="obj">Instance to compare with.</param>
                                                                                                                      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                                                      public static bool operator !=(int value, global::Thinktecture.Tests.TestEnum? obj)
                                                                                                                      {
                                                                                                                         return !(obj == value);
                                                                                                                      }
                                                                                                                }

                                                                                                                """;

   private const string _EQUALITY_COMPARABLE_OPERATORS_VALIDATABLE_CLASS = _GENERATED_HEADER + """

                                                                                               namespace Thinktecture.Tests;

                                                                                               partial class TestEnum :
                                                                                                  global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestEnum, global::Thinktecture.Tests.TestEnum, bool>
                                                                                               {
                                                                                                     /// <summary>
                                                                                                     /// Compares two instances of <see cref="TestEnum"/>.
                                                                                                     /// </summary>
                                                                                                     /// <param name="obj">Instance to compare.</param>
                                                                                                     /// <param name="other">Another instance to compare.</param>
                                                                                                     /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                                     public static bool operator ==(global::Thinktecture.Tests.TestEnum? obj, global::Thinktecture.Tests.TestEnum? other)
                                                                                                     {
                                                                                                        if (obj is null)
                                                                                                           return other is null;

                                                                                                        return obj.Equals(other);
                                                                                                     }

                                                                                                     /// <summary>
                                                                                                     /// Compares two instances of <see cref="TestEnum"/>.
                                                                                                     /// </summary>
                                                                                                     /// <param name="obj">Instance to compare.</param>
                                                                                                     /// <param name="other">Another instance to compare.</param>
                                                                                                     /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                                     public static bool operator !=(global::Thinktecture.Tests.TestEnum? obj, global::Thinktecture.Tests.TestEnum? other)
                                                                                                     {
                                                                                                        return !(obj == other);
                                                                                                     }
                                                                                               }

                                                                                               """;

   private const string _PARSABLE_OUTPUT_VALIDATABLE_CLASS_STRING_BASED = _GENERATED_HEADER + """

                                                                                              namespace Thinktecture.Tests;

                                                                                              partial class TestEnum :
                                                                                                 global::System.IParsable<global::Thinktecture.Tests.TestEnum>
                                                                                              {
                                                                                                 private static global::Thinktecture.ValidationError? Validate<T>(string key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestEnum? result)
                                                                                                    where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>
                                                                                                 {
                                                                                                    return T.Validate(key, provider, out result);
                                                                                                 }

                                                                                                 /// <inheritdoc />
                                                                                                 public static global::Thinktecture.Tests.TestEnum Parse(string s, global::System.IFormatProvider? provider)
                                                                                                 {
                                                                                                    var validationError = Validate<global::Thinktecture.Tests.TestEnum>(s, provider, out var result);
                                                                                                    return result!;
                                                                                                 }

                                                                                                 /// <inheritdoc />
                                                                                                 public static bool TryParse(
                                                                                                    string? s,
                                                                                                    global::System.IFormatProvider? provider,
                                                                                                    [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum result)
                                                                                                 {
                                                                                                    if(s is null)
                                                                                                    {
                                                                                                       result = default;
                                                                                                       return false;
                                                                                                    }

                                                                                                    var validationError = Validate<global::Thinktecture.Tests.TestEnum>(s, provider, out result!);
                                                                                                    return true;
                                                                                                 }
                                                                                              }

                                                                                              """;

   private const string _PARSABLE_OUTPUT_CLASS_INT_BASED = _GENERATED_HEADER + """

                                                                               namespace Thinktecture.Tests;

                                                                               partial class TestEnum :
                                                                                  global::System.IParsable<global::Thinktecture.Tests.TestEnum>
                                                                               {
                                                                                  private static global::Thinktecture.ValidationError? Validate<T>(int key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestEnum? result)
                                                                                     where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestEnum, int, global::Thinktecture.ValidationError>
                                                                                  {
                                                                                     return T.Validate(key, provider, out result);
                                                                                  }

                                                                                  /// <inheritdoc />
                                                                                  public static global::Thinktecture.Tests.TestEnum Parse(string s, global::System.IFormatProvider? provider)
                                                                                  {
                                                                                     var key = int.Parse(s, provider);
                                                                                     var validationError = Validate<global::Thinktecture.Tests.TestEnum>(key, provider, out var result);

                                                                                     if(validationError is null)
                                                                                        return result!;

                                                                                     throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestEnum\".");
                                                                                  }

                                                                                  /// <inheritdoc />
                                                                                  public static bool TryParse(
                                                                                     string? s,
                                                                                     global::System.IFormatProvider? provider,
                                                                                     [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum result)
                                                                                  {
                                                                                     if(s is null)
                                                                                     {
                                                                                        result = default;
                                                                                        return false;
                                                                                     }

                                                                                     if(!int.TryParse(s, provider, out var key))
                                                                                     {
                                                                                        result = default;
                                                                                        return false;
                                                                                     }

                                                                                     var validationError = Validate<global::Thinktecture.Tests.TestEnum>(key, provider, out result!);
                                                                                     return validationError is null;
                                                                                  }
                                                                               }

                                                                               """;

   private const string _FORMATTABLE_OUTPUT_CLASS = _GENERATED_HEADER + """

                                                                        namespace Thinktecture.Tests;

                                                                        partial class TestEnum :
                                                                           global::System.IFormattable
                                                                        {
                                                                           /// <inheritdoc />
                                                                           public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)
                                                                           {
                                                                              return this.Key.ToString(format, formatProvider);
                                                                           }
                                                                        }

                                                                        """;

   private const string _COMPARISON_OPERATORS_OUTPUT_CLASS_STRING_BASED = _GENERATED_HEADER + """

                                                                                              namespace Thinktecture.Tests;

                                                                                              partial class TestEnum :
                                                                                                 global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestEnum, global::Thinktecture.Tests.TestEnum, bool>
                                                                                              {

                                                                                                 /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                                                 public static bool operator <(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                                                 {
                                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                                    return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) < 0;
                                                                                                 }

                                                                                                 /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                                                 public static bool operator <=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                                                 {
                                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                                    return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) <= 0;
                                                                                                 }

                                                                                                 /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                                                 public static bool operator >(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                                                 {
                                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                                    return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) > 0;
                                                                                                 }

                                                                                                 /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                                                 public static bool operator >=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                                                 {
                                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                                    global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                                    return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) >= 0;
                                                                                                 }
                                                                                              }

                                                                                              """;

   [Fact]
   public void Should_not_generate_if_class_is_not_partial()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                     [SmartEnum<string>]
                   	public class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }
                   """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      AssertOutput(output, null);
   }

   [Fact]
   public void Should_not_generate_if_generic()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                   	[SmartEnum<string>]
                     public partial class TestEnum<T>
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }
                   """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      AssertOutput(output, null);
   }

   [Fact]
   public void Should_generate_string_based_class()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                   	[SmartEnum<string>]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(mainOutput, _MAIN_OUTPUT_STRING_BASED_CLASS);
      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);
   }

   [Fact]
   public void Should_not_crash_if_type_flagged_with_multiple_source_gen_attributes()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                     [SmartEnum]
                   	[SmartEnum<string>]
                     [ValueObject]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().BeEmpty();
   }

   [Fact]
   public void Should_generate_keyless_class()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                   	[SmartEnum]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(2);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      partial class TestEnum :
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(GetItems, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         private readonly int _hashCode;

                                                         private TestEnum()
                                                         {
                                                            ValidateConstructorArguments();

                                                            this._hashCode = base.GetHashCode();
                                                         }

                                                         static partial void ValidateConstructorArguments();

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            return global::System.Object.ReferenceEquals(this, other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> GetItems()
                                                         {
                                                            var list = new global::System.Collections.Generic.List<global::Thinktecture.Tests.TestEnum>(2);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               list.Add(item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                            return list.AsReadOnly();
                                                         }
                                                      }
                                                   }

                                                   """);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);
   }

   [Fact]
   public void Should_generate_keyless_class_with_factory()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                   	[SmartEnum]
                     [ValueObjectFactory<string>]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(2);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      partial class TestEnum :
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(GetItems, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         private readonly int _hashCode;

                                                         private TestEnum()
                                                         {
                                                            ValidateConstructorArguments();

                                                            this._hashCode = base.GetHashCode();
                                                         }

                                                         static partial void ValidateConstructorArguments();

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            return global::System.Object.ReferenceEquals(this, other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> GetItems()
                                                         {
                                                            var list = new global::System.Collections.Generic.List<global::Thinktecture.Tests.TestEnum>(2);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               list.Add(item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                            return list.AsReadOnly();
                                                         }
                                                      }
                                                   }

                                                   """);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);
   }

   [Fact]
   public void Should_generate_string_based_class_having_ValueObjectValidationErrorAttribute()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                   	[SmartEnum<string>]
                     [ValueObjectValidationError<TestEnumValidationError>]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }

                      public class TestEnumValidationError : IValidationError<TestEnumValidationError>
                      {
                         public string Message { get; }

                         public TestEnumValidationError(string message)
                         {
                            Message = message;
                         }

                         public static TestEnumValidationError Create(string message)
                         {
                            return new TestEnumValidationError(message);
                         }

                         public override string ToString()
                         {
                            return Message;
                         }
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.Tests.TestEnumValidationError>))]
                                                      partial class TestEnum : global::Thinktecture.IEnum<string, global::Thinktecture.Tests.TestEnum, global::Thinktecture.Tests.TestEnumValidationError>,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Key);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Key;

                                                            var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public string Key { get; }

                                                         private readonly int _hashCode;

                                                         private TestEnum(string key)
                                                         {
                                                            ValidateConstructorArguments(ref key);

                                                            if (key is null)
                                                               throw new global::System.ArgumentNullException(nameof(key));

                                                            this.Key = key;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(key));
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string key);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this.Key;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="key"/>.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static global::Thinktecture.Tests.TestEnum? Get(string? key)
                                                         {
                                                            if (key is null)
                                                               return default;

                                                            if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                            {
                                                               throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::Thinktecture.Tests.TestEnum), key);
                                                            }

                                                            return item;
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if (key is null)
                                                            {
                                                               item = default;
                                                               return false;
                                                            }

                                                            return _itemsLookup.Value.TryGetValue(key, out item);
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.Tests.TestEnumValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.Tests.TestEnumValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.Tests.TestEnumValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestEnum? item)
                                                         {
                                                            return item is null ? default : item.Key;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="key">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static explicit operator global::Thinktecture.Tests.TestEnum?(string? key)
                                                         {
                                                            return global::Thinktecture.Tests.TestEnum.Get(key);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            return global::System.Object.ReferenceEquals(this, other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Key.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(2, global::System.StringComparer.OrdinalIgnoreCase);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (item.Key is null)
                                                                  throw new global::System.ArgumentException($"The \"Key\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (lookup.ContainsKey(item.Key))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                               lookup.Add(item.Key, item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::System.StringComparer.OrdinalIgnoreCase);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }
                                                   }

                                                   """);

      AssertOutput(parsableOutput, _GENERATED_HEADER + """

                                                       namespace Thinktecture.Tests;

                                                       partial class TestEnum :
                                                          global::System.IParsable<global::Thinktecture.Tests.TestEnum>
                                                       {
                                                          private static global::Thinktecture.Tests.TestEnumValidationError? Validate<T>(string key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestEnum? result)
                                                             where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.Tests.TestEnumValidationError>
                                                          {
                                                             return T.Validate(key, provider, out result);
                                                          }

                                                          /// <inheritdoc />
                                                          public static global::Thinktecture.Tests.TestEnum Parse(string s, global::System.IFormatProvider? provider)
                                                          {
                                                             var validationError = Validate<global::Thinktecture.Tests.TestEnum>(s, provider, out var result);

                                                             if(validationError is null)
                                                                return result!;

                                                             throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestEnum\".");
                                                          }

                                                          /// <inheritdoc />
                                                          public static bool TryParse(
                                                             string? s,
                                                             global::System.IFormatProvider? provider,
                                                             [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum result)
                                                          {
                                                             if(s is null)
                                                             {
                                                                result = default;
                                                                return false;
                                                             }

                                                             var validationError = Validate<global::Thinktecture.Tests.TestEnum>(s, provider, out result!);
                                                             return validationError is null;
                                                          }
                                                       }

                                                       """);
   }

   [Fact]
   public void Should_generate_string_based_class_without_Switch()
   {
      var source = """
                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                      [SmartEnum<string>(SkipSwitchMethods = true)]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
                                                      partial class TestEnum : global::Thinktecture.IEnum<string, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Key);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Key;

                                                            var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public string Key { get; }

                                                         private readonly int _hashCode;

                                                         private TestEnum(string key)
                                                         {
                                                            ValidateConstructorArguments(ref key);

                                                            if (key is null)
                                                               throw new global::System.ArgumentNullException(nameof(key));

                                                            this.Key = key;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(key));
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string key);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this.Key;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="key"/>.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static global::Thinktecture.Tests.TestEnum? Get(string? key)
                                                         {
                                                            if (key is null)
                                                               return default;

                                                            if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                            {
                                                               throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::Thinktecture.Tests.TestEnum), key);
                                                            }

                                                            return item;
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if (key is null)
                                                            {
                                                               item = default;
                                                               return false;
                                                            }

                                                            return _itemsLookup.Value.TryGetValue(key, out item);
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestEnum? item)
                                                         {
                                                            return item is null ? default : item.Key;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="key">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static explicit operator global::Thinktecture.Tests.TestEnum?(string? key)
                                                         {
                                                            return global::Thinktecture.Tests.TestEnum.Get(key);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            return global::System.Object.ReferenceEquals(this, other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Key.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(2, global::System.StringComparer.OrdinalIgnoreCase);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (item.Key is null)
                                                                  throw new global::System.ArgumentException($"The \"Key\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (lookup.ContainsKey(item.Key))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                               lookup.Add(item.Key, item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::System.StringComparer.OrdinalIgnoreCase);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_string_based_class_without_Map()
   {
      var source = """
                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [SmartEnum<string>(SkipMapMethods = true)]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
                                                      partial class TestEnum : global::Thinktecture.IEnum<string, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Key);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Key;

                                                            var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public string Key { get; }

                                                         private readonly int _hashCode;

                                                         private TestEnum(string key)
                                                         {
                                                            ValidateConstructorArguments(ref key);

                                                            if (key is null)
                                                               throw new global::System.ArgumentNullException(nameof(key));

                                                            this.Key = key;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(key));
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string key);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this.Key;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="key"/>.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static global::Thinktecture.Tests.TestEnum? Get(string? key)
                                                         {
                                                            if (key is null)
                                                               return default;

                                                            if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                            {
                                                               throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::Thinktecture.Tests.TestEnum), key);
                                                            }

                                                            return item;
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if (key is null)
                                                            {
                                                               item = default;
                                                               return false;
                                                            }

                                                            return _itemsLookup.Value.TryGetValue(key, out item);
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestEnum? item)
                                                         {
                                                            return item is null ? default : item.Key;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="key">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static explicit operator global::Thinktecture.Tests.TestEnum?(string? key)
                                                         {
                                                            return global::Thinktecture.Tests.TestEnum.Get(key);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            return global::System.Object.ReferenceEquals(this, other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Key.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(2, global::System.StringComparer.OrdinalIgnoreCase);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (item.Key is null)
                                                                  throw new global::System.ArgumentException($"The \"Key\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (lookup.ContainsKey(item.Key))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                               lookup.Add(item.Key, item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::System.StringComparer.OrdinalIgnoreCase);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_string_based_class_with_base_class_and_non_default_constructors()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                      public class BaseClass
                      {
                         protected BaseClass(int value)
                         {
                         }

                         protected BaseClass(string key)
                         {
                         }
                      }

                     [SmartEnum<string>]
                   	public partial class TestEnum : BaseClass
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
                                                      partial class TestEnum : global::Thinktecture.IEnum<string, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Key);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Key;

                                                            var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public string Key { get; }

                                                         private readonly int _hashCode;

                                                         private TestEnum(string key, int value)
                                                            : base(value)
                                                         {
                                                            ValidateConstructorArguments(ref key, ref value);

                                                            if (key is null)
                                                               throw new global::System.ArgumentNullException(nameof(key));

                                                            this.Key = key;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(key));
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string key, ref int value);

                                                         private TestEnum(string key, string key1)
                                                            : base(key1)
                                                         {
                                                            ValidateConstructorArguments(ref key, ref key1);

                                                            if (key is null)
                                                               throw new global::System.ArgumentNullException(nameof(key));

                                                            this.Key = key;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(key));
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string key, ref string key1);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this.Key;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="key"/>.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static global::Thinktecture.Tests.TestEnum? Get(string? key)
                                                         {
                                                            if (key is null)
                                                               return default;

                                                            if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                            {
                                                               throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::Thinktecture.Tests.TestEnum), key);
                                                            }

                                                            return item;
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if (key is null)
                                                            {
                                                               item = default;
                                                               return false;
                                                            }

                                                            return _itemsLookup.Value.TryGetValue(key, out item);
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestEnum? item)
                                                         {
                                                            return item is null ? default : item.Key;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="key">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static explicit operator global::Thinktecture.Tests.TestEnum?(string? key)
                                                         {
                                                            return global::Thinktecture.Tests.TestEnum.Get(key);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            return global::System.Object.ReferenceEquals(this, other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Key.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(2, global::System.StringComparer.OrdinalIgnoreCase);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (item.Key is null)
                                                                  throw new global::System.ArgumentException($"The \"Key\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (lookup.ContainsKey(item.Key))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                               lookup.Add(item.Key, item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::System.StringComparer.OrdinalIgnoreCase);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_string_based_class_without_namespace()
   {
      var source = """
                   using System;
                   using Thinktecture;

                   [SmartEnum<string>]
                   public partial class TestEnum
                   {
                      public static readonly TestEnum Item1 = new("Item1");
                      public static readonly TestEnum Item2 = new("Item2");
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::TestEnum, string, global::Thinktecture.ValidationError>))]
                                                      partial class TestEnum : global::Thinktecture.IEnum<string, global::TestEnum, global::Thinktecture.ValidationError>,
                                                         global::System.IEquatable<global::TestEnum?>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<string?, global::TestEnum?>(global::TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string?, global::TestEnum?>> convertFromKeyExpression = static key => global::TestEnum.Get(key);

                                                            var convertToKey = new global::System.Func<global::TestEnum, string>(static item => item.Key);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::TestEnum, string>> convertToKeyExpression = static item => item.Key;

                                                            var enumType = typeof(global::TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public string Key { get; }

                                                         private readonly int _hashCode;

                                                         private TestEnum(string key)
                                                         {
                                                            ValidateConstructorArguments(ref key);

                                                            if (key is null)
                                                               throw new global::System.ArgumentNullException(nameof(key));

                                                            this.Key = key;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(key));
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string key);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this.Key;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="key"/>.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static global::TestEnum? Get(string? key)
                                                         {
                                                            if (key is null)
                                                               return default;

                                                            if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                            {
                                                               throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::TestEnum), key);
                                                            }

                                                            return item;
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::TestEnum item)
                                                         {
                                                            if (key is null)
                                                            {
                                                               item = default;
                                                               return false;
                                                            }

                                                            return _itemsLookup.Value.TryGetValue(key, out item);
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::TestEnum item)
                                                         {
                                                            if(global::TestEnum.TryGet(key, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator string?(global::TestEnum? item)
                                                         {
                                                            return item is null ? default : item.Key;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="key">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static explicit operator global::TestEnum?(string? key)
                                                         {
                                                            return global::TestEnum.Get(key);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::TestEnum? other)
                                                         {
                                                            return global::System.Object.ReferenceEquals(this, other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Key.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<string, global::TestEnum>(2, global::System.StringComparer.OrdinalIgnoreCase);

                                                            void AddItem(global::TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (item.Key is null)
                                                                  throw new global::System.ArgumentException($"The \"Key\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (lookup.ContainsKey(item.Key))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                               lookup.Add(item.Key, item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::System.StringComparer.OrdinalIgnoreCase);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }

                                                   """);

      AssertOutput(comparableOutput, _GENERATED_HEADER + """

                                                         partial class TestEnum :
                                                            global::System.IComparable,
                                                            global::System.IComparable<global::TestEnum>
                                                         {
                                                            /// <inheritdoc />
                                                            public int CompareTo(object? obj)
                                                            {
                                                               if(obj is null)
                                                                  return 1;

                                                               if(obj is not global::TestEnum item)
                                                                  throw new global::System.ArgumentException("Argument must be of type \"TestEnum\".", nameof(obj));

                                                               return this.CompareTo(item);
                                                            }

                                                            /// <inheritdoc />
                                                            public int CompareTo(global::TestEnum? obj)
                                                            {
                                                               if(obj is null)
                                                                  return 1;

                                                               return global::System.StringComparer.OrdinalIgnoreCase.Compare(this.Key, obj.Key);
                                                            }
                                                         }

                                                         """);

      AssertOutput(parsableOutput, _GENERATED_HEADER + """

                                                       partial class TestEnum :
                                                          global::System.IParsable<global::TestEnum>
                                                       {
                                                          private static global::Thinktecture.ValidationError? Validate<T>(string key, global::System.IFormatProvider? provider, out global::TestEnum? result)
                                                             where T : global::Thinktecture.IValueObjectFactory<global::TestEnum, string, global::Thinktecture.ValidationError>
                                                          {
                                                             return T.Validate(key, provider, out result);
                                                          }

                                                          /// <inheritdoc />
                                                          public static global::TestEnum Parse(string s, global::System.IFormatProvider? provider)
                                                          {
                                                             var validationError = Validate<global::TestEnum>(s, provider, out var result);

                                                             if(validationError is null)
                                                                return result!;

                                                             throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestEnum\".");
                                                          }

                                                          /// <inheritdoc />
                                                          public static bool TryParse(
                                                             string? s,
                                                             global::System.IFormatProvider? provider,
                                                             [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::TestEnum result)
                                                          {
                                                             if(s is null)
                                                             {
                                                                result = default;
                                                                return false;
                                                             }

                                                             var validationError = Validate<global::TestEnum>(s, provider, out result!);
                                                             return validationError is null;
                                                          }
                                                       }

                                                       """);

      AssertOutput(comparisonOperatorsOutput, _GENERATED_HEADER + """

                                                                  partial class TestEnum :
                                                                     global::System.Numerics.IComparisonOperators<global::TestEnum, global::TestEnum, bool>
                                                                  {

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                     public static bool operator <(global::TestEnum left, global::TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) < 0;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator <=(global::TestEnum left, global::TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) <= 0;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                     public static bool operator >(global::TestEnum left, global::TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) > 0;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator >=(global::TestEnum left, global::TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) >= 0;
                                                                     }
                                                                  }

                                                                  """);

      AssertOutput(equalityComparisonOperators, _GENERATED_HEADER + """

                                                                    partial class TestEnum :
                                                                       global::System.Numerics.IEqualityOperators<global::TestEnum, global::TestEnum, bool>
                                                                    {
                                                                          /// <summary>
                                                                          /// Compares two instances of <see cref="TestEnum"/>.
                                                                          /// </summary>
                                                                          /// <param name="obj">Instance to compare.</param>
                                                                          /// <param name="other">Another instance to compare.</param>
                                                                          /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                          public static bool operator ==(global::TestEnum? obj, global::TestEnum? other)
                                                                          {
                                                                             return global::System.Object.ReferenceEquals(obj, other);
                                                                          }

                                                                          /// <summary>
                                                                          /// Compares two instances of <see cref="TestEnum"/>.
                                                                          /// </summary>
                                                                          /// <param name="obj">Instance to compare.</param>
                                                                          /// <param name="other">Another instance to compare.</param>
                                                                          /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                          public static bool operator !=(global::TestEnum? obj, global::TestEnum? other)
                                                                          {
                                                                             return !(obj == other);
                                                                          }
                                                                    }

                                                                    """);
   }

   [Fact]
   public void Should_generate_string_based_class_with_inner_derived_type_which_is_generic()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                     [SmartEnum<string>]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item_1 = new("Item 1");
                         public static readonly TestEnum Item_2 = new("Item 2");
                         public static readonly TestEnum Item_int_1 = new GenericEnum<int>("GenericEnum<int> 1");
                         public static readonly TestEnum Item_decimal_1 = new GenericEnum<decimal>("GenericEnum<decimal> 1");
                         public static readonly TestEnum Item_decimal_2 = new GenericEnum<decimal>("GenericEnum<decimal> 2");
                         public static readonly TestEnum Item_derived_1 = new DerivedEnum("DerivedEnum 1");
                         public static readonly TestEnum Item_derived_2 = new DerivedEnum("DerivedEnum 2");

                         private class GenericEnum<T> : TestEnum
                         {
                            public DerivedEnum(string key)
                               : base(key)
                            {
                            }
                         }

                         private class UnusedGenericEnum<T> : TestEnum
                         {
                            public UnusedGenericEnum(string key)
                               : base(key)
                            {
                            }
                         }

                         private class DerivedEnum : TestEnum
                         {
                            public DerivedEnum(string key)
                               : base(key)
                            {
                            }
                         }

                         private class UnusedDerivedEnum : TestEnum
                         {
                            public UnusedDerivedEnum(string key)
                               : base(key)
                            {
                            }
                         }

                      }
                   }

                   """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(6);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var derivedTypesOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.DerivedTypes.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
                                                      partial class TestEnum : global::Thinktecture.IEnum<string, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Key);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Key;

                                                            var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public string Key { get; }

                                                         private readonly int _hashCode;

                                                         private TestEnum(string key)
                                                         {
                                                            ValidateConstructorArguments(ref key);

                                                            if (key is null)
                                                               throw new global::System.ArgumentNullException(nameof(key));

                                                            this.Key = key;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(key));
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string key);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this.Key;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="key"/>.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static global::Thinktecture.Tests.TestEnum? Get(string? key)
                                                         {
                                                            if (key is null)
                                                               return default;

                                                            if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                            {
                                                               throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::Thinktecture.Tests.TestEnum), key);
                                                            }

                                                            return item;
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if (key is null)
                                                            {
                                                               item = default;
                                                               return false;
                                                            }

                                                            return _itemsLookup.Value.TryGetValue(key, out item);
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestEnum? item)
                                                         {
                                                            return item is null ? default : item.Key;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="key">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static explicit operator global::Thinktecture.Tests.TestEnum?(string? key)
                                                         {
                                                            return global::Thinktecture.Tests.TestEnum.Get(key);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            return global::System.Object.ReferenceEquals(this, other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Key.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         /// <param name="testEnum3">The item to compare to.</param>
                                                         /// <param name="testEnumAction3">The action to execute if the current item is equal to <paramref name="testEnum3"/>.</param>
                                                         /// <param name="testEnum4">The item to compare to.</param>
                                                         /// <param name="testEnumAction4">The action to execute if the current item is equal to <paramref name="testEnum4"/>.</param>
                                                         /// <param name="testEnum5">The item to compare to.</param>
                                                         /// <param name="testEnumAction5">The action to execute if the current item is equal to <paramref name="testEnum5"/>.</param>
                                                         /// <param name="testEnum6">The item to compare to.</param>
                                                         /// <param name="testEnumAction6">The action to execute if the current item is equal to <paramref name="testEnum6"/>.</param>
                                                         /// <param name="testEnum7">The item to compare to.</param>
                                                         /// <param name="testEnumAction7">The action to execute if the current item is equal to <paramref name="testEnum7"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2,
                                                            TestEnum testEnum3, global::System.Action testEnumAction3,
                                                            TestEnum testEnum4, global::System.Action testEnumAction4,
                                                            TestEnum testEnum5, global::System.Action testEnumAction5,
                                                            TestEnum testEnum6, global::System.Action testEnumAction6,
                                                            TestEnum testEnum7, global::System.Action testEnumAction7)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else if (this == testEnum3)
                                                            {
                                                               testEnumAction3();
                                                            }
                                                            else if (this == testEnum4)
                                                            {
                                                               testEnumAction4();
                                                            }
                                                            else if (this == testEnum5)
                                                            {
                                                               testEnumAction5();
                                                            }
                                                            else if (this == testEnum6)
                                                            {
                                                               testEnumAction6();
                                                            }
                                                            else if (this == testEnum7)
                                                            {
                                                               testEnumAction7();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         /// <param name="testEnum3">The item to compare to.</param>
                                                         /// <param name="testEnumAction3">The action to execute if the current item is equal to <paramref name="testEnum3"/>.</param>
                                                         /// <param name="testEnum4">The item to compare to.</param>
                                                         /// <param name="testEnumAction4">The action to execute if the current item is equal to <paramref name="testEnum4"/>.</param>
                                                         /// <param name="testEnum5">The item to compare to.</param>
                                                         /// <param name="testEnumAction5">The action to execute if the current item is equal to <paramref name="testEnum5"/>.</param>
                                                         /// <param name="testEnum6">The item to compare to.</param>
                                                         /// <param name="testEnumAction6">The action to execute if the current item is equal to <paramref name="testEnum6"/>.</param>
                                                         /// <param name="testEnum7">The item to compare to.</param>
                                                         /// <param name="testEnumAction7">The action to execute if the current item is equal to <paramref name="testEnum7"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2,
                                                            TestEnum testEnum3, global::System.Action<TContext> testEnumAction3,
                                                            TestEnum testEnum4, global::System.Action<TContext> testEnumAction4,
                                                            TestEnum testEnum5, global::System.Action<TContext> testEnumAction5,
                                                            TestEnum testEnum6, global::System.Action<TContext> testEnumAction6,
                                                            TestEnum testEnum7, global::System.Action<TContext> testEnumAction7)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else if (this == testEnum3)
                                                            {
                                                               testEnumAction3(context);
                                                            }
                                                            else if (this == testEnum4)
                                                            {
                                                               testEnumAction4(context);
                                                            }
                                                            else if (this == testEnum5)
                                                            {
                                                               testEnumAction5(context);
                                                            }
                                                            else if (this == testEnum6)
                                                            {
                                                               testEnumAction6(context);
                                                            }
                                                            else if (this == testEnum7)
                                                            {
                                                               testEnumAction7(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         /// <param name="testEnum3">The item to compare to.</param>
                                                         /// <param name="testEnumFunc3">The function to execute if the current item is equal to <paramref name="testEnum3"/>.</param>
                                                         /// <param name="testEnum4">The item to compare to.</param>
                                                         /// <param name="testEnumFunc4">The function to execute if the current item is equal to <paramref name="testEnum4"/>.</param>
                                                         /// <param name="testEnum5">The item to compare to.</param>
                                                         /// <param name="testEnumFunc5">The function to execute if the current item is equal to <paramref name="testEnum5"/>.</param>
                                                         /// <param name="testEnum6">The item to compare to.</param>
                                                         /// <param name="testEnumFunc6">The function to execute if the current item is equal to <paramref name="testEnum6"/>.</param>
                                                         /// <param name="testEnum7">The item to compare to.</param>
                                                         /// <param name="testEnumFunc7">The function to execute if the current item is equal to <paramref name="testEnum7"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2,
                                                            TestEnum testEnum3, global::System.Func<TResult> testEnumFunc3,
                                                            TestEnum testEnum4, global::System.Func<TResult> testEnumFunc4,
                                                            TestEnum testEnum5, global::System.Func<TResult> testEnumFunc5,
                                                            TestEnum testEnum6, global::System.Func<TResult> testEnumFunc6,
                                                            TestEnum testEnum7, global::System.Func<TResult> testEnumFunc7)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else if (this == testEnum3)
                                                            {
                                                               return testEnumFunc3();
                                                            }
                                                            else if (this == testEnum4)
                                                            {
                                                               return testEnumFunc4();
                                                            }
                                                            else if (this == testEnum5)
                                                            {
                                                               return testEnumFunc5();
                                                            }
                                                            else if (this == testEnum6)
                                                            {
                                                               return testEnumFunc6();
                                                            }
                                                            else if (this == testEnum7)
                                                            {
                                                               return testEnumFunc7();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         /// <param name="testEnum3">The item to compare to.</param>
                                                         /// <param name="testEnumFunc3">The function to execute if the current item is equal to <paramref name="testEnum3"/>.</param>
                                                         /// <param name="testEnum4">The item to compare to.</param>
                                                         /// <param name="testEnumFunc4">The function to execute if the current item is equal to <paramref name="testEnum4"/>.</param>
                                                         /// <param name="testEnum5">The item to compare to.</param>
                                                         /// <param name="testEnumFunc5">The function to execute if the current item is equal to <paramref name="testEnum5"/>.</param>
                                                         /// <param name="testEnum6">The item to compare to.</param>
                                                         /// <param name="testEnumFunc6">The function to execute if the current item is equal to <paramref name="testEnum6"/>.</param>
                                                         /// <param name="testEnum7">The item to compare to.</param>
                                                         /// <param name="testEnumFunc7">The function to execute if the current item is equal to <paramref name="testEnum7"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2,
                                                            TestEnum testEnum3, global::System.Func<TContext, TResult> testEnumFunc3,
                                                            TestEnum testEnum4, global::System.Func<TContext, TResult> testEnumFunc4,
                                                            TestEnum testEnum5, global::System.Func<TContext, TResult> testEnumFunc5,
                                                            TestEnum testEnum6, global::System.Func<TContext, TResult> testEnumFunc6,
                                                            TestEnum testEnum7, global::System.Func<TContext, TResult> testEnumFunc7)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else if (this == testEnum3)
                                                            {
                                                               return testEnumFunc3(context);
                                                            }
                                                            else if (this == testEnum4)
                                                            {
                                                               return testEnumFunc4(context);
                                                            }
                                                            else if (this == testEnum5)
                                                            {
                                                               return testEnumFunc5(context);
                                                            }
                                                            else if (this == testEnum6)
                                                            {
                                                               return testEnumFunc6(context);
                                                            }
                                                            else if (this == testEnum7)
                                                            {
                                                               return testEnumFunc7(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         /// <param name="testEnum3">The item to compare to.</param>
                                                         /// <param name="other3">The instance to return if the current item is equal to <paramref name="testEnum3"/>.</param>
                                                         /// <param name="testEnum4">The item to compare to.</param>
                                                         /// <param name="other4">The instance to return if the current item is equal to <paramref name="testEnum4"/>.</param>
                                                         /// <param name="testEnum5">The item to compare to.</param>
                                                         /// <param name="other5">The instance to return if the current item is equal to <paramref name="testEnum5"/>.</param>
                                                         /// <param name="testEnum6">The item to compare to.</param>
                                                         /// <param name="other6">The instance to return if the current item is equal to <paramref name="testEnum6"/>.</param>
                                                         /// <param name="testEnum7">The item to compare to.</param>
                                                         /// <param name="other7">The instance to return if the current item is equal to <paramref name="testEnum7"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2,
                                                            TestEnum testEnum3, TResult other3,
                                                            TestEnum testEnum4, TResult other4,
                                                            TestEnum testEnum5, TResult other5,
                                                            TestEnum testEnum6, TResult other6,
                                                            TestEnum testEnum7, TResult other7)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else if (this == testEnum3)
                                                            {
                                                               return other3;
                                                            }
                                                            else if (this == testEnum4)
                                                            {
                                                               return other4;
                                                            }
                                                            else if (this == testEnum5)
                                                            {
                                                               return other5;
                                                            }
                                                            else if (this == testEnum6)
                                                            {
                                                               return other6;
                                                            }
                                                            else if (this == testEnum7)
                                                            {
                                                               return other7;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(7, global::System.StringComparer.OrdinalIgnoreCase);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (item.Key is null)
                                                                  throw new global::System.ArgumentException($"The \"Key\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (lookup.ContainsKey(item.Key))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                               lookup.Add(item.Key, item);
                                                            }

                                                            AddItem(@Item_1, nameof(@Item_1));
                                                            AddItem(@Item_2, nameof(@Item_2));
                                                            AddItem(@Item_int_1, nameof(@Item_int_1));
                                                            AddItem(@Item_decimal_1, nameof(@Item_decimal_1));
                                                            AddItem(@Item_decimal_2, nameof(@Item_decimal_2));
                                                            AddItem(@Item_derived_1, nameof(@Item_derived_1));
                                                            AddItem(@Item_derived_2, nameof(@Item_derived_2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::System.StringComparer.OrdinalIgnoreCase);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }
                                                   }

                                                   """);

      AssertOutput(derivedTypesOutput, _GENERATED_HEADER + """

                                                           namespace Thinktecture.Tests;

                                                           partial class TestEnum
                                                           {
                                                              [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                              internal static void DerivedTypesModuleInit()
                                                              {
                                                                 var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                                 global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddDerivedType(enumType, typeof(global::Thinktecture.Tests.TestEnum.GenericEnum<>));
                                                                 global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddDerivedType(enumType, typeof(global::Thinktecture.Tests.TestEnum.UnusedGenericEnum<>));
                                                                 global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddDerivedType(enumType, typeof(global::Thinktecture.Tests.TestEnum.DerivedEnum));
                                                                 global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddDerivedType(enumType, typeof(global::Thinktecture.Tests.TestEnum.UnusedDerivedEnum));
                                                              }
                                                           }

                                                           """);
   }

   [Fact]
   public void Should_generate_string_based_validatable_class()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                     [SmartEnum<string>(IsValidatable = true)]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_VALIDATABLE_CLASS_STRING_BASED);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_VALIDATABLE_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
                                                      partial class TestEnum : global::Thinktecture.IEnum<string, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                         global::Thinktecture.IValidatableEnum,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Key);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Key;

                                                            var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, true, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public string Key { get; }

                                                         /// <inheritdoc />
                                                         public bool IsValid { get; }

                                                         /// <summary>
                                                         /// Checks whether current enumeration item is valid.
                                                         /// </summary>
                                                         /// <exception cref="System.InvalidOperationException">The enumeration item is not valid.</exception>
                                                         public void EnsureValid()
                                                         {
                                                            if (!IsValid)
                                                               throw new global::System.InvalidOperationException($"The current enumeration item of type \"TestEnum\" with identifier \"{this.Key}\" is not valid.");
                                                         }

                                                         private readonly int _hashCode;

                                                         private TestEnum(string key)
                                                            : this(key, true)
                                                         {
                                                         }

                                                         private TestEnum(string key, bool isValid)
                                                         {
                                                            ValidateConstructorArguments(ref key, isValid);

                                                            if (key is null)
                                                               throw new global::System.ArgumentNullException(nameof(key));

                                                            this.Key = key;
                                                            this.IsValid = isValid;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(key));
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string key, bool isValid);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this.Key;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="key"/>.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static global::Thinktecture.Tests.TestEnum? Get(string? key)
                                                         {
                                                            if (key is null)
                                                               return default;

                                                            if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                            {
                                                               item = CreateAndCheckInvalidItem(key);
                                                            }

                                                            return item;
                                                         }

                                                         private static global::Thinktecture.Tests.TestEnum CreateAndCheckInvalidItem(string key)
                                                         {
                                                            var item = CreateInvalidItem(key);

                                                            if (item is null)
                                                               throw new global::System.Exception("The implementation of method 'CreateInvalidItem' must not return 'null'.");

                                                            if (item.IsValid)
                                                               throw new global::System.Exception("The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'.");

                                                            return item;
                                                         }

                                                         private static global::Thinktecture.Tests.TestEnum CreateInvalidItem(string key)
                                                         {
                                                            return new global::Thinktecture.Tests.TestEnum(key, false);
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if (key is null)
                                                            {
                                                               item = default;
                                                               return false;
                                                            }

                                                            if(_itemsLookup.Value.TryGetValue(key, out item))
                                                               return true;

                                                            item = CreateAndCheckInvalidItem(key);
                                                            return false;
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               if(key is not null)
                                                                  item = CreateAndCheckInvalidItem(key);
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestEnum? item)
                                                         {
                                                            return item is null ? default : item.Key;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="key">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static explicit operator global::Thinktecture.Tests.TestEnum?(string? key)
                                                         {
                                                            return global::Thinktecture.Tests.TestEnum.Get(key);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            if (other is null)
                                                               return false;

                                                            if (!global::System.Object.ReferenceEquals(GetType(), other.GetType()))
                                                               return false;

                                                            if (global::System.Object.ReferenceEquals(this, other))
                                                               return true;

                                                            if (this.IsValid != other.IsValid)
                                                               return false;

                                                            return global::System.StringComparer.OrdinalIgnoreCase.Equals(this.Key, other.Key);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Key.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(2, global::System.StringComparer.OrdinalIgnoreCase);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (item.Key is null)
                                                                  throw new global::System.ArgumentException($"The \"Key\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (!item.IsValid)
                                                                  throw new global::System.ArgumentException($"All \"public static readonly\" fields of type \"TestEnum\" must be valid but the item \"{itemName}\" with the identifier \"{item.Key}\" is not.");

                                                               if (lookup.ContainsKey(item.Key))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                               lookup.Add(item.Key, item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::System.StringComparer.OrdinalIgnoreCase);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_string_based_validatable_struct()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                     [SmartEnum<string>(IsValidatable = true)]
                   	public readonly partial struct TestEnum
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_STRUCT);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
                                                      partial struct TestEnum : global::Thinktecture.IEnum<string, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                         global::Thinktecture.IValidatableEnum,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<string?, global::Thinktecture.Tests.TestEnum>(global::Thinktecture.Tests.TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Key);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Key;

                                                            var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, true, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public string Key { get; }

                                                         /// <inheritdoc />
                                                         public bool IsValid { get; }

                                                         /// <summary>
                                                         /// Checks whether current enumeration item is valid.
                                                         /// </summary>
                                                         /// <exception cref="System.InvalidOperationException">The enumeration item is not valid.</exception>
                                                         public void EnsureValid()
                                                         {
                                                            if (!IsValid)
                                                               throw new global::System.InvalidOperationException($"The current enumeration item of type \"TestEnum\" with identifier \"{this.Key}\" is not valid.");
                                                         }

                                                         private readonly int _hashCode;

                                                         private TestEnum(string key)
                                                            : this(key, true)
                                                         {
                                                         }

                                                         private TestEnum(string key, bool isValid)
                                                         {
                                                            ValidateConstructorArguments(ref key, isValid);

                                                            if (key is null)
                                                               throw new global::System.ArgumentNullException(nameof(key));

                                                            this.Key = key;
                                                            this.IsValid = isValid;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(key));
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string key, bool isValid);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this.Key;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="key"/>.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         public static global::Thinktecture.Tests.TestEnum Get(string? key)
                                                         {
                                                            if (key is null)
                                                               return default;

                                                            if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                            {
                                                               item = CreateAndCheckInvalidItem(key);
                                                            }

                                                            return item;
                                                         }

                                                         private static global::Thinktecture.Tests.TestEnum CreateAndCheckInvalidItem(string key)
                                                         {
                                                            var item = CreateInvalidItem(key);

                                                            if (item.IsValid)
                                                               throw new global::System.Exception("The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'.");

                                                            return item;
                                                         }

                                                         private static global::Thinktecture.Tests.TestEnum CreateInvalidItem(string key)
                                                         {
                                                            return new global::Thinktecture.Tests.TestEnum(key, false);
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if (key is null)
                                                            {
                                                               item = default;
                                                               return false;
                                                            }

                                                            if(_itemsLookup.Value.TryGetValue(key, out item))
                                                               return true;

                                                            item = CreateAndCheckInvalidItem(key);
                                                            return false;
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               if(key is not null)
                                                                  item = CreateAndCheckInvalidItem(key);
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            return item.Key;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="key">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static explicit operator global::Thinktecture.Tests.TestEnum(string? key)
                                                         {
                                                            return global::Thinktecture.Tests.TestEnum.Get(key);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum other)
                                                         {
                                                            if (this.IsValid != other.IsValid)
                                                               return false;

                                                            return global::System.StringComparer.OrdinalIgnoreCase.Equals(this.Key, other.Key);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Key.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(2, global::System.StringComparer.OrdinalIgnoreCase);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item.Key is null)
                                                                  throw new global::System.ArgumentException($"The \"Key\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (!item.IsValid)
                                                                  throw new global::System.ArgumentException($"All \"public static readonly\" fields of type \"TestEnum\" must be valid but the item \"{itemName}\" with the identifier \"{item.Key}\" is not.");

                                                               if (lookup.ContainsKey(item.Key))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                               lookup.Add(item.Key, item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::System.StringComparer.OrdinalIgnoreCase);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }
                                                   }

                                                   """);

      AssertOutput(comparableOutput, _GENERATED_HEADER + """

                                                         namespace Thinktecture.Tests;

                                                         partial struct TestEnum :
                                                            global::System.IComparable,
                                                            global::System.IComparable<global::Thinktecture.Tests.TestEnum>
                                                         {
                                                            /// <inheritdoc />
                                                            public int CompareTo(object? obj)
                                                            {
                                                               if(obj is null)
                                                                  return 1;

                                                               if(obj is not global::Thinktecture.Tests.TestEnum item)
                                                                  throw new global::System.ArgumentException("Argument must be of type \"TestEnum\".", nameof(obj));

                                                               return this.CompareTo(item);
                                                            }

                                                            /// <inheritdoc />
                                                            public int CompareTo(global::Thinktecture.Tests.TestEnum obj)
                                                            {
                                                               return global::System.StringComparer.OrdinalIgnoreCase.Compare(this.Key, obj.Key);
                                                            }
                                                         }

                                                         """);

      AssertOutput(comparisonOperatorsOutput, _GENERATED_HEADER + """

                                                                  namespace Thinktecture.Tests;

                                                                  partial struct TestEnum :
                                                                     global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestEnum, global::Thinktecture.Tests.TestEnum, bool>
                                                                  {

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                     public static bool operator <(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) < 0;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator <=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) <= 0;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                     public static bool operator >(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) > 0;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator >=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) >= 0;
                                                                     }
                                                                  }

                                                                  """);

      AssertOutput(parsableOutput, _GENERATED_HEADER + """

                                                       namespace Thinktecture.Tests;

                                                       partial struct TestEnum :
                                                          global::System.IParsable<global::Thinktecture.Tests.TestEnum>
                                                       {
                                                          private static global::Thinktecture.ValidationError? Validate<T>(string key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestEnum result)
                                                             where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>
                                                          {
                                                             return T.Validate(key, provider, out result);
                                                          }

                                                          /// <inheritdoc />
                                                          public static global::Thinktecture.Tests.TestEnum Parse(string s, global::System.IFormatProvider? provider)
                                                          {
                                                             var validationError = Validate<global::Thinktecture.Tests.TestEnum>(s, provider, out var result);
                                                             return result!;
                                                          }

                                                          /// <inheritdoc />
                                                          public static bool TryParse(
                                                             string? s,
                                                             global::System.IFormatProvider? provider,
                                                             [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum result)
                                                          {
                                                             if(s is null)
                                                             {
                                                                result = default;
                                                                return false;
                                                             }

                                                             var validationError = Validate<global::Thinktecture.Tests.TestEnum>(s, provider, out result!);
                                                             return true;
                                                          }
                                                       }

                                                       """);
   }

   [Fact]
   public void Should_generate_advanced_string_based_validatable_class()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                     [SmartEnum<string>(IsValidatable = true, KeyMemberName = "Name")]
                     [ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
                     [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
                   	public partial class TestEnum
                   	{
                   		 public static readonly TestEnum Item1 = new("Item1", 1, -1, "ReferenceProperty1", "NullableReferenceProperty1", 11, "ReferenceField1");
                         public static readonly TestEnum Item2 = new DerivedEnum("Item2", 2, 2, "ReferenceProperty2", "NullableReferenceProperty2", 22, "ReferenceField2");

                         public int StructProperty { get; }
                         public int? NullableStructProperty { get; }
                         public string ReferenceProperty { get; }
                         public string? NullableReferenceProperty { get; }
                         public readonly int StructField;
                         public readonly string ReferenceField;

                         static partial void ValidateConstructorArguments(
                            int name, bool isValid,
                            int structProperty, int? nullableStructProperty,
                            string referenceProperty, string? nullableReferenceProperty,
                            int structField, string referenceField)
                         {
                         }

                         private static ProductCategory CreateInvalidItem(string name)
                         {
                            return new(name, false, 0, null, String.Empty, null, 0, null);
                         }

                         private class DerivedEnum : EnumWithDerivedType
                         {
                            public DerivedEnum(
                               int name, bool isValid,
                               int structProperty, int? nullableStructProperty,
                               string referenceProperty, string? nullableReferenceProperty,
                               int structField, string referenceField)
                               : base(name, isValid, structProperty, nullableStructProperty, referenceProperty, nullableReferenceProperty, structField, referenceField)
                            {
                            }
                         }
                      }
                   }
                   """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_VALIDATABLE_CLASS_STRING_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_VALIDATABLE_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
                                                      partial class TestEnum : global::Thinktecture.IEnum<string, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                         global::Thinktecture.IValidatableEnum,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static name => global::Thinktecture.Tests.TestEnum.Get(name);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Name);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Name;

                                                            var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, true, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public string Name { get; }

                                                         /// <inheritdoc />
                                                         public bool IsValid { get; }

                                                         /// <summary>
                                                         /// Checks whether current enumeration item is valid.
                                                         /// </summary>
                                                         /// <exception cref="System.InvalidOperationException">The enumeration item is not valid.</exception>
                                                         public void EnsureValid()
                                                         {
                                                            if (!IsValid)
                                                               throw new global::System.InvalidOperationException($"The current enumeration item of type \"TestEnum\" with identifier \"{this.Name}\" is not valid.");
                                                         }

                                                         private readonly int _hashCode;

                                                         private TestEnum(string name, int structProperty, int? nullableStructProperty, string referenceProperty, string? nullableReferenceProperty, int structField, string referenceField)
                                                            : this(name, true, structProperty, nullableStructProperty, referenceProperty, nullableReferenceProperty, structField, referenceField)
                                                         {
                                                         }

                                                         private TestEnum(string name, bool isValid, int structProperty, int? nullableStructProperty, string referenceProperty, string? nullableReferenceProperty, int structField, string referenceField)
                                                         {
                                                            ValidateConstructorArguments(ref name, isValid, ref structProperty, ref nullableStructProperty, ref referenceProperty, ref nullableReferenceProperty, ref structField, ref referenceField);

                                                            if (name is null)
                                                               throw new global::System.ArgumentNullException(nameof(name));

                                                            this.Name = name;
                                                            this.IsValid = isValid;
                                                            this.StructProperty = structProperty;
                                                            this.NullableStructProperty = nullableStructProperty;
                                                            this.ReferenceProperty = referenceProperty;
                                                            this.NullableReferenceProperty = nullableReferenceProperty;
                                                            this.StructField = structField;
                                                            this.ReferenceField = referenceField;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer.GetHashCode(name));
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string name, bool isValid, ref int structProperty, ref int? nullableStructProperty, ref string referenceProperty, ref string? nullableReferenceProperty, ref int structField, ref string referenceField);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this.Name;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="name"/>.
                                                         /// </summary>
                                                         /// <param name="name">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="name"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("name")]
                                                         public static global::Thinktecture.Tests.TestEnum? Get(string? name)
                                                         {
                                                            if (name is null)
                                                               return default;

                                                            if (!_itemsLookup.Value.TryGetValue(name, out var item))
                                                            {
                                                               item = CreateAndCheckInvalidItem(name);
                                                            }

                                                            return item;
                                                         }

                                                         private static global::Thinktecture.Tests.TestEnum CreateAndCheckInvalidItem(string name)
                                                         {
                                                            var item = CreateInvalidItem(name);

                                                            if (item is null)
                                                               throw new global::System.Exception("The implementation of method 'CreateInvalidItem' must not return 'null'.");

                                                            if (item.IsValid)
                                                               throw new global::System.Exception("The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'.");

                                                            if (_itemsLookup.Value.ContainsKey(item.Name))
                                                               throw new global::System.Exception("The implementation of method 'CreateInvalidItem' must not return an instance with property 'Name' equals to one of a valid item.");

                                                            return item;
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="name"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="name">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="name"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string name, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if (name is null)
                                                            {
                                                               item = default;
                                                               return false;
                                                            }

                                                            if(_itemsLookup.Value.TryGetValue(name, out item))
                                                               return true;

                                                            item = CreateAndCheckInvalidItem(name);
                                                            return false;
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="name"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="name">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="name"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string name, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if(global::Thinktecture.Tests.TestEnum.TryGet(name, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               if(name is not null)
                                                                  item = CreateAndCheckInvalidItem(name);
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{name}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Name"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestEnum? item)
                                                         {
                                                            return item is null ? default : item.Name;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="name">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="name"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("name")]
                                                         public static explicit operator global::Thinktecture.Tests.TestEnum?(string? name)
                                                         {
                                                            return global::Thinktecture.Tests.TestEnum.Get(name);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            if (other is null)
                                                               return false;

                                                            if (!global::System.Object.ReferenceEquals(GetType(), other.GetType()))
                                                               return false;

                                                            if (global::System.Object.ReferenceEquals(this, other))
                                                               return true;

                                                            if (this.IsValid != other.IsValid)
                                                               return false;

                                                            return global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer.Equals(this.Name, other.Name);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Name.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(2, global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (item.Name is null)
                                                                  throw new global::System.ArgumentException($"The \"Name\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (!item.IsValid)
                                                                  throw new global::System.ArgumentException($"All \"public static readonly\" fields of type \"TestEnum\" must be valid but the item \"{itemName}\" with the identifier \"{item.Name}\" is not.");

                                                               if (lookup.ContainsKey(item.Name))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Name}\".");

                                                               lookup.Add(item.Name, item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }
                                                   }

                                                   """);

      AssertOutput(comparableOutput, _GENERATED_HEADER + """

                                                         namespace Thinktecture.Tests;

                                                         partial class TestEnum :
                                                            global::System.IComparable,
                                                            global::System.IComparable<global::Thinktecture.Tests.TestEnum>
                                                         {
                                                            /// <inheritdoc />
                                                            public int CompareTo(object? obj)
                                                            {
                                                               if(obj is null)
                                                                  return 1;

                                                               if(obj is not global::Thinktecture.Tests.TestEnum item)
                                                                  throw new global::System.ArgumentException("Argument must be of type \"TestEnum\".", nameof(obj));

                                                               return this.CompareTo(item);
                                                            }

                                                            /// <inheritdoc />
                                                            public int CompareTo(global::Thinktecture.Tests.TestEnum? obj)
                                                            {
                                                               if(obj is null)
                                                                  return 1;

                                                               return global::Thinktecture.ComparerAccessors.StringOrdinal.Comparer.Compare(this.Name, obj.Name);
                                                            }
                                                         }

                                                         """);

      AssertOutput(comparisonOperators, _GENERATED_HEADER + """

                                                            namespace Thinktecture.Tests;

                                                            partial class TestEnum :
                                                               global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestEnum, global::Thinktecture.Tests.TestEnum, bool>
                                                            {

                                                               /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                               public static bool operator <(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                               {
                                                                  global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                  global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                  return global::Thinktecture.ComparerAccessors.StringOrdinal.Comparer.Compare(left.Name, right.Name) < 0;
                                                               }

                                                               /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                               public static bool operator <=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                               {
                                                                  global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                  global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                  return global::Thinktecture.ComparerAccessors.StringOrdinal.Comparer.Compare(left.Name, right.Name) <= 0;
                                                               }

                                                               /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                               public static bool operator >(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                               {
                                                                  global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                  global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                  return global::Thinktecture.ComparerAccessors.StringOrdinal.Comparer.Compare(left.Name, right.Name) > 0;
                                                               }

                                                               /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                               public static bool operator >=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                               {
                                                                  global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                  global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                  return global::Thinktecture.ComparerAccessors.StringOrdinal.Comparer.Compare(left.Name, right.Name) >= 0;
                                                               }
                                                            }

                                                            """);
   }

   [Fact]
   public void Should_generate_int_based_class()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                     [SmartEnum<int>]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new(1);
                         public static readonly TestEnum Item2 = new(2);
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(6);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var formattable = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(formattable, _FORMATTABLE_OUTPUT_CLASS);
      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_INT_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_CLASS_INT_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);
      AssertOutput(mainOutput, _MAIN_OUTPUT_INT_BASED_CLASS);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_INT_BASED);
   }

   [Fact]
   public void Should_generate_int_based_class_with_ComparisonOperators_DefaultWithKeyTypeOverloads()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                     [SmartEnum<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new(1);
                         public static readonly TestEnum Item2 = new(2);
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(6);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var formattable = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(formattable, _FORMATTABLE_OUTPUT_CLASS);
      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_INT_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_CLASS_INT_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS_INT_BASED_WITH_KEY_OVERLOADS);
      AssertOutput(mainOutput, _MAIN_OUTPUT_INT_BASED_CLASS);

      AssertOutput(comparisonOperatorsOutput, _GENERATED_HEADER + """

                                                                  namespace Thinktecture.Tests;

                                                                  partial class TestEnum :
                                                                     global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestEnum, global::Thinktecture.Tests.TestEnum, bool>,
                                                                     global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestEnum, int, bool>
                                                                  {
                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                     public static bool operator <(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left.Key < right.Key;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator <=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left.Key <= right.Key;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                     public static bool operator >(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left.Key > right.Key;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator >=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left.Key >= right.Key;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                     public static bool operator <(global::Thinktecture.Tests.TestEnum left, int right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        return left.Key < right;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                     public static bool operator <(int left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left < right.Key;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator <=(global::Thinktecture.Tests.TestEnum left, int right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        return left.Key <= right;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator <=(int left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left <= right.Key;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                     public static bool operator >(global::Thinktecture.Tests.TestEnum left, int right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        return left.Key > right;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                     public static bool operator >(int left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left > right.Key;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator >=(global::Thinktecture.Tests.TestEnum left, int right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        return left.Key >= right;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator >=(int left, global::Thinktecture.Tests.TestEnum right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left >= right.Key;
                                                                     }
                                                                  }

                                                                  """);
   }

   [Fact]
   public void Should_generate_int_based_class_with_ValueObjectFactoryAttribute()
   {
      var source = """

                   using System;

                   namespace Thinktecture.Tests
                   {
                      [SmartEnum<int>]
                      [ValueObjectFactory<string>]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new(1);
                         public static readonly TestEnum Item2 = new(2);
                      }
                   }

                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(6);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var formattable = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(formattable, _FORMATTABLE_OUTPUT_CLASS);
      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_INT_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_CLASS_STRING_BASED); // string-based due to [ValueObjectFactory<string>]
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_INT_BASED);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, int, global::Thinktecture.ValidationError>))]
                                                      partial class TestEnum : global::Thinktecture.IEnum<int, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<int, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, int>(static item => item.Key);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, int>> convertToKeyExpression = static item => item.Key;

                                                            var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(int), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<int, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<int, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public int Key { get; }

                                                         private readonly int _hashCode;

                                                         private TestEnum(int key)
                                                         {
                                                            ValidateConstructorArguments(ref key);

                                                            this.Key = key;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), key.GetHashCode());
                                                         }

                                                         static partial void ValidateConstructorArguments(ref int key);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         int global::Thinktecture.IValueObjectConvertable<int>.ToValue()
                                                         {
                                                            return this.Key;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="key"/>.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
                                                         public static global::Thinktecture.Tests.TestEnum Get(int key)
                                                         {
                                                            if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                            {
                                                               throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::Thinktecture.Tests.TestEnum), key);
                                                            }

                                                            return item;
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] int key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            return _itemsLookup.Value.TryGetValue(key, out item);
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] int key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="int"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator int(global::Thinktecture.Tests.TestEnum? item)
                                                         {
                                                            return item is null ? default : item.Key;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="int"/>.
                                                         /// </summary>
                                                         /// <param name="key">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static explicit operator global::Thinktecture.Tests.TestEnum?(int key)
                                                         {
                                                            return global::Thinktecture.Tests.TestEnum.Get(key);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            return global::System.Object.ReferenceEquals(this, other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Key.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<int, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<int, global::Thinktecture.Tests.TestEnum>(2);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (lookup.ContainsKey(item.Key))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                               lookup.Add(item.Key, item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_int_based_class_with_ValueObjectFactoryAttribute_and_UseForSerialization()
   {
      var source = """

                   using System;

                   namespace Thinktecture.Tests
                   {
                      [SmartEnum<int>]
                      [ValueObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new(1);
                         public static readonly TestEnum Item2 = new(2);
                      }
                   }

                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(6);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var formattable = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(formattable, _FORMATTABLE_OUTPUT_CLASS);
      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_INT_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_CLASS_STRING_BASED); // string-based due to [ValueObjectFactory<string>]
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_INT_BASED);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, int, global::Thinktecture.ValidationError>))]
                                                      partial class TestEnum : global::Thinktecture.IEnum<int, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>,
                                                         global::Thinktecture.IValueObjectConvertable<string>,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<int, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, int>(static item => item.Key);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, int>> convertToKeyExpression = static item => item.Key;

                                                            var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(int), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<int, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<int, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public int Key { get; }

                                                         private readonly int _hashCode;

                                                         private TestEnum(int key)
                                                         {
                                                            ValidateConstructorArguments(ref key);

                                                            this.Key = key;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), key.GetHashCode());
                                                         }

                                                         static partial void ValidateConstructorArguments(ref int key);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         int global::Thinktecture.IValueObjectConvertable<int>.ToValue()
                                                         {
                                                            return this.Key;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="key"/>.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
                                                         public static global::Thinktecture.Tests.TestEnum Get(int key)
                                                         {
                                                            if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                            {
                                                               throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::Thinktecture.Tests.TestEnum), key);
                                                            }

                                                            return item;
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] int key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            return _itemsLookup.Value.TryGetValue(key, out item);
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] int key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="int"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator int(global::Thinktecture.Tests.TestEnum? item)
                                                         {
                                                            return item is null ? default : item.Key;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="int"/>.
                                                         /// </summary>
                                                         /// <param name="key">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static explicit operator global::Thinktecture.Tests.TestEnum?(int key)
                                                         {
                                                            return global::Thinktecture.Tests.TestEnum.Get(key);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            return global::System.Object.ReferenceEquals(this, other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Key.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<int, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<int, global::Thinktecture.Tests.TestEnum>(2);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (lookup.ContainsKey(item.Key))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                               lookup.Add(item.Key, item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_class_with_abstract_property()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                   	[SmartEnum<string>]
                   	public abstract partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = null!;
                         public static readonly TestEnum Item2 = null!;

                         public abstract int Value { get; }

                         private sealed class ConcreteEnum : TestEnum
                         {
                            public override int Value => 100;

                            public ConcreteEnum(int key)
                               : base(key)
                            {
                            }
                         }
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(6);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;
      var derivedTypes = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.DerivedTypes.g.cs")).Value;

      AssertOutput(mainOutput, _MAIN_OUTPUT_STRING_BASED_CLASS);
      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);
      AssertOutput(derivedTypes, _GENERATED_HEADER + """

                                                     namespace Thinktecture.Tests;

                                                     partial class TestEnum
                                                     {
                                                        [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                        internal static void DerivedTypesModuleInit()
                                                        {
                                                           var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                           global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddDerivedType(enumType, typeof(global::Thinktecture.Tests.TestEnum.ConcreteEnum));
                                                        }
                                                     }

                                                     """);
   }

   [Fact]
   public void Should_take_over_nullability_of_generic_members()
   {
      var source = """
                   using System;

                   namespace Thinktecture.Tests
                   {
                   	[SmartEnum<string>]
                   	public abstract partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = null!;
                         public static readonly TestEnum Item2 = null!;

                         public Func<string?, Task<string?>?>? Prop1 { get; }
                      }
                   }
                   """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(IEnum<>).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperators = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
                                                      partial class TestEnum : global::Thinktecture.IEnum<string, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
                                                         global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            var convertFromKey = new global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static key => global::Thinktecture.Tests.TestEnum.Get(key);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Key);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Key;

                                                            var enumType = typeof(global::Thinktecture.Tests.TestEnum);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
                                                         }

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>> _itemsLookup
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                                                                                = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Value.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

                                                         /// <summary>
                                                         /// Gets all valid items.
                                                         /// </summary>
                                                         public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

                                                         /// <summary>
                                                         /// The identifier of this item.
                                                         /// </summary>
                                                         public string Key { get; }

                                                         private readonly int _hashCode;

                                                         private TestEnum(string key, global::System.Func<string?, Task<string?>?>? prop1)
                                                         {
                                                            ValidateConstructorArguments(ref key, ref prop1);

                                                            if (key is null)
                                                               throw new global::System.ArgumentNullException(nameof(key));

                                                            this.Key = key;
                                                            this.Prop1 = prop1;
                                                            this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(key));
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string key, ref global::System.Func<string?, Task<string?>?>? prop1);

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this.Key;
                                                         }

                                                         /// <summary>
                                                         /// Gets an enumeration item for provided <paramref name="key"/>.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
                                                         /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static global::Thinktecture.Tests.TestEnum? Get(string? key)
                                                         {
                                                            if (key is null)
                                                               return default;

                                                            if (!_itemsLookup.Value.TryGetValue(key, out var item))
                                                            {
                                                               throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::Thinktecture.Tests.TestEnum), key);
                                                            }

                                                            return item;
                                                         }

                                                         /// <summary>
                                                         /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
                                                         public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if (key is null)
                                                            {
                                                               item = default;
                                                               return false;
                                                            }

                                                            return _itemsLookup.Value.TryGetValue(key, out item);
                                                         }

                                                         /// <summary>
                                                         /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
                                                         /// </summary>
                                                         /// <param name="key">The identifier to return an enumeration item for.</param>
                                                         /// <param name="provider">An object that provides culture-specific formatting information.</param>
                                                         /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
                                                         /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
                                                         public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string key, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
                                                         {
                                                            if(global::Thinktecture.Tests.TestEnum.TryGet(key, out item))
                                                            {
                                                               return null;
                                                            }
                                                            else
                                                            {
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{key}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="item">Item to covert.</param>
                                                         /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestEnum? item)
                                                         {
                                                            return item is null ? default : item.Key;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="key">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
                                                         public static explicit operator global::Thinktecture.Tests.TestEnum?(string? key)
                                                         {
                                                            return global::Thinktecture.Tests.TestEnum.Get(key);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestEnum? other)
                                                         {
                                                            return global::System.Object.ReferenceEquals(this, other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return _hashCode;
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this.Key.ToString();
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch(
                                                            TestEnum testEnum1, global::System.Action testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes an action depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumAction1">The action to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumAction2">The action to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public void Switch<TContext>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Action<TContext> testEnumAction1,
                                                            TestEnum testEnum2, global::System.Action<TContext> testEnumAction2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               testEnumAction1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               testEnumAction2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No action provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TResult>(
                                                            TestEnum testEnum1, global::System.Func<TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1();
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2();
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Executes a function depending on the current item.
                                                         /// </summary>
                                                         /// <param name="context">Context to be passed to the callbacks.</param>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="testEnumFunc1">The function to execute if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="testEnumFunc2">The function to execute if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Switch<TContext, TResult>(
                                                            TContext context,
                                                            TestEnum testEnum1, global::System.Func<TContext, TResult> testEnumFunc1,
                                                            TestEnum testEnum2, global::System.Func<TContext, TResult> testEnumFunc2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return testEnumFunc1(context);
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return testEnumFunc2(context);
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No function provided for the item '{this}'.");
                                                            }
                                                         }

                                                         /// <summary>
                                                         /// Maps an item to an instance of type <typeparamref name="TResult"/>.
                                                         /// </summary>
                                                         /// <param name="testEnum1">The item to compare to.</param>
                                                         /// <param name="other1">The instance to return if the current item is equal to <paramref name="testEnum1"/>.</param>
                                                         /// <param name="testEnum2">The item to compare to.</param>
                                                         /// <param name="other2">The instance to return if the current item is equal to <paramref name="testEnum2"/>.</param>
                                                         public TResult Map<TResult>(
                                                            TestEnum testEnum1, TResult other1,
                                                            TestEnum testEnum2, TResult other2)
                                                         {
                                                            if (this == testEnum1)
                                                            {
                                                               return other1;
                                                            }
                                                            else if (this == testEnum2)
                                                            {
                                                               return other2;
                                                            }
                                                            else
                                                            {
                                                               throw new global::System.ArgumentOutOfRangeException($"No instance provided for the item '{this}'.");
                                                            }
                                                         }

                                                         private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> GetLookup()
                                                         {
                                                            var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(2, global::System.StringComparer.OrdinalIgnoreCase);

                                                            void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
                                                            {
                                                               if (item is null)
                                                                  throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (item.Key is null)
                                                                  throw new global::System.ArgumentException($"The \"Key\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

                                                               if (lookup.ContainsKey(item.Key))
                                                                  throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

                                                               lookup.Add(item.Key, item);
                                                            }

                                                            AddItem(@Item1, nameof(@Item1));
                                                            AddItem(@Item2, nameof(@Item2));

                                                   #if NET8_0_OR_GREATER
                                                            return global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::System.StringComparer.OrdinalIgnoreCase);
                                                   #else
                                                            return lookup;
                                                   #endif
                                                         }
                                                      }
                                                   }

                                                   """);
      AssertOutput(comparableOutput, _COMPARABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(parsableOutput, _PARSABLE_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_OUTPUT_CLASS_STRING_BASED);
      AssertOutput(equalityComparisonOperators, _EQUALITY_COMPARABLE_OPERATORS_CLASS);
   }
}
