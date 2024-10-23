using System.Linq;
using Thinktecture.CodeAnalysis.ValueObjects;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class ValueObjectSourceGeneratorTests : SourceGeneratorTestsBase
{
   public ValueObjectSourceGeneratorTests(ITestOutputHelper output)
      : base(output)
   {
   }

   private const string _MAIN_OUTPUT_INT_BASED_STRUCT = _GENERATED_HEADER + """

                                                                            namespace Thinktecture.Tests
                                                                            {
                                                                               [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]
                                                                               [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>))]
                                                                               readonly partial struct TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject>,
                                                                                  global::Thinktecture.IKeyedValueObject<int>,
                                                                                  global::Thinktecture.IValueObjectConvertable<int>,
                                                                                  global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>
                                                                               {
                                                                                  [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                                                  internal static void ModuleInit()
                                                                                  {
                                                                                     global::System.Func<int, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                                                     global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                                                     global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                                                     var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, int>(static item => item._value);
                                                                                     global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, int>> convertToKeyExpression = static obj => obj._value;

                                                                                     var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                                                     var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(int), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                                                     global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                                                  }

                                                                                  private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                                                  /// <summary>
                                                                                  /// The identifier of this object.
                                                                                  /// </summary>
                                                                                  private readonly int _value;

                                                                                  public static global::Thinktecture.ValidationError? Validate(int @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject obj)
                                                                                  {
                                                                                     global::Thinktecture.ValidationError? validationError = null;
                                                                                     ValidateFactoryArguments(ref validationError, ref @value);

                                                                                     if (validationError is null)
                                                                                     {
                                                                                        obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                                                        obj.FactoryPostInit();
                                                                                     }
                                                                                     else
                                                                                     {
                                                                                        obj = default;
                                                                                     }

                                                                                     return validationError;
                                                                                  }

                                                                                  public static global::Thinktecture.Tests.TestValueObject Create(int @value)
                                                                                  {
                                                                                     var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject obj);

                                                                                     if (validationError is not null)
                                                                                        throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                                                     return obj!;
                                                                                  }

                                                                                  public static bool TryCreate(int @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj)
                                                                                  {
                                                                                     return TryCreate(@value, out obj, out _);
                                                                                  }

                                                                                  public static bool TryCreate(
                                                                                     int @value,
                                                                                     [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj,
                                                                                     [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                                                  {
                                                                                     validationError = Validate(@value, null, out obj);

                                                                                     return validationError is null;
                                                                                  }

                                                                                  static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, ref int @value);

                                                                                  partial void FactoryPostInit();

                                                                                  /// <summary>
                                                                                  /// Gets the identifier of the item.
                                                                                  /// </summary>
                                                                                  [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                                                  int global::Thinktecture.IValueObjectConvertable<int>.ToValue()
                                                                                  {
                                                                                     return this._value;
                                                                                  }

                                                                                  /// <summary>
                                                                                  /// Implicit conversion to the type <see cref="int"/>.
                                                                                  /// </summary>
                                                                                  /// <param name="obj">Object to covert.</param>
                                                                                  /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                                                  [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                                                  public static implicit operator int?(global::Thinktecture.Tests.TestValueObject? obj)
                                                                                  {
                                                                                     return obj?._value;
                                                                                  }

                                                                                  /// <summary>
                                                                                  /// Implicit conversion to the type <see cref="int"/>.
                                                                                  /// </summary>
                                                                                  /// <param name="obj">Object to covert.</param>
                                                                                  /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/>.</returns>
                                                                                  public static implicit operator int(global::Thinktecture.Tests.TestValueObject obj)
                                                                                  {
                                                                                     return obj._value;
                                                                                  }

                                                                                  /// <summary>
                                                                                  /// Explicit conversion from the type <see cref="int"/>.
                                                                                  /// </summary>
                                                                                  /// <param name="value">Value to covert.</param>
                                                                                  /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                                                  public static explicit operator global::Thinktecture.Tests.TestValueObject(int @value)
                                                                                  {
                                                                                     return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                                                  }

                                                                                  private TestValueObject(int @value)
                                                                                  {
                                                                                     ValidateConstructorArguments(ref @value);

                                                                                     this._value = @value;
                                                                                  }

                                                                                  static partial void ValidateConstructorArguments(ref int @value);

                                                                                  /// <inheritdoc />
                                                                                  public override bool Equals(object? other)
                                                                                  {
                                                                                     return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                                                  }

                                                                                  /// <inheritdoc />
                                                                                  public bool Equals(global::Thinktecture.Tests.TestValueObject other)
                                                                                  {
                                                                                     return this._value.Equals(other._value);
                                                                                  }

                                                                                  /// <inheritdoc />
                                                                                  public override int GetHashCode()
                                                                                  {
                                                                                     return global::System.HashCode.Combine(_typeHashCode, this._value);
                                                                                  }

                                                                                  /// <inheritdoc />
                                                                                  public override string ToString()
                                                                                  {
                                                                                     return this._value.ToString();
                                                                                  }
                                                                               }
                                                                            }

                                                                            """;

   private const string _GENERATED_HEADER = """
                                            // <auto-generated />
                                            #nullable enable

                                            """;

   private const string _COMPARISON_OPERATORS_STRUCT = _GENERATED_HEADER + """

                                                                           namespace Thinktecture.Tests;

                                                                           partial struct TestValueObject :
                                                                              global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>
                                                                           {
                                                                              /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                              public static bool operator <(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                              {
                                                                                 return left._value < right._value;
                                                                              }

                                                                              /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                              public static bool operator <=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                              {
                                                                                 return left._value <= right._value;
                                                                              }

                                                                              /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                              public static bool operator >(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                              {
                                                                                 return left._value > right._value;
                                                                              }

                                                                              /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                              public static bool operator >=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                              {
                                                                                 return left._value >= right._value;
                                                                              }
                                                                           }

                                                                           """;

   private const string _COMPARISON_OPERATORS_CLASS = _GENERATED_HEADER + """

                                                                          namespace Thinktecture.Tests;

                                                                          partial class TestValueObject :
                                                                             global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>
                                                                          {
                                                                             /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                             public static bool operator <(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                             {
                                                                                global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                return left._value < right._value;
                                                                             }

                                                                             /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                             public static bool operator <=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                             {
                                                                                global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                return left._value <= right._value;
                                                                             }

                                                                             /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                             public static bool operator >(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                             {
                                                                                global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                return left._value > right._value;
                                                                             }

                                                                             /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                             public static bool operator >=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                             {
                                                                                global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                return left._value >= right._value;
                                                                             }
                                                                          }

                                                                          """;

   private const string _COMPARISON_OPERATORS_CLASS_STRING = _GENERATED_HEADER + """

                                                                                 namespace Thinktecture.Tests;

                                                                                 partial class TestValueObject :
                                                                                    global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>
                                                                                 {

                                                                                    /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                                    public static bool operator <(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                    {
                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                       return global::System.StringComparer.OrdinalIgnoreCase.Compare(left._value, right._value) < 0;
                                                                                    }

                                                                                    /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                                    public static bool operator <=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                    {
                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                       return global::System.StringComparer.OrdinalIgnoreCase.Compare(left._value, right._value) <= 0;
                                                                                    }

                                                                                    /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                                    public static bool operator >(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                    {
                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                       return global::System.StringComparer.OrdinalIgnoreCase.Compare(left._value, right._value) > 0;
                                                                                    }

                                                                                    /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                                    public static bool operator >=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                    {
                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                       return global::System.StringComparer.OrdinalIgnoreCase.Compare(left._value, right._value) >= 0;
                                                                                    }
                                                                                 }

                                                                                 """;

   private const string _COMPARISON_OPERATORS_STRUCT_STRING = _GENERATED_HEADER + """

                                                                                  namespace Thinktecture.Tests;

                                                                                  partial struct TestValueObject :
                                                                                     global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>
                                                                                  {

                                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                                     public static bool operator <(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                     {
                                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left._value, right._value) < 0;
                                                                                     }

                                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                                     public static bool operator <=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                     {
                                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left._value, right._value) <= 0;
                                                                                     }

                                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                                     public static bool operator >(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                     {
                                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left._value, right._value) > 0;
                                                                                     }

                                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                                     public static bool operator >=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                     {
                                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Compare(left._value, right._value) >= 0;
                                                                                     }
                                                                                  }

                                                                                  """;

   private const string _COMPARISON_OPERATORS_STRING_WITH_ORDINAL_COMPARER = _GENERATED_HEADER + """

                                                                                                 namespace Thinktecture.Tests;

                                                                                                 partial class TestValueObject :
                                                                                                    global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>
                                                                                                 {

                                                                                                    /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                                                    public static bool operator <(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                                    {
                                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                                       return global::Thinktecture.ComparerAccessors.StringOrdinal.Comparer.Compare(left._value, right._value) < 0;
                                                                                                    }

                                                                                                    /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                                                    public static bool operator <=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                                    {
                                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                                       return global::Thinktecture.ComparerAccessors.StringOrdinal.Comparer.Compare(left._value, right._value) <= 0;
                                                                                                    }

                                                                                                    /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                                                    public static bool operator >(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                                    {
                                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                                       return global::Thinktecture.ComparerAccessors.StringOrdinal.Comparer.Compare(left._value, right._value) > 0;
                                                                                                    }

                                                                                                    /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                                                    public static bool operator >=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                                                    {
                                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                                       global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                                       return global::Thinktecture.ComparerAccessors.StringOrdinal.Comparer.Compare(left._value, right._value) >= 0;
                                                                                                    }
                                                                                                 }

                                                                                                 """;

   private const string _ADDITION_OPERATORS_STRUCT = _GENERATED_HEADER + """

                                                                         namespace Thinktecture.Tests;

                                                                         partial struct TestValueObject :
                                                                            global::System.Numerics.IAdditionOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>
                                                                         {
                                                                            /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
                                                                            public static global::Thinktecture.Tests.TestValueObject operator +(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                            {
                                                                               return Create((left._value + right._value));
                                                                            }

                                                                            /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
                                                                            public static global::Thinktecture.Tests.TestValueObject operator checked +(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                            {
                                                                               return Create(checked((left._value + right._value)));
                                                                            }
                                                                         }

                                                                         """;

   private const string _ADDITION_OPERATORS_CLASS = _GENERATED_HEADER + """

                                                                        namespace Thinktecture.Tests;

                                                                        partial class TestValueObject :
                                                                           global::System.Numerics.IAdditionOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>
                                                                        {
                                                                           /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
                                                                           public static global::Thinktecture.Tests.TestValueObject operator +(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                           {
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                              return Create((left._value + right._value));
                                                                           }

                                                                           /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
                                                                           public static global::Thinktecture.Tests.TestValueObject operator checked +(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                           {
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                              return Create(checked((left._value + right._value)));
                                                                           }
                                                                        }

                                                                        """;

   private const string _SUBTRACTION_OPERATORS_STRUCT = _GENERATED_HEADER + """

                                                                            namespace Thinktecture.Tests;

                                                                            partial struct TestValueObject :
                                                                               global::System.Numerics.ISubtractionOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>
                                                                            {
                                                                               /// <inheritdoc cref="global::System.Numerics.ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)" />
                                                                               public static global::Thinktecture.Tests.TestValueObject operator -(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                               {
                                                                                  return Create((left._value - right._value));
                                                                               }

                                                                               /// <inheritdoc cref="global::System.Numerics.ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)" />
                                                                               public static global::Thinktecture.Tests.TestValueObject operator checked -(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                               {
                                                                                  return Create(checked((left._value - right._value)));
                                                                               }
                                                                            }

                                                                            """;

   private const string _SUBTRACTION_OPERATORS_CLASS = _GENERATED_HEADER + """

                                                                           namespace Thinktecture.Tests;

                                                                           partial class TestValueObject :
                                                                              global::System.Numerics.ISubtractionOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>
                                                                           {
                                                                              /// <inheritdoc cref="global::System.Numerics.ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)" />
                                                                              public static global::Thinktecture.Tests.TestValueObject operator -(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                              {
                                                                                 global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                 global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                 return Create((left._value - right._value));
                                                                              }

                                                                              /// <inheritdoc cref="global::System.Numerics.ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)" />
                                                                              public static global::Thinktecture.Tests.TestValueObject operator checked -(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                              {
                                                                                 global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                                 global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                                 return Create(checked((left._value - right._value)));
                                                                              }
                                                                           }

                                                                           """;

   private const string _MULTIPLY_OPERATORS_STRUCT = _GENERATED_HEADER + """

                                                                         namespace Thinktecture.Tests;

                                                                         partial struct TestValueObject :
                                                                            global::System.Numerics.IMultiplyOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>
                                                                         {
                                                                            /// <inheritdoc cref="global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
                                                                            public static global::Thinktecture.Tests.TestValueObject operator *(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                            {
                                                                               return Create((left._value * right._value));
                                                                            }

                                                                            /// <inheritdoc cref="global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
                                                                            public static global::Thinktecture.Tests.TestValueObject operator checked *(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                            {
                                                                               return Create(checked((left._value * right._value)));
                                                                            }
                                                                         }

                                                                         """;

   private const string _MULTIPLY_OPERATORS_CLASS = _GENERATED_HEADER + """

                                                                        namespace Thinktecture.Tests;

                                                                        partial class TestValueObject :
                                                                           global::System.Numerics.IMultiplyOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>
                                                                        {
                                                                           /// <inheritdoc cref="global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
                                                                           public static global::Thinktecture.Tests.TestValueObject operator *(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                           {
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                              return Create((left._value * right._value));
                                                                           }

                                                                           /// <inheritdoc cref="global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
                                                                           public static global::Thinktecture.Tests.TestValueObject operator checked *(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                           {
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                              return Create(checked((left._value * right._value)));
                                                                           }
                                                                        }

                                                                        """;

   private const string _DIVISION_OPERATORS_STRUCT = _GENERATED_HEADER + """

                                                                         namespace Thinktecture.Tests;

                                                                         partial struct TestValueObject :
                                                                            global::System.Numerics.IDivisionOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>
                                                                         {
                                                                            /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
                                                                            public static global::Thinktecture.Tests.TestValueObject operator /(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                            {
                                                                               return Create((left._value / right._value));
                                                                            }

                                                                            /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
                                                                            public static global::Thinktecture.Tests.TestValueObject operator checked /(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                            {
                                                                               return Create(checked((left._value / right._value)));
                                                                            }
                                                                         }

                                                                         """;

   private const string _DIVISION_OPERATORS_CLASS = _GENERATED_HEADER + """

                                                                        namespace Thinktecture.Tests;

                                                                        partial class TestValueObject :
                                                                           global::System.Numerics.IDivisionOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>
                                                                        {
                                                                           /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
                                                                           public static global::Thinktecture.Tests.TestValueObject operator /(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                           {
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                              return Create((left._value / right._value));
                                                                           }

                                                                           /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
                                                                           public static global::Thinktecture.Tests.TestValueObject operator checked /(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                           {
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                              global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                              return Create(checked((left._value / right._value)));
                                                                           }
                                                                        }

                                                                        """;

   private const string _FORMATTABLE_STRUCT = _GENERATED_HEADER + """

                                                                  namespace Thinktecture.Tests;

                                                                  partial struct TestValueObject :
                                                                     global::System.IFormattable
                                                                  {
                                                                     /// <inheritdoc />
                                                                     public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)
                                                                     {
                                                                        return this._value.ToString(format, formatProvider);
                                                                     }
                                                                  }

                                                                  """;

   private const string _MAIN_OUTPUT_INT_BASED_CLASS = _GENERATED_HEADER + """

                                                                           namespace Thinktecture.Tests
                                                                           {
                                                                              [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>))]
                                                                              sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                                                 global::Thinktecture.IKeyedValueObject<int>,
                                                                                 global::Thinktecture.IValueObjectConvertable<int>,
                                                                                 global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>
                                                                              {
                                                                                 [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                                                 internal static void ModuleInit()
                                                                                 {
                                                                                    global::System.Func<int, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                                                    global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                                                    global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                                                    var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, int>(static item => item._value);
                                                                                    global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, int>> convertToKeyExpression = static obj => obj._value;

                                                                                    var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                                                    var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(int), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                                                    global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                                                 }

                                                                                 private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                                                 /// <summary>
                                                                                 /// The identifier of this object.
                                                                                 /// </summary>
                                                                                 private readonly int _value;

                                                                                 public static global::Thinktecture.ValidationError? Validate(int @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject? obj)
                                                                                 {
                                                                                    global::Thinktecture.ValidationError? validationError = null;
                                                                                    ValidateFactoryArguments(ref validationError, ref @value);

                                                                                    if (validationError is null)
                                                                                    {
                                                                                       obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                                                       obj.FactoryPostInit();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                       obj = default;
                                                                                    }

                                                                                    return validationError;
                                                                                 }

                                                                                 public static global::Thinktecture.Tests.TestValueObject Create(int @value)
                                                                                 {
                                                                                    var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject? obj);

                                                                                    if (validationError is not null)
                                                                                       throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                                                    return obj!;
                                                                                 }

                                                                                 public static bool TryCreate(int @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                                                 {
                                                                                    return TryCreate(@value, out obj, out _);
                                                                                 }

                                                                                 public static bool TryCreate(
                                                                                    int @value,
                                                                                    [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                                                    [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                                                 {
                                                                                    validationError = Validate(@value, null, out obj);

                                                                                    return validationError is null;
                                                                                 }

                                                                                 static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, ref int @value);

                                                                                 partial void FactoryPostInit();

                                                                                 /// <summary>
                                                                                 /// Gets the identifier of the item.
                                                                                 /// </summary>
                                                                                 [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                                                 int global::Thinktecture.IValueObjectConvertable<int>.ToValue()
                                                                                 {
                                                                                    return this._value;
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Implicit conversion to the type <see cref="int"/>.
                                                                                 /// </summary>
                                                                                 /// <param name="obj">Object to covert.</param>
                                                                                 /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                                                 [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                                                 public static implicit operator int?(global::Thinktecture.Tests.TestValueObject? obj)
                                                                                 {
                                                                                    return obj?._value;
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Explicit conversion to the type <see cref="int"/>.
                                                                                 /// </summary>
                                                                                 /// <param name="obj">Object to covert.</param>
                                                                                 /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                                                 [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                                                 public static explicit operator int(global::Thinktecture.Tests.TestValueObject obj)
                                                                                 {
                                                                                    if(obj is null)
                                                                                       throw new global::System.NullReferenceException();

                                                                                    return obj._value;
                                                                                 }

                                                                                 /// <summary>
                                                                                 /// Explicit conversion from the type <see cref="int"/>.
                                                                                 /// </summary>
                                                                                 /// <param name="value">Value to covert.</param>
                                                                                 /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                                                 public static explicit operator global::Thinktecture.Tests.TestValueObject(int @value)
                                                                                 {
                                                                                    return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                                                 }

                                                                                 private TestValueObject(int @value)
                                                                                 {
                                                                                    ValidateConstructorArguments(ref @value);

                                                                                    this._value = @value;
                                                                                 }

                                                                                 static partial void ValidateConstructorArguments(ref int @value);

                                                                                 /// <inheritdoc />
                                                                                 public override bool Equals(object? other)
                                                                                 {
                                                                                    return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                                                 }

                                                                                 /// <inheritdoc />
                                                                                 public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                                                 {
                                                                                    if (other is null)
                                                                                       return false;

                                                                                    if (global::System.Object.ReferenceEquals(this, other))
                                                                                       return true;

                                                                                    return this._value.Equals(other._value);
                                                                                 }

                                                                                 /// <inheritdoc />
                                                                                 public override int GetHashCode()
                                                                                 {
                                                                                    return global::System.HashCode.Combine(_typeHashCode, this._value);
                                                                                 }

                                                                                 /// <inheritdoc />
                                                                                 public override string ToString()
                                                                                 {
                                                                                    return this._value.ToString();
                                                                                 }
                                                                              }
                                                                           }

                                                                           """;

   private const string _FORMATTABLE_CLASS = _GENERATED_HEADER + """

                                                                 namespace Thinktecture.Tests;

                                                                 partial class TestValueObject :
                                                                    global::System.IFormattable
                                                                 {
                                                                    /// <inheritdoc />
                                                                    public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)
                                                                    {
                                                                       return this._value.ToString(format, formatProvider);
                                                                    }
                                                                 }

                                                                 """;

   private const string _COMPARABLE_STRUCT = _GENERATED_HEADER + """

                                                                 namespace Thinktecture.Tests;

                                                                 partial struct TestValueObject :
                                                                    global::System.IComparable,
                                                                    global::System.IComparable<global::Thinktecture.Tests.TestValueObject>
                                                                 {
                                                                    /// <inheritdoc />
                                                                    public int CompareTo(object? obj)
                                                                    {
                                                                       if(obj is null)
                                                                          return 1;

                                                                       if(obj is not global::Thinktecture.Tests.TestValueObject item)
                                                                          throw new global::System.ArgumentException("Argument must be of type \"TestValueObject\".", nameof(obj));

                                                                       return this.CompareTo(item);
                                                                    }

                                                                    /// <inheritdoc />
                                                                    public int CompareTo(global::Thinktecture.Tests.TestValueObject obj)
                                                                    {
                                                                       return this._value.CompareTo(obj._value);
                                                                    }
                                                                 }

                                                                 """;

   private const string _COMPARABLE_STRUCT_STRING = _GENERATED_HEADER + """

                                                                        namespace Thinktecture.Tests;

                                                                        partial struct TestValueObject :
                                                                           global::System.IComparable,
                                                                           global::System.IComparable<global::Thinktecture.Tests.TestValueObject>
                                                                        {
                                                                           /// <inheritdoc />
                                                                           public int CompareTo(object? obj)
                                                                           {
                                                                              if(obj is null)
                                                                                 return 1;

                                                                              if(obj is not global::Thinktecture.Tests.TestValueObject item)
                                                                                 throw new global::System.ArgumentException("Argument must be of type \"TestValueObject\".", nameof(obj));

                                                                              return this.CompareTo(item);
                                                                           }

                                                                           /// <inheritdoc />
                                                                           public int CompareTo(global::Thinktecture.Tests.TestValueObject obj)
                                                                           {
                                                                              return global::System.StringComparer.OrdinalIgnoreCase.Compare(this._value, obj._value);
                                                                           }
                                                                        }

                                                                        """;

   private const string _COMPARABLE_CLASS = _GENERATED_HEADER + """

                                                                namespace Thinktecture.Tests;

                                                                partial class TestValueObject :
                                                                   global::System.IComparable,
                                                                   global::System.IComparable<global::Thinktecture.Tests.TestValueObject>
                                                                {
                                                                   /// <inheritdoc />
                                                                   public int CompareTo(object? obj)
                                                                   {
                                                                      if(obj is null)
                                                                         return 1;

                                                                      if(obj is not global::Thinktecture.Tests.TestValueObject item)
                                                                         throw new global::System.ArgumentException("Argument must be of type \"TestValueObject\".", nameof(obj));

                                                                      return this.CompareTo(item);
                                                                   }

                                                                   /// <inheritdoc />
                                                                   public int CompareTo(global::Thinktecture.Tests.TestValueObject? obj)
                                                                   {
                                                                      if(obj is null)
                                                                         return 1;

                                                                      return this._value.CompareTo(obj._value);
                                                                   }
                                                                }

                                                                """;

   private const string _COMPARABLE_CLASS_STRING = _GENERATED_HEADER + """

                                                                       namespace Thinktecture.Tests;

                                                                       partial class TestValueObject :
                                                                          global::System.IComparable,
                                                                          global::System.IComparable<global::Thinktecture.Tests.TestValueObject>
                                                                       {
                                                                          /// <inheritdoc />
                                                                          public int CompareTo(object? obj)
                                                                          {
                                                                             if(obj is null)
                                                                                return 1;

                                                                             if(obj is not global::Thinktecture.Tests.TestValueObject item)
                                                                                throw new global::System.ArgumentException("Argument must be of type \"TestValueObject\".", nameof(obj));

                                                                             return this.CompareTo(item);
                                                                          }

                                                                          /// <inheritdoc />
                                                                          public int CompareTo(global::Thinktecture.Tests.TestValueObject? obj)
                                                                          {
                                                                             if(obj is null)
                                                                                return 1;

                                                                             return global::System.StringComparer.OrdinalIgnoreCase.Compare(this._value, obj._value);
                                                                          }
                                                                       }

                                                                       """;

   private const string _COMPARABLE_CLASS_STRING_WITH_ORDINAL_COMPARER = _GENERATED_HEADER + """

                                                                                             namespace Thinktecture.Tests;

                                                                                             partial class TestValueObject :
                                                                                                global::System.IComparable,
                                                                                                global::System.IComparable<global::Thinktecture.Tests.TestValueObject>
                                                                                             {
                                                                                                /// <inheritdoc />
                                                                                                public int CompareTo(object? obj)
                                                                                                {
                                                                                                   if(obj is null)
                                                                                                      return 1;

                                                                                                   if(obj is not global::Thinktecture.Tests.TestValueObject item)
                                                                                                      throw new global::System.ArgumentException("Argument must be of type \"TestValueObject\".", nameof(obj));

                                                                                                   return this.CompareTo(item);
                                                                                                }

                                                                                                /// <inheritdoc />
                                                                                                public int CompareTo(global::Thinktecture.Tests.TestValueObject? obj)
                                                                                                {
                                                                                                   if(obj is null)
                                                                                                      return 1;

                                                                                                   return global::Thinktecture.ComparerAccessors.StringOrdinal.Comparer.Compare(this._value, obj._value);
                                                                                                }
                                                                                             }

                                                                                             """;

   private const string _PARSABLE_STRUCT = _GENERATED_HEADER + """

                                                               namespace Thinktecture.Tests;

                                                               partial struct TestValueObject :
                                                                  global::System.IParsable<global::Thinktecture.Tests.TestValueObject>
                                                               {
                                                                  private static global::Thinktecture.ValidationError? Validate<T>(int key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestValueObject result)
                                                                     where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>
                                                                  {
                                                                     return T.Validate(key, provider, out result);
                                                                  }

                                                                  /// <inheritdoc />
                                                                  public static global::Thinktecture.Tests.TestValueObject Parse(string s, global::System.IFormatProvider? provider)
                                                                  {
                                                                     var key = int.Parse(s, provider);
                                                                     var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(key, provider, out var result);

                                                                     if(validationError is null)
                                                                        return result!;

                                                                     throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestValueObject\".");
                                                                  }

                                                                  /// <inheritdoc />
                                                                  public static bool TryParse(
                                                                     string? s,
                                                                     global::System.IFormatProvider? provider,
                                                                     [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestValueObject result)
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

                                                                     var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(key, provider, out result!);
                                                                     return validationError is null;
                                                                  }
                                                               }

                                                               """;

   private const string _PARSABLE_CLASS = _GENERATED_HEADER + """

                                                              namespace Thinktecture.Tests;

                                                              partial class TestValueObject :
                                                                 global::System.IParsable<global::Thinktecture.Tests.TestValueObject>
                                                              {
                                                                 private static global::Thinktecture.ValidationError? Validate<T>(int key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestValueObject? result)
                                                                    where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>
                                                                 {
                                                                    return T.Validate(key, provider, out result);
                                                                 }

                                                                 /// <inheritdoc />
                                                                 public static global::Thinktecture.Tests.TestValueObject Parse(string s, global::System.IFormatProvider? provider)
                                                                 {
                                                                    var key = int.Parse(s, provider);
                                                                    var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(key, provider, out var result);

                                                                    if(validationError is null)
                                                                       return result!;

                                                                    throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestValueObject\".");
                                                                 }

                                                                 /// <inheritdoc />
                                                                 public static bool TryParse(
                                                                    string? s,
                                                                    global::System.IFormatProvider? provider,
                                                                    [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestValueObject result)
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

                                                                    var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(key, provider, out result!);
                                                                    return validationError is null;
                                                                 }
                                                              }

                                                              """;

   private const string _PARSABLE_STRUCT_STRING = _GENERATED_HEADER + """

                                                                      namespace Thinktecture.Tests;

                                                                      partial struct TestValueObject :
                                                                         global::System.IParsable<global::Thinktecture.Tests.TestValueObject>
                                                                      {
                                                                         private static global::Thinktecture.ValidationError? Validate<T>(string key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestValueObject result)
                                                                            where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>
                                                                         {
                                                                            return T.Validate(key, provider, out result);
                                                                         }

                                                                         /// <inheritdoc />
                                                                         public static global::Thinktecture.Tests.TestValueObject Parse(string s, global::System.IFormatProvider? provider)
                                                                         {
                                                                            var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(s, provider, out var result);

                                                                            if(validationError is null)
                                                                               return result!;

                                                                            throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestValueObject\".");
                                                                         }

                                                                         /// <inheritdoc />
                                                                         public static bool TryParse(
                                                                            string? s,
                                                                            global::System.IFormatProvider? provider,
                                                                            [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestValueObject result)
                                                                         {
                                                                            if(s is null)
                                                                            {
                                                                               result = default;
                                                                               return false;
                                                                            }

                                                                            var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(s, provider, out result!);
                                                                            return validationError is null;
                                                                         }
                                                                      }

                                                                      """;

   private const string _PARSABLE_CLASS_STRING = _GENERATED_HEADER + """

                                                                     namespace Thinktecture.Tests;

                                                                     partial class TestValueObject :
                                                                        global::System.IParsable<global::Thinktecture.Tests.TestValueObject>
                                                                     {
                                                                        private static global::Thinktecture.ValidationError? Validate<T>(string key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestValueObject? result)
                                                                           where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>
                                                                        {
                                                                           return T.Validate(key, provider, out result);
                                                                        }

                                                                        /// <inheritdoc />
                                                                        public static global::Thinktecture.Tests.TestValueObject Parse(string s, global::System.IFormatProvider? provider)
                                                                        {
                                                                           var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(s, provider, out var result);

                                                                           if(validationError is null)
                                                                              return result!;

                                                                           throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestValueObject\".");
                                                                        }

                                                                        /// <inheritdoc />
                                                                        public static bool TryParse(
                                                                           string? s,
                                                                           global::System.IFormatProvider? provider,
                                                                           [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestValueObject result)
                                                                        {
                                                                           if(s is null)
                                                                           {
                                                                              result = default;
                                                                              return false;
                                                                           }

                                                                           var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(s, provider, out result!);
                                                                           return validationError is null;
                                                                        }
                                                                     }

                                                                     """;

   private const string _EQUALITY_COMPARISON_OPERATORS_CLASS = _GENERATED_HEADER + """

                                                                                   namespace Thinktecture.Tests;

                                                                                   partial class TestValueObject :
                                                                                      global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>
                                                                                   {
                                                                                         /// <summary>
                                                                                         /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                         /// </summary>
                                                                                         /// <param name="obj">Instance to compare.</param>
                                                                                         /// <param name="other">Another instance to compare.</param>
                                                                                         /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                         public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                                                         {
                                                                                            if (obj is null)
                                                                                               return other is null;

                                                                                            return obj.Equals(other);
                                                                                         }

                                                                                         /// <summary>
                                                                                         /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                         /// </summary>
                                                                                         /// <param name="obj">Instance to compare.</param>
                                                                                         /// <param name="other">Another instance to compare.</param>
                                                                                         /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                         public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                                                         {
                                                                                            return !(obj == other);
                                                                                         }
                                                                                   }

                                                                                   """;

   private const string _EQUALITY_COMPARISON_OPERATORS_STRUCT = _GENERATED_HEADER + """

                                                                                    namespace Thinktecture.Tests;

                                                                                    partial struct TestValueObject :
                                                                                       global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>
                                                                                    {
                                                                                          /// <summary>
                                                                                          /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                          /// </summary>
                                                                                          /// <param name="obj">Instance to compare.</param>
                                                                                          /// <param name="other">Another instance to compare.</param>
                                                                                          /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                          public static bool operator ==(global::Thinktecture.Tests.TestValueObject obj, global::Thinktecture.Tests.TestValueObject other)
                                                                                          {
                                                                                             return obj.Equals(other);
                                                                                          }

                                                                                          /// <summary>
                                                                                          /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                          /// </summary>
                                                                                          /// <param name="obj">Instance to compare.</param>
                                                                                          /// <param name="other">Another instance to compare.</param>
                                                                                          /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                          public static bool operator !=(global::Thinktecture.Tests.TestValueObject obj, global::Thinktecture.Tests.TestValueObject other)
                                                                                          {
                                                                                             return !(obj == other);
                                                                                          }
                                                                                    }

                                                                                    """;

   private const string _EQUALITY_COMPARISON_OPERATORS_INT_WITH_KEY_OVERLOADS = _GENERATED_HEADER + """

                                                                                                    namespace Thinktecture.Tests;

                                                                                                    partial class TestValueObject :
                                                                                                       global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                                                                       global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, int, bool>
                                                                                                    {
                                                                                                          /// <summary>
                                                                                                          /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                                          /// </summary>
                                                                                                          /// <param name="obj">Instance to compare.</param>
                                                                                                          /// <param name="other">Another instance to compare.</param>
                                                                                                          /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                                          public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                                                                          {
                                                                                                             if (obj is null)
                                                                                                                return other is null;

                                                                                                             return obj.Equals(other);
                                                                                                          }

                                                                                                          /// <summary>
                                                                                                          /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                                          /// </summary>
                                                                                                          /// <param name="obj">Instance to compare.</param>
                                                                                                          /// <param name="other">Another instance to compare.</param>
                                                                                                          /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                                          public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                                                                          {
                                                                                                             return !(obj == other);
                                                                                                          }

                                                                                                          private static bool Equals(global::Thinktecture.Tests.TestValueObject? obj, int value)
                                                                                                          {
                                                                                                             if (obj is null)
                                                                                                                return false;

                                                                                                             return obj._value.Equals(value);
                                                                                                          }

                                                                                                          /// <summary>
                                                                                                          /// Compares an instance of <see cref="TestValueObject"/> with <see cref="int"/>.
                                                                                                          /// </summary>
                                                                                                          /// <param name="obj">Instance to compare.</param>
                                                                                                          /// <param name="value">Value to compare with.</param>
                                                                                                          /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                                          public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, int value)
                                                                                                          {
                                                                                                             return Equals(obj, value);
                                                                                                          }

                                                                                                          /// <summary>
                                                                                                          /// Compares an instance of <see cref="TestValueObject"/> with <see cref="int"/>.
                                                                                                          /// </summary>
                                                                                                          /// <param name="value">Value to compare.</param>
                                                                                                          /// <param name="obj">Instance to compare with.</param>
                                                                                                          /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                                          public static bool operator ==(int value, global::Thinktecture.Tests.TestValueObject? obj)
                                                                                                          {
                                                                                                             return Equals(obj, value);
                                                                                                          }

                                                                                                          /// <summary>
                                                                                                          /// Compares an instance of <see cref="TestValueObject"/> with <see cref="int"/>.
                                                                                                          /// </summary>
                                                                                                          /// <param name="obj">Instance to compare.</param>
                                                                                                          /// <param name="value">Value to compare with.</param>
                                                                                                          /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                                          public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, int value)
                                                                                                          {
                                                                                                             return !(obj == value);
                                                                                                          }

                                                                                                          /// <summary>
                                                                                                          /// Compares an instance of <see cref="int"/> with <see cref="TestValueObject"/>.
                                                                                                          /// </summary>
                                                                                                          /// <param name="value">Value to compare.</param>
                                                                                                          /// <param name="obj">Instance to compare with.</param>
                                                                                                          /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                                          public static bool operator !=(int value, global::Thinktecture.Tests.TestValueObject? obj)
                                                                                                          {
                                                                                                             return !(obj == value);
                                                                                                          }
                                                                                                    }

                                                                                                    """;

   private const string _COMPLEX_CLASS_WITHOUT_MEMBERS = _GENERATED_HEADER + """

                                                                             namespace Thinktecture.Tests
                                                                             {
                                                                                sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                                                   global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                                                   global::Thinktecture.IComplexValueObject
                                                                                {
                                                                                   [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                                                   internal static void ModuleInit()
                                                                                   {
                                                                                      global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                                                                                                         {
                                                                                                                                                                                         };

                                                                                      var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

                                                                                      foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
                                                                                      {
                                                                                         members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
                                                                                      }

                                                                                      var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                                                      var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

                                                                                      global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                                                   }

                                                                                   private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                                                   public static global::Thinktecture.ValidationError? Validate(
                                                                                      out global::Thinktecture.Tests.TestValueObject? obj)
                                                                                   {
                                                                                      global::Thinktecture.ValidationError? validationError = null;
                                                                                      ValidateFactoryArguments(
                                                                                         ref validationError);

                                                                                      if (validationError is null)
                                                                                      {
                                                                                         obj = new global::Thinktecture.Tests.TestValueObject();
                                                                                         obj.FactoryPostInit();
                                                                                      }
                                                                                      else
                                                                                      {
                                                                                         obj = default;
                                                                                      }

                                                                                      return validationError;
                                                                                   }

                                                                                   public static global::Thinktecture.Tests.TestValueObject Create()
                                                                                   {
                                                                                      var validationError = Validate(
                                                                                         out global::Thinktecture.Tests.TestValueObject? obj);

                                                                                      if (validationError is not null)
                                                                                         throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                                                      return obj!;
                                                                                   }

                                                                                   public static bool TryCreate(
                                                                                      [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                                                   {
                                                                                      return TryCreate(
                                                                                         out obj,
                                                                                         out _);
                                                                                   }

                                                                                   public static bool TryCreate(
                                                                                      [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                                                      [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                                                   {
                                                                                      validationError = Validate(
                                                                                         out obj);

                                                                                      return validationError is null;
                                                                                   }

                                                                                   static partial void ValidateFactoryArguments(
                                                                                      ref global::Thinktecture.ValidationError? validationError);

                                                                                   partial void FactoryPostInit();

                                                                                   private TestValueObject()
                                                                                   {
                                                                                   }

                                                                                   /// <summary>
                                                                                   /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                   /// </summary>
                                                                                   /// <param name="obj">Instance to compare.</param>
                                                                                   /// <param name="other">Another instance to compare.</param>
                                                                                   /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                   public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                                                   {
                                                                                      if (obj is null)
                                                                                         return other is null;

                                                                                      return obj.Equals(other);
                                                                                   }

                                                                                   /// <summary>
                                                                                   /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                   /// </summary>
                                                                                   /// <param name="obj">Instance to compare.</param>
                                                                                   /// <param name="other">Another instance to compare.</param>
                                                                                   /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                   public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                                                   {
                                                                                      return !(obj == other);
                                                                                   }

                                                                                   /// <inheritdoc />
                                                                                   public override bool Equals(object? other)
                                                                                   {
                                                                                      return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                                                   }

                                                                                   /// <inheritdoc />
                                                                                   public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                                                   {
                                                                                      if (other is null)
                                                                                         return false;

                                                                                      if (global::System.Object.ReferenceEquals(this, other))
                                                                                         return true;

                                                                                      return true;
                                                                                   }

                                                                                   /// <inheritdoc />
                                                                                   public override int GetHashCode()
                                                                                   {
                                                                                      return _typeHashCode;
                                                                                   }

                                                                                   /// <inheritdoc />
                                                                                   public override string ToString()
                                                                                   {
                                                                                      return "TestValueObject";
                                                                                   }
                                                                                }
                                                                             }

                                                                             """;

   private const string _COMPLEX_STRUCT_WITHOUT_MEMBERS = _GENERATED_HEADER + """

                                                                              namespace Thinktecture.Tests
                                                                              {
                                                                                 readonly partial struct TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject>,
                                                                                    global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                                                    global::Thinktecture.IComplexValueObject
                                                                                 {
                                                                                    [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                                                    internal static void ModuleInit()
                                                                                    {
                                                                                       global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                                                                                                          {
                                                                                                                                                                                          };

                                                                                       var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

                                                                                       foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
                                                                                       {
                                                                                          members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
                                                                                       }

                                                                                       var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                                                       var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

                                                                                       global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                                                    }

                                                                                    private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                                                    public static global::Thinktecture.ValidationError? Validate(
                                                                                       out global::Thinktecture.Tests.TestValueObject obj)
                                                                                    {
                                                                                       global::Thinktecture.ValidationError? validationError = null;
                                                                                       ValidateFactoryArguments(
                                                                                          ref validationError);

                                                                                       if (validationError is null)
                                                                                       {
                                                                                          obj = new global::Thinktecture.Tests.TestValueObject();
                                                                                          obj.FactoryPostInit();
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                          obj = default;
                                                                                       }

                                                                                       return validationError;
                                                                                    }

                                                                                    public static global::Thinktecture.Tests.TestValueObject Create()
                                                                                    {
                                                                                       var validationError = Validate(
                                                                                          out global::Thinktecture.Tests.TestValueObject obj);

                                                                                       if (validationError is not null)
                                                                                          throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                                                       return obj!;
                                                                                    }

                                                                                    public static bool TryCreate(
                                                                                       [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj)
                                                                                    {
                                                                                       return TryCreate(
                                                                                          out obj,
                                                                                          out _);
                                                                                    }

                                                                                    public static bool TryCreate(
                                                                                       [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj,
                                                                                       [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                                                    {
                                                                                       validationError = Validate(
                                                                                          out obj);

                                                                                       return validationError is null;
                                                                                    }

                                                                                    static partial void ValidateFactoryArguments(
                                                                                       ref global::Thinktecture.ValidationError? validationError);

                                                                                    partial void FactoryPostInit();

                                                                                    /// <summary>
                                                                                    /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                    /// </summary>
                                                                                    /// <param name="obj">Instance to compare.</param>
                                                                                    /// <param name="other">Another instance to compare.</param>
                                                                                    /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                    public static bool operator ==(global::Thinktecture.Tests.TestValueObject obj, global::Thinktecture.Tests.TestValueObject other)
                                                                                    {
                                                                                       return obj.Equals(other);
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                    /// </summary>
                                                                                    /// <param name="obj">Instance to compare.</param>
                                                                                    /// <param name="other">Another instance to compare.</param>
                                                                                    /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                    public static bool operator !=(global::Thinktecture.Tests.TestValueObject obj, global::Thinktecture.Tests.TestValueObject other)
                                                                                    {
                                                                                       return !(obj == other);
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public override bool Equals(object? other)
                                                                                    {
                                                                                       return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public bool Equals(global::Thinktecture.Tests.TestValueObject other)
                                                                                    {
                                                                                       return true;
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public override int GetHashCode()
                                                                                    {
                                                                                       return _typeHashCode;
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public override string ToString()
                                                                                    {
                                                                                       return "TestValueObject";
                                                                                    }
                                                                                 }
                                                                              }

                                                                              """;

   private const string _MAIN_OUTPUT_STRING_BASED_CLASS = _GENERATED_HEADER + """

                                                                              namespace Thinktecture.Tests
                                                                              {
                                                                                 [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>))]
                                                                                 sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                                                    global::Thinktecture.IKeyedValueObject<string>,
                                                                                    global::Thinktecture.IValueObjectConvertable<string>,
                                                                                    global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>
                                                                                 {
                                                                                    [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                                                    internal static void ModuleInit()
                                                                                    {
                                                                                       global::System.Func<string, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                                                       global::System.Linq.Expressions.Expression<global::System.Func<string, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                                                       global::System.Linq.Expressions.Expression<global::System.Func<string, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                                                       var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, string>(static item => item._value);
                                                                                       global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, string>> convertToKeyExpression = static obj => obj._value;

                                                                                       var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                                                       var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(string), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                                                       global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                                                    }

                                                                                    private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                                                    /// <summary>
                                                                                    /// The identifier of this object.
                                                                                    /// </summary>
                                                                                    private readonly string _value;

                                                                                    public static global::Thinktecture.ValidationError? Validate(string? @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject? obj)
                                                                                    {
                                                                                       if(@value is null)
                                                                                       {
                                                                                          obj = default;
                                                                                          return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>("The argument 'value' must not be null.");
                                                                                       }

                                                                                       global::Thinktecture.ValidationError? validationError = null;
                                                                                       ValidateFactoryArguments(ref validationError, ref @value);

                                                                                       if (validationError is null)
                                                                                       {
                                                                                          obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                                                          obj.FactoryPostInit();
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                          obj = default;
                                                                                       }

                                                                                       return validationError;
                                                                                    }

                                                                                    public static global::Thinktecture.Tests.TestValueObject Create(string @value)
                                                                                    {
                                                                                       var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject? obj);

                                                                                       if (validationError is not null)
                                                                                          throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                                                       return obj!;
                                                                                    }

                                                                                    public static bool TryCreate(string @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                                                    {
                                                                                       return TryCreate(@value, out obj, out _);
                                                                                    }

                                                                                    public static bool TryCreate(
                                                                                       string @value,
                                                                                       [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                                                       [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                                                    {
                                                                                       validationError = Validate(@value, null, out obj);

                                                                                       return validationError is null;
                                                                                    }

                                                                                    static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value);

                                                                                    partial void FactoryPostInit();

                                                                                    /// <summary>
                                                                                    /// Gets the identifier of the item.
                                                                                    /// </summary>
                                                                                    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                                                    string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                                                    {
                                                                                       return this._value;
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Implicit conversion to the type <see cref="string"/>.
                                                                                    /// </summary>
                                                                                    /// <param name="obj">Object to covert.</param>
                                                                                    /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                                                    [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                                                    public static implicit operator string?(global::Thinktecture.Tests.TestValueObject? obj)
                                                                                    {
                                                                                       return obj?._value;
                                                                                    }

                                                                                    /// <summary>
                                                                                    /// Explicit conversion from the type <see cref="string"/>.
                                                                                    /// </summary>
                                                                                    /// <param name="value">Value to covert.</param>
                                                                                    /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                                                    [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("value")]
                                                                                    public static explicit operator global::Thinktecture.Tests.TestValueObject?(string? @value)
                                                                                    {
                                                                                       if(@value is null)
                                                                                          return null;

                                                                                       return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                                                    }

                                                                                    private TestValueObject(string @value)
                                                                                    {
                                                                                       ValidateConstructorArguments(ref @value);

                                                                                       this._value = @value;
                                                                                    }

                                                                                    static partial void ValidateConstructorArguments(ref string @value);

                                                                                    /// <inheritdoc />
                                                                                    public override bool Equals(object? other)
                                                                                    {
                                                                                       return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                                                    {
                                                                                       if (other is null)
                                                                                          return false;

                                                                                       if (global::System.Object.ReferenceEquals(this, other))
                                                                                          return true;

                                                                                       return global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value, other._value);
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public override int GetHashCode()
                                                                                    {
                                                                                       return global::System.HashCode.Combine(_typeHashCode, global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(this._value));
                                                                                    }

                                                                                    /// <inheritdoc />
                                                                                    public override string ToString()
                                                                                    {
                                                                                       return this._value.ToString();
                                                                                    }
                                                                                 }
                                                                              }

                                                                              """;

   private const string _MAIN_OUTPUT_STRING_BASED_STRUCT = _GENERATED_HEADER + """

                                                                               namespace Thinktecture.Tests
                                                                               {
                                                                                  [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]
                                                                                  [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>))]
                                                                                  readonly partial struct TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject>,
                                                                                     global::Thinktecture.IKeyedValueObject<string>,
                                                                                     global::Thinktecture.IValueObjectConvertable<string>,
                                                                                     global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>
                                                                                  {
                                                                                     [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                                                     internal static void ModuleInit()
                                                                                     {
                                                                                        global::System.Func<string, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                                                        global::System.Linq.Expressions.Expression<global::System.Func<string, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                                                        global::System.Linq.Expressions.Expression<global::System.Func<string, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                                                        var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, string>(static item => item._value);
                                                                                        global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, string>> convertToKeyExpression = static obj => obj._value;

                                                                                        var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                                                        var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(string), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                                                        global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                                                     }

                                                                                     private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                                                     /// <summary>
                                                                                     /// The identifier of this object.
                                                                                     /// </summary>
                                                                                     private readonly string _value;

                                                                                     public static global::Thinktecture.ValidationError? Validate(string? @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject obj)
                                                                                     {
                                                                                        if(@value is null)
                                                                                        {
                                                                                           obj = default;
                                                                                           return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>("The argument 'value' must not be null.");
                                                                                        }

                                                                                        global::Thinktecture.ValidationError? validationError = null;
                                                                                        ValidateFactoryArguments(ref validationError, ref @value);

                                                                                        if (validationError is null)
                                                                                        {
                                                                                           obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                                                           obj.FactoryPostInit();
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                           obj = default;
                                                                                        }

                                                                                        return validationError;
                                                                                     }

                                                                                     public static global::Thinktecture.Tests.TestValueObject Create(string @value)
                                                                                     {
                                                                                        var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject obj);

                                                                                        if (validationError is not null)
                                                                                           throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                                                        return obj!;
                                                                                     }

                                                                                     public static bool TryCreate(string @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj)
                                                                                     {
                                                                                        return TryCreate(@value, out obj, out _);
                                                                                     }

                                                                                     public static bool TryCreate(
                                                                                        string @value,
                                                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj,
                                                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                                                     {
                                                                                        validationError = Validate(@value, null, out obj);

                                                                                        return validationError is null;
                                                                                     }

                                                                                     static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value);

                                                                                     partial void FactoryPostInit();

                                                                                     /// <summary>
                                                                                     /// Gets the identifier of the item.
                                                                                     /// </summary>
                                                                                     [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                                                     string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                                                     {
                                                                                        return this._value;
                                                                                     }

                                                                                     /// <summary>
                                                                                     /// Implicit conversion to the type <see cref="string"/>.
                                                                                     /// </summary>
                                                                                     /// <param name="obj">Object to covert.</param>
                                                                                     /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                                                     [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                                                     public static implicit operator string?(global::Thinktecture.Tests.TestValueObject? obj)
                                                                                     {
                                                                                        return obj?._value;
                                                                                     }

                                                                                     /// <summary>
                                                                                     /// Explicit conversion from the type <see cref="string"/>.
                                                                                     /// </summary>
                                                                                     /// <param name="value">Value to covert.</param>
                                                                                     /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                                                     public static explicit operator global::Thinktecture.Tests.TestValueObject(string @value)
                                                                                     {
                                                                                        return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                                                     }

                                                                                     private TestValueObject(string @value)
                                                                                     {
                                                                                        ValidateConstructorArguments(ref @value);

                                                                                        this._value = @value;
                                                                                     }

                                                                                     static partial void ValidateConstructorArguments(ref string @value);

                                                                                     /// <inheritdoc />
                                                                                     public override bool Equals(object? other)
                                                                                     {
                                                                                        return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                                                     }

                                                                                     /// <inheritdoc />
                                                                                     public bool Equals(global::Thinktecture.Tests.TestValueObject other)
                                                                                     {
                                                                                        return global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value, other._value);
                                                                                     }

                                                                                     /// <inheritdoc />
                                                                                     public override int GetHashCode()
                                                                                     {
                                                                                        return global::System.HashCode.Combine(_typeHashCode, global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(this._value));
                                                                                     }

                                                                                     /// <inheritdoc />
                                                                                     public override string ToString()
                                                                                     {
                                                                                        return this._value.ToString();
                                                                                     }
                                                                                  }
                                                                               }

                                                                               """;

   [Fact]
   public void Should_not_generate_code_for_keyed_class_if_not_partial()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<string>]
                   	public class TestValueObject
                   	{
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      AssertOutput(output, null);
   }

   [Fact]
   public void Should_not_generate_code_for_complex_class_if_not_partial()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject]
                   	public class TestValueObject
                   	{
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      AssertOutput(output, null);
   }

   [Fact]
   public void Should_generate_complex_class_with_nullable_members()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject]
                   	public partial class TestValueObject
                   	{
                         public string? Prop1 { get; }
                         public Func<string?, Task<string?>?>? Prop2 { get; }

                         static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string? prop1, ref Func<string?, Task<string?>?>? prop2)
                         {
                         }
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      AssertOutput(output, _GENERATED_HEADER + """

                                               namespace Thinktecture.Tests
                                               {
                                                  sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                     global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                     global::Thinktecture.IComplexValueObject
                                                  {
                                                     [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                     internal static void ModuleInit()
                                                     {
                                                        global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                                                                           {
                                                                                                                                                              o.Prop1,
                                                                                                                                                              o.Prop2
                                                                                                                                                           };

                                                        var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

                                                        foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
                                                        {
                                                           members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
                                                        }

                                                        var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                        var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

                                                        global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                     }

                                                     private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                     public static global::Thinktecture.ValidationError? Validate(
                                                        string? @prop1,
                                                        global::System.Func<string?, Task<string?>?>? @prop2,
                                                        out global::Thinktecture.Tests.TestValueObject? obj)
                                                     {
                                                        global::Thinktecture.ValidationError? validationError = null;
                                                        ValidateFactoryArguments(
                                                           ref validationError,
                                                           ref @prop1,
                                                           ref @prop2);

                                                        if (validationError is null)
                                                        {
                                                           obj = new global::Thinktecture.Tests.TestValueObject(
                                                              @prop1,
                                                              @prop2);
                                                           obj.FactoryPostInit();
                                                        }
                                                        else
                                                        {
                                                           obj = default;
                                                        }

                                                        return validationError;
                                                     }

                                                     public static global::Thinktecture.Tests.TestValueObject Create(
                                                        string? @prop1,
                                                        global::System.Func<string?, Task<string?>?>? @prop2)
                                                     {
                                                        var validationError = Validate(
                                                           @prop1,
                                                           @prop2,
                                                           out global::Thinktecture.Tests.TestValueObject? obj);

                                                        if (validationError is not null)
                                                           throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                        return obj!;
                                                     }

                                                     public static bool TryCreate(
                                                        string? @prop1,
                                                        global::System.Func<string?, Task<string?>?>? @prop2,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                     {
                                                        return TryCreate(
                                                           @prop1,
                                                           @prop2,
                                                           out obj,
                                                           out _);
                                                     }

                                                     public static bool TryCreate(
                                                        string? @prop1,
                                                        global::System.Func<string?, Task<string?>?>? @prop2,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                     {
                                                        validationError = Validate(
                                                           @prop1,
                                                           @prop2,
                                                           out obj);

                                                        return validationError is null;
                                                     }

                                                     static partial void ValidateFactoryArguments(
                                                        ref global::Thinktecture.ValidationError? validationError,
                                                        ref string? @prop1,
                                                        ref global::System.Func<string?, Task<string?>?>? @prop2);

                                                     partial void FactoryPostInit();

                                                     private TestValueObject(
                                                        string? @prop1,
                                                        global::System.Func<string?, Task<string?>?>? @prop2)
                                                     {
                                                        ValidateConstructorArguments(
                                                           ref @prop1,
                                                           ref @prop2);

                                                        this.Prop1 = @prop1;
                                                        this.Prop2 = @prop2;
                                                     }

                                                     static partial void ValidateConstructorArguments(
                                                        ref string? @prop1,
                                                        ref global::System.Func<string?, Task<string?>?>? @prop2);

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                     public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        if (obj is null)
                                                           return other is null;

                                                        return obj.Equals(other);
                                                     }

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                     public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        return !(obj == other);
                                                     }

                                                     /// <inheritdoc />
                                                     public override bool Equals(object? other)
                                                     {
                                                        return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                     }

                                                     /// <inheritdoc />
                                                     public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        if (other is null)
                                                           return false;

                                                        if (global::System.Object.ReferenceEquals(this, other))
                                                           return true;

                                                        return global::System.StringComparer.OrdinalIgnoreCase.Equals(this.Prop1, other.Prop1)
                                                            && (this.Prop2 is null ? other.Prop2 is null : this.Prop2.Equals(other.Prop2));
                                                     }

                                                     /// <inheritdoc />
                                                     public override int GetHashCode()
                                                     {
                                                        return global::System.HashCode.Combine(
                                                           _typeHashCode,
                                                           this.Prop1,
                                                           this.Prop2);
                                                     }

                                                     /// <inheritdoc />
                                                     public override string ToString()
                                                     {
                                                        return $"{{ Prop1 = {this.Prop1}, Prop2 = {this.Prop2} }}";
                                                     }
                                                  }
                                               }

                                               """);
   }

   [Fact]
   public void Should_generate_complex_class_with_post_init_method_if_validation_method_returns_struct()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject]
                   	public partial class TestValueObject
                   	{
                         static partial int ValidateFactoryArguments(ref ValidationError? validationError)
                         {
                            return 42;
                         }
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      AssertOutput(output, _GENERATED_HEADER + """

                                               namespace Thinktecture.Tests
                                               {
                                                  sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                     global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                     global::Thinktecture.IComplexValueObject
                                                  {
                                                     [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                     internal static void ModuleInit()
                                                     {
                                                        global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                                                                           {
                                                                                                                                                           };

                                                        var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

                                                        foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
                                                        {
                                                           members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
                                                        }

                                                        var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                        var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

                                                        global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                     }

                                                     private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                     public static global::Thinktecture.ValidationError? Validate(
                                                        out global::Thinktecture.Tests.TestValueObject? obj)
                                                     {
                                                        global::Thinktecture.ValidationError? validationError = null;
                                                        var factoryArgumentsValidationError = ValidateFactoryArguments(
                                                           ref validationError);

                                                        if (validationError is null)
                                                        {
                                                           obj = new global::Thinktecture.Tests.TestValueObject();
                                                           obj.FactoryPostInit(factoryArgumentsValidationError);
                                                        }
                                                        else
                                                        {
                                                           obj = default;
                                                        }

                                                        return validationError;
                                                     }

                                                     public static global::Thinktecture.Tests.TestValueObject Create()
                                                     {
                                                        var validationError = Validate(
                                                           out global::Thinktecture.Tests.TestValueObject? obj);

                                                        if (validationError is not null)
                                                           throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                        return obj!;
                                                     }

                                                     public static bool TryCreate(
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                     {
                                                        return TryCreate(
                                                           out obj,
                                                           out _);
                                                     }

                                                     public static bool TryCreate(
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                     {
                                                        validationError = Validate(
                                                           out obj);

                                                        return validationError is null;
                                                     }

                                                     private static partial int ValidateFactoryArguments(
                                                        ref global::Thinktecture.ValidationError? validationError);

                                                     partial void FactoryPostInit(int factoryArgumentsValidationError);

                                                     private TestValueObject()
                                                     {
                                                     }

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                     public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        if (obj is null)
                                                           return other is null;

                                                        return obj.Equals(other);
                                                     }

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                     public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        return !(obj == other);
                                                     }

                                                     /// <inheritdoc />
                                                     public override bool Equals(object? other)
                                                     {
                                                        return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                     }

                                                     /// <inheritdoc />
                                                     public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        if (other is null)
                                                           return false;

                                                        if (global::System.Object.ReferenceEquals(this, other))
                                                           return true;

                                                        return true;
                                                     }

                                                     /// <inheritdoc />
                                                     public override int GetHashCode()
                                                     {
                                                        return _typeHashCode;
                                                     }

                                                     /// <inheritdoc />
                                                     public override string ToString()
                                                     {
                                                        return "TestValueObject";
                                                     }
                                                  }
                                               }

                                               """);
   }

   [Fact]
   public void Should_generate_complex_class_with_post_init_method_if_validation_method_returns_nullable_string()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   #nullable enable

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject]
                   	public partial class TestValueObject
                   	{
                         static partial string? ValidateFactoryArguments(ref ValidationError? validationError)
                         {
                            return String.Empty;
                         }
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      AssertOutput(output, _GENERATED_HEADER + """

                                               namespace Thinktecture.Tests
                                               {
                                                  sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                     global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                     global::Thinktecture.IComplexValueObject
                                                  {
                                                     [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                     internal static void ModuleInit()
                                                     {
                                                        global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                                                                           {
                                                                                                                                                           };

                                                        var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

                                                        foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
                                                        {
                                                           members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
                                                        }

                                                        var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                        var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

                                                        global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                     }

                                                     private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                     public static global::Thinktecture.ValidationError? Validate(
                                                        out global::Thinktecture.Tests.TestValueObject? obj)
                                                     {
                                                        global::Thinktecture.ValidationError? validationError = null;
                                                        var factoryArgumentsValidationError = ValidateFactoryArguments(
                                                           ref validationError);

                                                        if (validationError is null)
                                                        {
                                                           obj = new global::Thinktecture.Tests.TestValueObject();
                                                           obj.FactoryPostInit(factoryArgumentsValidationError);
                                                        }
                                                        else
                                                        {
                                                           obj = default;
                                                        }

                                                        return validationError;
                                                     }

                                                     public static global::Thinktecture.Tests.TestValueObject Create()
                                                     {
                                                        var validationError = Validate(
                                                           out global::Thinktecture.Tests.TestValueObject? obj);

                                                        if (validationError is not null)
                                                           throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                        return obj!;
                                                     }

                                                     public static bool TryCreate(
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                     {
                                                        return TryCreate(
                                                           out obj,
                                                           out _);
                                                     }

                                                     public static bool TryCreate(
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                     {
                                                        validationError = Validate(
                                                           out obj);

                                                        return validationError is null;
                                                     }

                                                     private static partial string? ValidateFactoryArguments(
                                                        ref global::Thinktecture.ValidationError? validationError);

                                                     partial void FactoryPostInit(string? factoryArgumentsValidationError);

                                                     private TestValueObject()
                                                     {
                                                     }

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                     public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        if (obj is null)
                                                           return other is null;

                                                        return obj.Equals(other);
                                                     }

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                     public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        return !(obj == other);
                                                     }

                                                     /// <inheritdoc />
                                                     public override bool Equals(object? other)
                                                     {
                                                        return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                     }

                                                     /// <inheritdoc />
                                                     public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        if (other is null)
                                                           return false;

                                                        if (global::System.Object.ReferenceEquals(this, other))
                                                           return true;

                                                        return true;
                                                     }

                                                     /// <inheritdoc />
                                                     public override int GetHashCode()
                                                     {
                                                        return _typeHashCode;
                                                     }

                                                     /// <inheritdoc />
                                                     public override string ToString()
                                                     {
                                                        return "TestValueObject";
                                                     }
                                                  }
                                               }

                                               """);
   }

   [Fact]
   public void Should_not_generate_code_for_keyed_class_with_generic()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<int>]
                   	public partial class TestValueObject<T>
                   	{
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      AssertOutput(output, null);
   }

   [Fact]
   public void Should_not_generate_code_for_complex_class_with_generic()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject]
                   	public partial class TestValueObject<T>
                   	{
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      AssertOutput(output, null);
   }

   [Fact]
   public void Should_generate_complex_class_without_namespace()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   [ComplexValueObject]
                   public partial class TestValueObject
                   {
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      AssertOutput(output, _GENERATED_HEADER + """

                                                  sealed partial class TestValueObject : global::System.IEquatable<global::TestValueObject?>,
                                                     global::System.Numerics.IEqualityOperators<global::TestValueObject, global::TestValueObject, bool>,
                                                     global::Thinktecture.IComplexValueObject
                                                  {
                                                     [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                     internal static void ModuleInit()
                                                     {
                                                        global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                                                                           {
                                                                                                                                                           };

                                                        var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

                                                        foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
                                                        {
                                                           members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
                                                        }

                                                        var type = typeof(global::TestValueObject);
                                                        var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

                                                        global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                     }

                                                     private static readonly int _typeHashCode = typeof(global::TestValueObject).GetHashCode();

                                                     public static global::Thinktecture.ValidationError? Validate(
                                                        out global::TestValueObject? obj)
                                                     {
                                                        global::Thinktecture.ValidationError? validationError = null;
                                                        ValidateFactoryArguments(
                                                           ref validationError);

                                                        if (validationError is null)
                                                        {
                                                           obj = new global::TestValueObject();
                                                           obj.FactoryPostInit();
                                                        }
                                                        else
                                                        {
                                                           obj = default;
                                                        }

                                                        return validationError;
                                                     }

                                                     public static global::TestValueObject Create()
                                                     {
                                                        var validationError = Validate(
                                                           out global::TestValueObject? obj);

                                                        if (validationError is not null)
                                                           throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                        return obj!;
                                                     }

                                                     public static bool TryCreate(
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::TestValueObject? obj)
                                                     {
                                                        return TryCreate(
                                                           out obj,
                                                           out _);
                                                     }

                                                     public static bool TryCreate(
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::TestValueObject? obj,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                     {
                                                        validationError = Validate(
                                                           out obj);

                                                        return validationError is null;
                                                     }

                                                     static partial void ValidateFactoryArguments(
                                                        ref global::Thinktecture.ValidationError? validationError);

                                                     partial void FactoryPostInit();

                                                     private TestValueObject()
                                                     {
                                                     }

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                     public static bool operator ==(global::TestValueObject? obj, global::TestValueObject? other)
                                                     {
                                                        if (obj is null)
                                                           return other is null;

                                                        return obj.Equals(other);
                                                     }

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                     public static bool operator !=(global::TestValueObject? obj, global::TestValueObject? other)
                                                     {
                                                        return !(obj == other);
                                                     }

                                                     /// <inheritdoc />
                                                     public override bool Equals(object? other)
                                                     {
                                                        return other is global::TestValueObject obj && Equals(obj);
                                                     }

                                                     /// <inheritdoc />
                                                     public bool Equals(global::TestValueObject? other)
                                                     {
                                                        if (other is null)
                                                           return false;

                                                        if (global::System.Object.ReferenceEquals(this, other))
                                                           return true;

                                                        return true;
                                                     }

                                                     /// <inheritdoc />
                                                     public override int GetHashCode()
                                                     {
                                                        return _typeHashCode;
                                                     }

                                                     /// <inheritdoc />
                                                     public override string ToString()
                                                     {
                                                        return "TestValueObject";
                                                     }
                                                  }

                                               """);
   }

   [Fact]
   public void Should_generate_complex_class_without_members()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject]
                   	public partial class TestValueObject
                   	{
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      AssertOutput(output, _COMPLEX_CLASS_WITHOUT_MEMBERS);
   }

   [Fact]
   public void Should_not_generate_factory_methods_if_SkipFactoryMethods_is_true()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<int>(SkipFactoryMethods = true)]
                   	public partial class TestValueObject
                   	{
                     }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var formattableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(formattableOutput, _FORMATTABLE_CLASS);
      AssertOutput(comparableOutput, _COMPARABLE_CLASS);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_CLASS);
      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                         global::Thinktecture.IKeyedValueObject<int>,
                                                         global::Thinktecture.IValueObjectConvertable<int>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            global::System.Func<int, global::Thinktecture.Tests.TestValueObject>? convertFromKey = null;
                                                            global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>>? convertFromKeyExpression = null;
                                                            global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, int>(static item => item._value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, int>> convertToKeyExpression = static obj => obj._value;

                                                            var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(int), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                         }

                                                         private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                         /// <summary>
                                                         /// The identifier of this object.
                                                         /// </summary>
                                                         private readonly int _value;

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         int global::Thinktecture.IValueObjectConvertable<int>.ToValue()
                                                         {
                                                            return this._value;
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="int"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                         public static implicit operator int?(global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return obj?._value;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion to the type <see cref="int"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                         public static explicit operator int(global::Thinktecture.Tests.TestValueObject obj)
                                                         {
                                                            if(obj is null)
                                                               throw new global::System.NullReferenceException();

                                                            return obj._value;
                                                         }

                                                         private TestValueObject(int @value)
                                                         {
                                                            ValidateConstructorArguments(ref @value);

                                                            this._value = @value;
                                                         }

                                                         static partial void ValidateConstructorArguments(ref int @value);

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            if (other is null)
                                                               return false;

                                                            if (global::System.Object.ReferenceEquals(this, other))
                                                               return true;

                                                            return this._value.Equals(other._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return global::System.HashCode.Combine(_typeHashCode, this._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this._value.ToString();
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_complex_struct_without_members()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject]
                   	public partial struct TestValueObject
                   	{
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      AssertOutput(output, _COMPLEX_STRUCT_WITHOUT_MEMBERS);
   }

   [Fact]
   public void Should_generate_complex_struct_with_custom_default_instance_property_name()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject(DefaultInstancePropertyName = "Null",
                                         AllowDefaultStructs = true)]
                   	public partial struct TestValueObject
                   	{
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      AssertOutput(output, _GENERATED_HEADER + """

                                               namespace Thinktecture.Tests
                                               {
                                                  readonly partial struct TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject>,
                                                     global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                     global::Thinktecture.IComplexValueObject
                                                  {
                                                     [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                     internal static void ModuleInit()
                                                     {
                                                        global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                                                                           {
                                                                                                                                                           };

                                                        var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

                                                        foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
                                                        {
                                                           members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
                                                        }

                                                        var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                        var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

                                                        global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                     }

                                                     private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                     public static readonly global::Thinktecture.Tests.TestValueObject Null = default;

                                                     public static global::Thinktecture.ValidationError? Validate(
                                                        out global::Thinktecture.Tests.TestValueObject obj)
                                                     {
                                                        global::Thinktecture.ValidationError? validationError = null;
                                                        ValidateFactoryArguments(
                                                           ref validationError);

                                                        if (validationError is null)
                                                        {
                                                           obj = new global::Thinktecture.Tests.TestValueObject();
                                                           obj.FactoryPostInit();
                                                        }
                                                        else
                                                        {
                                                           obj = default;
                                                        }

                                                        return validationError;
                                                     }

                                                     public static global::Thinktecture.Tests.TestValueObject Create()
                                                     {
                                                        var validationError = Validate(
                                                           out global::Thinktecture.Tests.TestValueObject obj);

                                                        if (validationError is not null)
                                                           throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                        return obj!;
                                                     }

                                                     public static bool TryCreate(
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj)
                                                     {
                                                        return TryCreate(
                                                           out obj,
                                                           out _);
                                                     }

                                                     public static bool TryCreate(
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                     {
                                                        validationError = Validate(
                                                           out obj);

                                                        return validationError is null;
                                                     }

                                                     static partial void ValidateFactoryArguments(
                                                        ref global::Thinktecture.ValidationError? validationError);

                                                     partial void FactoryPostInit();

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                     public static bool operator ==(global::Thinktecture.Tests.TestValueObject obj, global::Thinktecture.Tests.TestValueObject other)
                                                     {
                                                        return obj.Equals(other);
                                                     }

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                     public static bool operator !=(global::Thinktecture.Tests.TestValueObject obj, global::Thinktecture.Tests.TestValueObject other)
                                                     {
                                                        return !(obj == other);
                                                     }

                                                     /// <inheritdoc />
                                                     public override bool Equals(object? other)
                                                     {
                                                        return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                     }

                                                     /// <inheritdoc />
                                                     public bool Equals(global::Thinktecture.Tests.TestValueObject other)
                                                     {
                                                        return true;
                                                     }

                                                     /// <inheritdoc />
                                                     public override int GetHashCode()
                                                     {
                                                        return _typeHashCode;
                                                     }

                                                     /// <inheritdoc />
                                                     public override string ToString()
                                                     {
                                                        return "TestValueObject";
                                                     }
                                                  }
                                               }

                                               """);
   }

   [Fact]
   public void Should_generate_string_based_keyed_struct()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<string>]
                   	public partial struct TestValueObject
                   	{
                     }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_STRUCT_STRING);
      AssertOutput(parsableOutput, _PARSABLE_STRUCT_STRING);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_STRUCT_STRING);
      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_STRUCT);
      AssertOutput(mainOutput, _MAIN_OUTPUT_STRING_BASED_STRUCT);
   }

   [Fact]
   public void Should_generate_int_based_keyed_struct()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<int>]
                   	public partial struct TestValueObject
                   	{
                   	}
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(10);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var formattableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;
      var additionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs")).Value;
      var subtractionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs")).Value;
      var multiplyOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs")).Value;
      var divisionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs")).Value;

      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_STRUCT);
      AssertOutput(formattableOutput, _FORMATTABLE_STRUCT);
      AssertOutput(comparableOutput, _COMPARABLE_STRUCT);
      AssertOutput(parsableOutput, _PARSABLE_STRUCT);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_STRUCT);
      AssertOutput(additionOperatorsOutput, _ADDITION_OPERATORS_STRUCT);
      AssertOutput(subtractionOperatorsOutput, _SUBTRACTION_OPERATORS_STRUCT);
      AssertOutput(multiplyOperatorsOutput, _MULTIPLY_OPERATORS_STRUCT);
      AssertOutput(divisionOperatorsOutput, _DIVISION_OPERATORS_STRUCT);
      AssertOutput(mainOutput, _MAIN_OUTPUT_INT_BASED_STRUCT);
   }

   [Fact]
   public void Should_generate_int_based_keyed_struct_custom_default_instance_property_name()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<int>(DefaultInstancePropertyName = "Null",
                                       AllowDefaultStructs = true)]
                   	public partial struct TestValueObject
                   	{
                   	}
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(10);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var formattableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;
      var additionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs")).Value;
      var subtractionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs")).Value;
      var multiplyOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs")).Value;
      var divisionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs")).Value;

      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_STRUCT);
      AssertOutput(formattableOutput, _FORMATTABLE_STRUCT);
      AssertOutput(comparableOutput, _COMPARABLE_STRUCT);
      AssertOutput(parsableOutput, _PARSABLE_STRUCT);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_STRUCT);
      AssertOutput(additionOperatorsOutput, _ADDITION_OPERATORS_STRUCT);
      AssertOutput(subtractionOperatorsOutput, _SUBTRACTION_OPERATORS_STRUCT);
      AssertOutput(multiplyOperatorsOutput, _MULTIPLY_OPERATORS_STRUCT);
      AssertOutput(divisionOperatorsOutput, _DIVISION_OPERATORS_STRUCT);
      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                                  namespace Thinktecture.Tests
                                                                  {
                                                                     [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]
                                                                     [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>))]
                                                                     readonly partial struct TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject>,
                                                                        global::Thinktecture.IKeyedValueObject<int>,
                                                                        global::Thinktecture.IValueObjectConvertable<int>,
                                                                        global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>
                                                                     {
                                                                        [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                                        internal static void ModuleInit()
                                                                        {
                                                                           global::System.Func<int, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                                           global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                                           global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                                           var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, int>(static item => item._value);
                                                                           global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, int>> convertToKeyExpression = static obj => obj._value;

                                                                           var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                                           var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(int), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                                           global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                                        }

                                                                        private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                                        public static readonly global::Thinktecture.Tests.TestValueObject Null = default;

                                                                        /// <summary>
                                                                        /// The identifier of this object.
                                                                        /// </summary>
                                                                        private readonly int _value;

                                                                        public static global::Thinktecture.ValidationError? Validate(int @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject obj)
                                                                        {
                                                                           global::Thinktecture.ValidationError? validationError = null;
                                                                           ValidateFactoryArguments(ref validationError, ref @value);

                                                                           if (validationError is null)
                                                                           {
                                                                              obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                                              obj.FactoryPostInit();
                                                                           }
                                                                           else
                                                                           {
                                                                              obj = default;
                                                                           }

                                                                           return validationError;
                                                                        }

                                                                        public static global::Thinktecture.Tests.TestValueObject Create(int @value)
                                                                        {
                                                                           var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject obj);

                                                                           if (validationError is not null)
                                                                              throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                                           return obj!;
                                                                        }

                                                                        public static bool TryCreate(int @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj)
                                                                        {
                                                                           return TryCreate(@value, out obj, out _);
                                                                        }

                                                                        public static bool TryCreate(
                                                                           int @value,
                                                                           [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj,
                                                                           [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                                        {
                                                                           validationError = Validate(@value, null, out obj);

                                                                           return validationError is null;
                                                                        }

                                                                        static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, ref int @value);

                                                                        partial void FactoryPostInit();

                                                                        /// <summary>
                                                                        /// Gets the identifier of the item.
                                                                        /// </summary>
                                                                        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                                        int global::Thinktecture.IValueObjectConvertable<int>.ToValue()
                                                                        {
                                                                           return this._value;
                                                                        }

                                                                        /// <summary>
                                                                        /// Implicit conversion to the type <see cref="int"/>.
                                                                        /// </summary>
                                                                        /// <param name="obj">Object to covert.</param>
                                                                        /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                                        [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                                        public static implicit operator int?(global::Thinktecture.Tests.TestValueObject? obj)
                                                                        {
                                                                           return obj?._value;
                                                                        }

                                                                        /// <summary>
                                                                        /// Implicit conversion to the type <see cref="int"/>.
                                                                        /// </summary>
                                                                        /// <param name="obj">Object to covert.</param>
                                                                        /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/>.</returns>
                                                                        public static implicit operator int(global::Thinktecture.Tests.TestValueObject obj)
                                                                        {
                                                                           return obj._value;
                                                                        }

                                                                        /// <summary>
                                                                        /// Explicit conversion from the type <see cref="int"/>.
                                                                        /// </summary>
                                                                        /// <param name="value">Value to covert.</param>
                                                                        /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                                        public static explicit operator global::Thinktecture.Tests.TestValueObject(int @value)
                                                                        {
                                                                           return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                                        }

                                                                        private TestValueObject(int @value)
                                                                        {
                                                                           ValidateConstructorArguments(ref @value);

                                                                           this._value = @value;
                                                                        }

                                                                        static partial void ValidateConstructorArguments(ref int @value);

                                                                        /// <inheritdoc />
                                                                        public override bool Equals(object? other)
                                                                        {
                                                                           return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                                        }

                                                                        /// <inheritdoc />
                                                                        public bool Equals(global::Thinktecture.Tests.TestValueObject other)
                                                                        {
                                                                           return this._value.Equals(other._value);
                                                                        }

                                                                        /// <inheritdoc />
                                                                        public override int GetHashCode()
                                                                        {
                                                                           return global::System.HashCode.Combine(_typeHashCode, this._value);
                                                                        }

                                                                        /// <inheritdoc />
                                                                        public override string ToString()
                                                                        {
                                                                           return this._value.ToString();
                                                                        }
                                                                     }
                                                                  }

                                                                  """);
   }

   [Fact]
   public void Should_generate_int_based_keyed_struct_with_custom_int_key_member_with_init_only()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<int>(SkipKeyMember = true)]
                   	public partial struct TestValueObject
                   	{
                         public readonly int _value { get; private init; }
                     }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(10);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var formattableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;
      var additionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs")).Value;
      var subtractionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs")).Value;
      var multiplyOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs")).Value;
      var divisionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs")).Value;

      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_STRUCT);
      AssertOutput(formattableOutput, _FORMATTABLE_STRUCT);
      AssertOutput(comparableOutput, _COMPARABLE_STRUCT);
      AssertOutput(parsableOutput, _PARSABLE_STRUCT);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_STRUCT);
      AssertOutput(additionOperatorsOutput, _ADDITION_OPERATORS_STRUCT);
      AssertOutput(subtractionOperatorsOutput, _SUBTRACTION_OPERATORS_STRUCT);
      AssertOutput(multiplyOperatorsOutput, _MULTIPLY_OPERATORS_STRUCT);
      AssertOutput(divisionOperatorsOutput, _DIVISION_OPERATORS_STRUCT);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>))]
                                                      readonly partial struct TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject>,
                                                         global::Thinktecture.IKeyedValueObject<int>,
                                                         global::Thinktecture.IValueObjectConvertable<int>,
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            global::System.Func<int, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, int>(static item => item._value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, int>> convertToKeyExpression = static obj => obj._value;

                                                            var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(int), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                         }

                                                         private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                         public static global::Thinktecture.ValidationError? Validate(int @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject obj)
                                                         {
                                                            global::Thinktecture.ValidationError? validationError = null;
                                                            ValidateFactoryArguments(ref validationError, ref @value);

                                                            if (validationError is null)
                                                            {
                                                               obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                               obj.FactoryPostInit();
                                                            }
                                                            else
                                                            {
                                                               obj = default;
                                                            }

                                                            return validationError;
                                                         }

                                                         public static global::Thinktecture.Tests.TestValueObject Create(int @value)
                                                         {
                                                            var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject obj);

                                                            if (validationError is not null)
                                                               throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                            return obj!;
                                                         }

                                                         public static bool TryCreate(int @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj)
                                                         {
                                                            return TryCreate(@value, out obj, out _);
                                                         }

                                                         public static bool TryCreate(
                                                            int @value,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                         {
                                                            validationError = Validate(@value, null, out obj);

                                                            return validationError is null;
                                                         }

                                                         static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, ref int @value);

                                                         partial void FactoryPostInit();

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         int global::Thinktecture.IValueObjectConvertable<int>.ToValue()
                                                         {
                                                            return this._value;
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="int"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                         public static implicit operator int?(global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return obj?._value;
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="int"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/>.</returns>
                                                         public static implicit operator int(global::Thinktecture.Tests.TestValueObject obj)
                                                         {
                                                            return obj._value;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="int"/>.
                                                         /// </summary>
                                                         /// <param name="value">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                         public static explicit operator global::Thinktecture.Tests.TestValueObject(int @value)
                                                         {
                                                            return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                         }

                                                         private TestValueObject(int @value)
                                                         {
                                                            ValidateConstructorArguments(ref @value);

                                                            this._value = @value;
                                                         }

                                                         static partial void ValidateConstructorArguments(ref int @value);

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestValueObject other)
                                                         {
                                                            return this._value.Equals(other._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return global::System.HashCode.Combine(_typeHashCode, this._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this._value.ToString();
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_string_based_keyed_struct_with_NullInFactoryMethodsYieldsNull_which_should_be_ignored()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<string>(NullInFactoryMethodsYieldsNull = true)]
                   	public partial struct TestValueObject
                   	{
                     }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_STRUCT_STRING);
      AssertOutput(parsableOutput, _PARSABLE_STRUCT_STRING);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_STRUCT_STRING);
      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_STRUCT);
      AssertOutput(mainOutput, _MAIN_OUTPUT_STRING_BASED_STRUCT);
   }

   [Fact]
   public void Should_generate_string_based_keyed_struct_with_EmptyStringInFactoryMethodsYieldsNull_which_should_be_ignored()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<string>(EmptyStringInFactoryMethodsYieldsNull = true)]
                   	public partial struct TestValueObject
                   	{
                      }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_STRUCT_STRING);
      AssertOutput(parsableOutput, _PARSABLE_STRUCT_STRING);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_STRUCT_STRING);
      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_STRUCT);
      AssertOutput(mainOutput, _MAIN_OUTPUT_STRING_BASED_STRUCT);
   }

   [Fact]
   public void Should_generate_string_based_keyed_class()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<string>]
                   	public partial class TestValueObject
                   	{
                     }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_CLASS_STRING);
      AssertOutput(parsableOutput, _PARSABLE_CLASS_STRING);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_CLASS_STRING);
      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_CLASS);
      AssertOutput(mainOutput, _MAIN_OUTPUT_STRING_BASED_CLASS);
   }

   [Fact]
   public void Should_generate_int_based_keyed_class()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<int>]
                   	public partial class TestValueObject
                   	{
                     }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(10);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var formattableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;
      var additionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs")).Value;
      var subtractionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs")).Value;
      var multiplyOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs")).Value;
      var divisionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs")).Value;

      AssertOutput(formattableOutput, _FORMATTABLE_CLASS);
      AssertOutput(comparableOutput, _COMPARABLE_CLASS);
      AssertOutput(parsableOutput, _PARSABLE_CLASS);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_CLASS);
      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_CLASS);
      AssertOutput(additionOperatorsOutput, _ADDITION_OPERATORS_CLASS);
      AssertOutput(subtractionOperatorsOutput, _SUBTRACTION_OPERATORS_CLASS);
      AssertOutput(multiplyOperatorsOutput, _MULTIPLY_OPERATORS_CLASS);
      AssertOutput(divisionOperatorsOutput, _DIVISION_OPERATORS_CLASS);
      AssertOutput(mainOutput, _MAIN_OUTPUT_INT_BASED_CLASS);
   }

   [Fact]
   public void Should_generate_DateOnly_based_keyed_class()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<DateOnly>(KeyMemberName = "_value")]
                   	public partial class TestValueObject
                   	{
                     }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(6);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var formattableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly, global::Thinktecture.ValidationError>))]
                                                      sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                         global::Thinktecture.IKeyedValueObject<global::System.DateOnly>,
                                                         global::Thinktecture.IValueObjectConvertable<global::System.DateOnly>,
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly, global::Thinktecture.ValidationError>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            global::System.Func<global::System.DateOnly, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::System.DateOnly, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::System.DateOnly, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly>(static item => item._value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly>> convertToKeyExpression = static obj => obj._value;

                                                            var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(global::System.DateOnly), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                         }

                                                         private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                         /// <summary>
                                                         /// The identifier of this object.
                                                         /// </summary>
                                                         private readonly global::System.DateOnly _value;

                                                         public static global::Thinktecture.ValidationError? Validate(global::System.DateOnly @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            global::Thinktecture.ValidationError? validationError = null;
                                                            ValidateFactoryArguments(ref validationError, ref @value);

                                                            if (validationError is null)
                                                            {
                                                               obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                               obj.FactoryPostInit();
                                                            }
                                                            else
                                                            {
                                                               obj = default;
                                                            }

                                                            return validationError;
                                                         }

                                                         public static global::Thinktecture.Tests.TestValueObject Create(global::System.DateOnly @value)
                                                         {
                                                            var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject? obj);

                                                            if (validationError is not null)
                                                               throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                            return obj!;
                                                         }

                                                         public static bool TryCreate(global::System.DateOnly @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return TryCreate(@value, out obj, out _);
                                                         }

                                                         public static bool TryCreate(
                                                            global::System.DateOnly @value,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                         {
                                                            validationError = Validate(@value, null, out obj);

                                                            return validationError is null;
                                                         }

                                                         static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, ref global::System.DateOnly @value);

                                                         partial void FactoryPostInit();

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         global::System.DateOnly global::Thinktecture.IValueObjectConvertable<global::System.DateOnly>.ToValue()
                                                         {
                                                            return this._value;
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="global::System.DateOnly"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                         public static implicit operator global::System.DateOnly?(global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return obj?._value;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion to the type <see cref="global::System.DateOnly"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                         public static explicit operator global::System.DateOnly(global::Thinktecture.Tests.TestValueObject obj)
                                                         {
                                                            if(obj is null)
                                                               throw new global::System.NullReferenceException();

                                                            return obj._value;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="global::System.DateOnly"/>.
                                                         /// </summary>
                                                         /// <param name="value">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                         public static explicit operator global::Thinktecture.Tests.TestValueObject(global::System.DateOnly @value)
                                                         {
                                                            return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                         }

                                                         private TestValueObject(global::System.DateOnly @value)
                                                         {
                                                            ValidateConstructorArguments(ref @value);

                                                            this._value = @value;
                                                         }

                                                         static partial void ValidateConstructorArguments(ref global::System.DateOnly @value);

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            if (other is null)
                                                               return false;

                                                            if (global::System.Object.ReferenceEquals(this, other))
                                                               return true;

                                                            return this._value.Equals(other._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return global::System.HashCode.Combine(_typeHashCode, this._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this._value.ToString();
                                                         }
                                                      }
                                                   }

                                                   """);

      AssertOutput(formattableOutput, _GENERATED_HEADER + """

                                                          namespace Thinktecture.Tests;

                                                          partial class TestValueObject :
                                                             global::System.IFormattable
                                                          {
                                                             /// <inheritdoc />
                                                             public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)
                                                             {
                                                                return this._value.ToString(format, formatProvider);
                                                             }
                                                          }

                                                          """);

      AssertOutput(comparableOutput, _GENERATED_HEADER + """

                                                         namespace Thinktecture.Tests;

                                                         partial class TestValueObject :
                                                            global::System.IComparable,
                                                            global::System.IComparable<global::Thinktecture.Tests.TestValueObject>
                                                         {
                                                            /// <inheritdoc />
                                                            public int CompareTo(object? obj)
                                                            {
                                                               if(obj is null)
                                                                  return 1;

                                                               if(obj is not global::Thinktecture.Tests.TestValueObject item)
                                                                  throw new global::System.ArgumentException("Argument must be of type \"TestValueObject\".", nameof(obj));

                                                               return this.CompareTo(item);
                                                            }

                                                            /// <inheritdoc />
                                                            public int CompareTo(global::Thinktecture.Tests.TestValueObject? obj)
                                                            {
                                                               if(obj is null)
                                                                  return 1;

                                                               return this._value.CompareTo(obj._value);
                                                            }
                                                         }

                                                         """);

      AssertOutput(parsableOutput, _GENERATED_HEADER + """

                                                       namespace Thinktecture.Tests;

                                                       partial class TestValueObject :
                                                          global::System.IParsable<global::Thinktecture.Tests.TestValueObject>
                                                       {
                                                          private static global::Thinktecture.ValidationError? Validate<T>(global::System.DateOnly key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestValueObject? result)
                                                             where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly, global::Thinktecture.ValidationError>
                                                          {
                                                             return T.Validate(key, provider, out result);
                                                          }

                                                          /// <inheritdoc />
                                                          public static global::Thinktecture.Tests.TestValueObject Parse(string s, global::System.IFormatProvider? provider)
                                                          {
                                                             var key = global::System.DateOnly.Parse(s, provider);
                                                             var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(key, provider, out var result);

                                                             if(validationError is null)
                                                                return result!;

                                                             throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestValueObject\".");
                                                          }

                                                          /// <inheritdoc />
                                                          public static bool TryParse(
                                                             string? s,
                                                             global::System.IFormatProvider? provider,
                                                             [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestValueObject result)
                                                          {
                                                             if(s is null)
                                                             {
                                                                result = default;
                                                                return false;
                                                             }

                                                             if(!global::System.DateOnly.TryParse(s, provider, out var key))
                                                             {
                                                                result = default;
                                                                return false;
                                                             }

                                                             var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(key, provider, out result!);
                                                             return validationError is null;
                                                          }
                                                       }

                                                       """);

      /* language=c# */
      AssertOutput(comparisonOperatorsOutput, _GENERATED_HEADER + """

                                                                  namespace Thinktecture.Tests;

                                                                  partial class TestValueObject :
                                                                     global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>
                                                                  {
                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                     public static bool operator <(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value < right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator <=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value <= right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                     public static bool operator >(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value > right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator >=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value >= right._value;
                                                                     }
                                                                  }

                                                                  """);

      AssertOutput(equalityComparisonOperatorsOutput, _GENERATED_HEADER + """

                                                                          namespace Thinktecture.Tests;

                                                                          partial class TestValueObject :
                                                                             global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>
                                                                          {
                                                                                /// <summary>
                                                                                /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                /// </summary>
                                                                                /// <param name="obj">Instance to compare.</param>
                                                                                /// <param name="other">Another instance to compare.</param>
                                                                                /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                                                {
                                                                                   if (obj is null)
                                                                                      return other is null;

                                                                                   return obj.Equals(other);
                                                                                }

                                                                                /// <summary>
                                                                                /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                /// </summary>
                                                                                /// <param name="obj">Instance to compare.</param>
                                                                                /// <param name="other">Another instance to compare.</param>
                                                                                /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                                                {
                                                                                   return !(obj == other);
                                                                                }
                                                                          }

                                                                          """);
   }

   [Fact]
   public void Should_generate_DateOnly_based_keyed_class_with_DefaultWithKeyTypeOverloads()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<DateOnly>(EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
                   	public partial class TestValueObject
                   	{
                     }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(6);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var formattableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly, global::Thinktecture.ValidationError>))]
                                                      sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                         global::Thinktecture.IKeyedValueObject<global::System.DateOnly>,
                                                         global::Thinktecture.IValueObjectConvertable<global::System.DateOnly>,
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly, global::Thinktecture.ValidationError>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            global::System.Func<global::System.DateOnly, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::System.DateOnly, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::System.DateOnly, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly>(static item => item._value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly>> convertToKeyExpression = static obj => obj._value;

                                                            var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(global::System.DateOnly), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                         }

                                                         private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                         /// <summary>
                                                         /// The identifier of this object.
                                                         /// </summary>
                                                         private readonly global::System.DateOnly _value;

                                                         public static global::Thinktecture.ValidationError? Validate(global::System.DateOnly @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            global::Thinktecture.ValidationError? validationError = null;
                                                            ValidateFactoryArguments(ref validationError, ref @value);

                                                            if (validationError is null)
                                                            {
                                                               obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                               obj.FactoryPostInit();
                                                            }
                                                            else
                                                            {
                                                               obj = default;
                                                            }

                                                            return validationError;
                                                         }

                                                         public static global::Thinktecture.Tests.TestValueObject Create(global::System.DateOnly @value)
                                                         {
                                                            var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject? obj);

                                                            if (validationError is not null)
                                                               throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                            return obj!;
                                                         }

                                                         public static bool TryCreate(global::System.DateOnly @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return TryCreate(@value, out obj, out _);
                                                         }

                                                         public static bool TryCreate(
                                                            global::System.DateOnly @value,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                         {
                                                            validationError = Validate(@value, null, out obj);

                                                            return validationError is null;
                                                         }

                                                         static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, ref global::System.DateOnly @value);

                                                         partial void FactoryPostInit();

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         global::System.DateOnly global::Thinktecture.IValueObjectConvertable<global::System.DateOnly>.ToValue()
                                                         {
                                                            return this._value;
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="global::System.DateOnly"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                         public static implicit operator global::System.DateOnly?(global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return obj?._value;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion to the type <see cref="global::System.DateOnly"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                         public static explicit operator global::System.DateOnly(global::Thinktecture.Tests.TestValueObject obj)
                                                         {
                                                            if(obj is null)
                                                               throw new global::System.NullReferenceException();

                                                            return obj._value;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="global::System.DateOnly"/>.
                                                         /// </summary>
                                                         /// <param name="value">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                         public static explicit operator global::Thinktecture.Tests.TestValueObject(global::System.DateOnly @value)
                                                         {
                                                            return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                         }

                                                         private TestValueObject(global::System.DateOnly @value)
                                                         {
                                                            ValidateConstructorArguments(ref @value);

                                                            this._value = @value;
                                                         }

                                                         static partial void ValidateConstructorArguments(ref global::System.DateOnly @value);

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            if (other is null)
                                                               return false;

                                                            if (global::System.Object.ReferenceEquals(this, other))
                                                               return true;

                                                            return this._value.Equals(other._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return global::System.HashCode.Combine(_typeHashCode, this._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this._value.ToString();
                                                         }
                                                      }
                                                   }

                                                   """);

      AssertOutput(formattableOutput, _GENERATED_HEADER + """

                                                          namespace Thinktecture.Tests;

                                                          partial class TestValueObject :
                                                             global::System.IFormattable
                                                          {
                                                             /// <inheritdoc />
                                                             public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)
                                                             {
                                                                return this._value.ToString(format, formatProvider);
                                                             }
                                                          }

                                                          """);

      AssertOutput(comparableOutput, _GENERATED_HEADER + """

                                                         namespace Thinktecture.Tests;

                                                         partial class TestValueObject :
                                                            global::System.IComparable,
                                                            global::System.IComparable<global::Thinktecture.Tests.TestValueObject>
                                                         {
                                                            /// <inheritdoc />
                                                            public int CompareTo(object? obj)
                                                            {
                                                               if(obj is null)
                                                                  return 1;

                                                               if(obj is not global::Thinktecture.Tests.TestValueObject item)
                                                                  throw new global::System.ArgumentException("Argument must be of type \"TestValueObject\".", nameof(obj));

                                                               return this.CompareTo(item);
                                                            }

                                                            /// <inheritdoc />
                                                            public int CompareTo(global::Thinktecture.Tests.TestValueObject? obj)
                                                            {
                                                               if(obj is null)
                                                                  return 1;

                                                               return this._value.CompareTo(obj._value);
                                                            }
                                                         }

                                                         """);

      AssertOutput(parsableOutput, _GENERATED_HEADER + """

                                                       namespace Thinktecture.Tests;

                                                       partial class TestValueObject :
                                                          global::System.IParsable<global::Thinktecture.Tests.TestValueObject>
                                                       {
                                                          private static global::Thinktecture.ValidationError? Validate<T>(global::System.DateOnly key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestValueObject? result)
                                                             where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly, global::Thinktecture.ValidationError>
                                                          {
                                                             return T.Validate(key, provider, out result);
                                                          }

                                                          /// <inheritdoc />
                                                          public static global::Thinktecture.Tests.TestValueObject Parse(string s, global::System.IFormatProvider? provider)
                                                          {
                                                             var key = global::System.DateOnly.Parse(s, provider);
                                                             var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(key, provider, out var result);

                                                             if(validationError is null)
                                                                return result!;

                                                             throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestValueObject\".");
                                                          }

                                                          /// <inheritdoc />
                                                          public static bool TryParse(
                                                             string? s,
                                                             global::System.IFormatProvider? provider,
                                                             [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestValueObject result)
                                                          {
                                                             if(s is null)
                                                             {
                                                                result = default;
                                                                return false;
                                                             }

                                                             if(!global::System.DateOnly.TryParse(s, provider, out var key))
                                                             {
                                                                result = default;
                                                                return false;
                                                             }

                                                             var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(key, provider, out result!);
                                                             return validationError is null;
                                                          }
                                                       }

                                                       """);

      AssertOutput(comparisonOperatorsOutput, _GENERATED_HEADER + """

                                                                  namespace Thinktecture.Tests;

                                                                  partial class TestValueObject :
                                                                     global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>
                                                                  {
                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                     public static bool operator <(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value < right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator <=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value <= right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                     public static bool operator >(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value > right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator >=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value >= right._value;
                                                                     }
                                                                  }

                                                                  """);

      AssertOutput(equalityComparisonOperatorsOutput, _GENERATED_HEADER + """

                                                                          namespace Thinktecture.Tests;

                                                                          partial class TestValueObject :
                                                                             global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                                             global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly, bool>
                                                                          {
                                                                                /// <summary>
                                                                                /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                /// </summary>
                                                                                /// <param name="obj">Instance to compare.</param>
                                                                                /// <param name="other">Another instance to compare.</param>
                                                                                /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                                                {
                                                                                   if (obj is null)
                                                                                      return other is null;

                                                                                   return obj.Equals(other);
                                                                                }

                                                                                /// <summary>
                                                                                /// Compares two instances of <see cref="TestValueObject"/>.
                                                                                /// </summary>
                                                                                /// <param name="obj">Instance to compare.</param>
                                                                                /// <param name="other">Another instance to compare.</param>
                                                                                /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                                                {
                                                                                   return !(obj == other);
                                                                                }

                                                                                private static bool Equals(global::Thinktecture.Tests.TestValueObject? obj, global::System.DateOnly value)
                                                                                {
                                                                                   if (obj is null)
                                                                                      return false;

                                                                                   return obj._value.Equals(value);
                                                                                }

                                                                                /// <summary>
                                                                                /// Compares an instance of <see cref="TestValueObject"/> with <see cref="global::System.DateOnly"/>.
                                                                                /// </summary>
                                                                                /// <param name="obj">Instance to compare.</param>
                                                                                /// <param name="value">Value to compare with.</param>
                                                                                /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::System.DateOnly value)
                                                                                {
                                                                                   return Equals(obj, value);
                                                                                }

                                                                                /// <summary>
                                                                                /// Compares an instance of <see cref="TestValueObject"/> with <see cref="global::System.DateOnly"/>.
                                                                                /// </summary>
                                                                                /// <param name="value">Value to compare.</param>
                                                                                /// <param name="obj">Instance to compare with.</param>
                                                                                /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                                                public static bool operator ==(global::System.DateOnly value, global::Thinktecture.Tests.TestValueObject? obj)
                                                                                {
                                                                                   return Equals(obj, value);
                                                                                }

                                                                                /// <summary>
                                                                                /// Compares an instance of <see cref="TestValueObject"/> with <see cref="global::System.DateOnly"/>.
                                                                                /// </summary>
                                                                                /// <param name="obj">Instance to compare.</param>
                                                                                /// <param name="value">Value to compare with.</param>
                                                                                /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::System.DateOnly value)
                                                                                {
                                                                                   return !(obj == value);
                                                                                }

                                                                                /// <summary>
                                                                                /// Compares an instance of <see cref="global::System.DateOnly"/> with <see cref="TestValueObject"/>.
                                                                                /// </summary>
                                                                                /// <param name="value">Value to compare.</param>
                                                                                /// <param name="obj">Instance to compare with.</param>
                                                                                /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                                                public static bool operator !=(global::System.DateOnly value, global::Thinktecture.Tests.TestValueObject? obj)
                                                                                {
                                                                                   return !(obj == value);
                                                                                }
                                                                          }

                                                                          """);
   }

   [Fact]
   public void Should_generate_int_based_keyed_class_with_DefaultWithKeyTypeOverloads()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                                AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                                SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                                MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                                DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
                   	public partial class TestValueObject
                   	{
                     }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(10);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var formattableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Formattable.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;
      var additionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs")).Value;
      var subtractionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs")).Value;
      var multiplyOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs")).Value;
      var divisionOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs")).Value;

      AssertOutput(formattableOutput, _FORMATTABLE_CLASS);
      AssertOutput(comparableOutput, _COMPARABLE_CLASS);
      AssertOutput(parsableOutput, _PARSABLE_CLASS);
      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_INT_WITH_KEY_OVERLOADS);
      AssertOutput(mainOutput, _MAIN_OUTPUT_INT_BASED_CLASS);

      AssertOutput(comparisonOperatorsOutput, _GENERATED_HEADER + """

                                                                  namespace Thinktecture.Tests;

                                                                  partial class TestValueObject :
                                                                     global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                                     global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestValueObject, int, bool>
                                                                  {
                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                     public static bool operator <(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value < right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator <=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value <= right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                     public static bool operator >(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value > right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator >=(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left._value >= right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                     public static bool operator <(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        return left._value < right;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
                                                                     public static bool operator <(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left < right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator <=(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        return left._value <= right;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator <=(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left <= right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                     public static bool operator >(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        return left._value > right;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
                                                                     public static bool operator >(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left > right._value;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator >=(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                        return left._value >= right;
                                                                     }

                                                                     /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
                                                                     public static bool operator >=(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                     {
                                                                        global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                        return left >= right._value;
                                                                     }
                                                                  }

                                                                  """);

      AssertOutput(additionOperatorsOutput, _GENERATED_HEADER + """

                                                                namespace Thinktecture.Tests;

                                                                partial class TestValueObject :
                                                                   global::System.Numerics.IAdditionOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>,
                                                                   global::System.Numerics.IAdditionOperators<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.Tests.TestValueObject>
                                                                {
                                                                   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator +(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create((left._value + right._value));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator checked +(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create(checked((left._value + right._value)));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator +(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      return Create((left._value + right));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator +(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create((left + right._value));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator checked +(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      return Create(checked((left._value + right)));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator checked +(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create(checked((left + right._value)));
                                                                   }
                                                                }

                                                                """);

      AssertOutput(subtractionOperatorsOutput, _GENERATED_HEADER + """

                                                                   namespace Thinktecture.Tests;

                                                                   partial class TestValueObject :
                                                                      global::System.Numerics.ISubtractionOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>,
                                                                      global::System.Numerics.ISubtractionOperators<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.Tests.TestValueObject>
                                                                   {
                                                                      /// <inheritdoc cref="global::System.Numerics.ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)" />
                                                                      public static global::Thinktecture.Tests.TestValueObject operator -(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                      {
                                                                         global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                         global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                         return Create((left._value - right._value));
                                                                      }

                                                                      /// <inheritdoc cref="global::System.Numerics.ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)" />
                                                                      public static global::Thinktecture.Tests.TestValueObject operator checked -(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                      {
                                                                         global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                         global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                         return Create(checked((left._value - right._value)));
                                                                      }

                                                                      /// <inheritdoc cref="global::System.Numerics.ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)" />
                                                                      public static global::Thinktecture.Tests.TestValueObject operator -(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                      {
                                                                         global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                         return Create((left._value - right));
                                                                      }

                                                                      /// <inheritdoc cref="global::System.Numerics.ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)" />
                                                                      public static global::Thinktecture.Tests.TestValueObject operator -(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                      {
                                                                         global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                         return Create((left - right._value));
                                                                      }

                                                                      /// <inheritdoc cref="global::System.Numerics.ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)" />
                                                                      public static global::Thinktecture.Tests.TestValueObject operator checked -(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                      {
                                                                         global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                         return Create(checked((left._value - right)));
                                                                      }

                                                                      /// <inheritdoc cref="global::System.Numerics.ISubtractionOperators{TSelf, TOther, TResult}.op_Subtraction(TSelf, TOther)" />
                                                                      public static global::Thinktecture.Tests.TestValueObject operator checked -(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                      {
                                                                         global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                         return Create(checked((left - right._value)));
                                                                      }
                                                                   }

                                                                   """);

      AssertOutput(multiplyOperatorsOutput, _GENERATED_HEADER + """

                                                                namespace Thinktecture.Tests;

                                                                partial class TestValueObject :
                                                                   global::System.Numerics.IMultiplyOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>,
                                                                   global::System.Numerics.IMultiplyOperators<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.Tests.TestValueObject>
                                                                {
                                                                   /// <inheritdoc cref="global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator *(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create((left._value * right._value));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator checked *(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create(checked((left._value * right._value)));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator *(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      return Create((left._value * right));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator *(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create((left * right._value));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator checked *(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      return Create(checked((left._value * right)));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IMultiplyOperators{TSelf, TOther, TResult}.op_Multiply(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator checked *(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create(checked((left * right._value)));
                                                                   }
                                                                }

                                                                """);

      AssertOutput(divisionOperatorsOutput, _GENERATED_HEADER + """

                                                                namespace Thinktecture.Tests;

                                                                partial class TestValueObject :
                                                                   global::System.Numerics.IDivisionOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>,
                                                                   global::System.Numerics.IDivisionOperators<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.Tests.TestValueObject>
                                                                {
                                                                   /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator /(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create((left._value / right._value));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator checked /(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create(checked((left._value / right._value)));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator /(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      return Create((left._value / right));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator /(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create((left / right._value));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator checked /(global::Thinktecture.Tests.TestValueObject left, int right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
                                                                      return Create(checked((left._value / right)));
                                                                   }

                                                                   /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
                                                                   public static global::Thinktecture.Tests.TestValueObject operator checked /(int left, global::Thinktecture.Tests.TestValueObject right)
                                                                   {
                                                                      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
                                                                      return Create(checked((left / right._value)));
                                                                   }
                                                                }

                                                                """);
   }

   [Fact]
   public void Should_generate_string_based_keyed_class_with_NullInFactoryMethodsYieldsNull()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<string>(NullInFactoryMethodsYieldsNull = true)]
                   	public partial class TestValueObject
                   	{
                   	}
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_CLASS_STRING);
      AssertOutput(parsableOutput, _PARSABLE_CLASS_STRING);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_CLASS_STRING);
      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>))]
                                                      sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                         global::Thinktecture.IKeyedValueObject<string>,
                                                         global::Thinktecture.IValueObjectConvertable<string>,
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            global::System.Func<string, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, string>(static item => item._value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, string>> convertToKeyExpression = static obj => obj._value;

                                                            var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(string), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                         }

                                                         private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                         /// <summary>
                                                         /// The identifier of this object.
                                                         /// </summary>
                                                         private readonly string _value;

                                                         public static global::Thinktecture.ValidationError? Validate(string? @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            if(@value is null)
                                                            {
                                                               obj = default;
                                                               return null;
                                                            }

                                                            global::Thinktecture.ValidationError? validationError = null;
                                                            ValidateFactoryArguments(ref validationError, ref @value);

                                                            if (validationError is null)
                                                            {
                                                               obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                               obj.FactoryPostInit();
                                                            }
                                                            else
                                                            {
                                                               obj = default;
                                                            }

                                                            return validationError;
                                                         }

                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("value")]
                                                         public static global::Thinktecture.Tests.TestValueObject? Create(string? @value)
                                                         {
                                                            var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject? obj);

                                                            if (validationError is not null)
                                                               throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                            return obj;
                                                         }

                                                         public static bool TryCreate(string? @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return TryCreate(@value, out obj, out _);
                                                         }

                                                         public static bool TryCreate(
                                                            string? @value,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                         {
                                                            validationError = Validate(@value, null, out obj);

                                                            return validationError is null;
                                                         }

                                                         static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value);

                                                         partial void FactoryPostInit();

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this._value;
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return obj?._value;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="value">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("value")]
                                                         public static explicit operator global::Thinktecture.Tests.TestValueObject?(string? @value)
                                                         {
                                                            if(@value is null)
                                                               return null;

                                                            return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                         }

                                                         private TestValueObject(string @value)
                                                         {
                                                            ValidateConstructorArguments(ref @value);

                                                            this._value = @value;
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string @value);

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            if (other is null)
                                                               return false;

                                                            if (global::System.Object.ReferenceEquals(this, other))
                                                               return true;

                                                            return global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value, other._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return global::System.HashCode.Combine(_typeHashCode, global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(this._value));
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this._value.ToString();
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_string_based_keyed_class_with_EmptyStringInFactoryMethodsYieldsNull()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<string>(EmptyStringInFactoryMethodsYieldsNull = true)]
                   	public partial class TestValueObject
                   	{
                   	}
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_CLASS_STRING);
      AssertOutput(parsableOutput, _PARSABLE_CLASS_STRING);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_CLASS_STRING);
      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>))]
                                                      sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                         global::Thinktecture.IKeyedValueObject<string>,
                                                         global::Thinktecture.IValueObjectConvertable<string>,
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            global::System.Func<string, global::Thinktecture.Tests.TestValueObject?> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string, global::Thinktecture.Tests.TestValueObject?>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, string>(static item => item._value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, string>> convertToKeyExpression = static obj => obj._value;

                                                            var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(string), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                         }

                                                         private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                         /// <summary>
                                                         /// The identifier of this object.
                                                         /// </summary>
                                                         private readonly string _value;

                                                         public static global::Thinktecture.ValidationError? Validate(string? @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            if(global::System.String.IsNullOrWhiteSpace(@value))
                                                            {
                                                               obj = default;
                                                               return null;
                                                            }

                                                            global::Thinktecture.ValidationError? validationError = null;
                                                            ValidateFactoryArguments(ref validationError, ref @value);

                                                            if (validationError is null)
                                                            {
                                                               obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                               obj.FactoryPostInit();
                                                            }
                                                            else
                                                            {
                                                               obj = default;
                                                            }

                                                            return validationError;
                                                         }

                                                         public static global::Thinktecture.Tests.TestValueObject? Create(string? @value)
                                                         {
                                                            var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject? obj);

                                                            if (validationError is not null)
                                                               throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                            return obj;
                                                         }

                                                         public static bool TryCreate(string? @value, out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return TryCreate(@value, out obj, out _);
                                                         }

                                                         public static bool TryCreate(
                                                            string? @value,
                                                            out global::Thinktecture.Tests.TestValueObject? obj,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                         {
                                                            validationError = Validate(@value, null, out obj);

                                                            return validationError is null;
                                                         }

                                                         static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value);

                                                         partial void FactoryPostInit();

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this._value;
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return obj?._value;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="value">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                         public static explicit operator global::Thinktecture.Tests.TestValueObject?(string? @value)
                                                         {
                                                            if(@value is null)
                                                               return null;

                                                            return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                         }

                                                         private TestValueObject(string @value)
                                                         {
                                                            ValidateConstructorArguments(ref @value);

                                                            this._value = @value;
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string @value);

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            if (other is null)
                                                               return false;

                                                            if (global::System.Object.ReferenceEquals(this, other))
                                                               return true;

                                                            return global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value, other._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return global::System.HashCode.Combine(_typeHashCode, global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(this._value));
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this._value.ToString();
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_string_based_keyed_class_with_additional_member()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<string>]
                   	public partial class TestValueObject
                   	{
                         public readonly string OtherField;
                      }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_CLASS_STRING);
      AssertOutput(parsableOutput, _PARSABLE_CLASS_STRING);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_CLASS_STRING);
      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_CLASS);
      AssertOutput(mainOutput, _MAIN_OUTPUT_STRING_BASED_CLASS);
   }

   [Fact]
   public void Should_generate_string_based_keyed_class_with_custom_comparers()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<string>]
                     [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
                     [ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
                   	public partial class TestValueObject
                   	{
                   	}
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(5);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var comparableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Comparable.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;
      var comparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(comparableOutput, _COMPARABLE_CLASS_STRING_WITH_ORDINAL_COMPARER);
      AssertOutput(parsableOutput, _PARSABLE_CLASS_STRING);
      AssertOutput(comparisonOperatorsOutput, _COMPARISON_OPERATORS_STRING_WITH_ORDINAL_COMPARER);
      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>))]
                                                      sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                         global::Thinktecture.IKeyedValueObject<string>,
                                                         global::Thinktecture.IValueObjectConvertable<string>,
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            global::System.Func<string, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<string, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, string>(static item => item._value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, string>> convertToKeyExpression = static obj => obj._value;

                                                            var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(string), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                         }

                                                         private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                         /// <summary>
                                                         /// The identifier of this object.
                                                         /// </summary>
                                                         private readonly string _value;

                                                         public static global::Thinktecture.ValidationError? Validate(string? @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            if(@value is null)
                                                            {
                                                               obj = default;
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>("The argument 'value' must not be null.");
                                                            }

                                                            global::Thinktecture.ValidationError? validationError = null;
                                                            ValidateFactoryArguments(ref validationError, ref @value);

                                                            if (validationError is null)
                                                            {
                                                               obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                               obj.FactoryPostInit();
                                                            }
                                                            else
                                                            {
                                                               obj = default;
                                                            }

                                                            return validationError;
                                                         }

                                                         public static global::Thinktecture.Tests.TestValueObject Create(string @value)
                                                         {
                                                            var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject? obj);

                                                            if (validationError is not null)
                                                               throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                            return obj!;
                                                         }

                                                         public static bool TryCreate(string @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return TryCreate(@value, out obj, out _);
                                                         }

                                                         public static bool TryCreate(
                                                            string @value,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                         {
                                                            validationError = Validate(@value, null, out obj);

                                                            return validationError is null;
                                                         }

                                                         static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value);

                                                         partial void FactoryPostInit();

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
                                                         {
                                                            return this._value;
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                         public static implicit operator string?(global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return obj?._value;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="string"/>.
                                                         /// </summary>
                                                         /// <param name="value">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("value")]
                                                         public static explicit operator global::Thinktecture.Tests.TestValueObject?(string? @value)
                                                         {
                                                            if(@value is null)
                                                               return null;

                                                            return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                         }

                                                         private TestValueObject(string @value)
                                                         {
                                                            ValidateConstructorArguments(ref @value);

                                                            this._value = @value;
                                                         }

                                                         static partial void ValidateConstructorArguments(ref string @value);

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            if (other is null)
                                                               return false;

                                                            if (global::System.Object.ReferenceEquals(this, other))
                                                               return true;

                                                            return global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer.Equals(this._value, other._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return global::System.HashCode.Combine(_typeHashCode, global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer.GetHashCode(this._value));
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this._value.ToString();
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_keyed_class_with_non_IEquatable_key_member_but_with_custom_equality_comparer()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<Foo>]
                     [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.Default<Foo>, Foo>]
                   	public partial class TestValueObject
                   	{
                     }

                     public class Foo
                     {
                     }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(2);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var equalityComparisonOperatorsOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs")).Value;

      AssertOutput(equalityComparisonOperatorsOutput, _EQUALITY_COMPARISON_OPERATORS_CLASS);

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.Foo, global::Thinktecture.ValidationError>))]
                                                      sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                         global::Thinktecture.IKeyedValueObject<global::Thinktecture.Tests.Foo>,
                                                         global::Thinktecture.IValueObjectConvertable<global::Thinktecture.Tests.Foo>,
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.Foo, global::Thinktecture.ValidationError>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            global::System.Func<global::Thinktecture.Tests.Foo, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.Foo, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.Foo, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

                                                            var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.Foo>(static item => item._value);
                                                            global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.Foo>> convertToKeyExpression = static obj => obj._value;

                                                            var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                            var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(global::Thinktecture.Tests.Foo), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

                                                            global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                         }

                                                         private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                         /// <summary>
                                                         /// The identifier of this object.
                                                         /// </summary>
                                                         private readonly global::Thinktecture.Tests.Foo _value;

                                                         public static global::Thinktecture.ValidationError? Validate(global::Thinktecture.Tests.Foo? @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            if(@value is null)
                                                            {
                                                               obj = default;
                                                               return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>("The argument 'value' must not be null.");
                                                            }

                                                            global::Thinktecture.ValidationError? validationError = null;
                                                            ValidateFactoryArguments(ref validationError, ref @value);

                                                            if (validationError is null)
                                                            {
                                                               obj = new global::Thinktecture.Tests.TestValueObject(@value);
                                                               obj.FactoryPostInit();
                                                            }
                                                            else
                                                            {
                                                               obj = default;
                                                            }

                                                            return validationError;
                                                         }

                                                         public static global::Thinktecture.Tests.TestValueObject Create(global::Thinktecture.Tests.Foo @value)
                                                         {
                                                            var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject? obj);

                                                            if (validationError is not null)
                                                               throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                            return obj!;
                                                         }

                                                         public static bool TryCreate(global::Thinktecture.Tests.Foo @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return TryCreate(@value, out obj, out _);
                                                         }

                                                         public static bool TryCreate(
                                                            global::Thinktecture.Tests.Foo @value,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                         {
                                                            validationError = Validate(@value, null, out obj);

                                                            return validationError is null;
                                                         }

                                                         static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref global::Thinktecture.Tests.Foo @value);

                                                         partial void FactoryPostInit();

                                                         /// <summary>
                                                         /// Gets the identifier of the item.
                                                         /// </summary>
                                                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                                                         global::Thinktecture.Tests.Foo global::Thinktecture.IValueObjectConvertable<global::Thinktecture.Tests.Foo>.ToValue()
                                                         {
                                                            return this._value;
                                                         }

                                                         /// <summary>
                                                         /// Implicit conversion to the type <see cref="global::Thinktecture.Tests.Foo"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Object to covert.</param>
                                                         /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
                                                         public static implicit operator global::Thinktecture.Tests.Foo?(global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return obj?._value;
                                                         }

                                                         /// <summary>
                                                         /// Explicit conversion from the type <see cref="global::Thinktecture.Tests.Foo"/>.
                                                         /// </summary>
                                                         /// <param name="value">Value to covert.</param>
                                                         /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
                                                         [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("value")]
                                                         public static explicit operator global::Thinktecture.Tests.TestValueObject?(global::Thinktecture.Tests.Foo? @value)
                                                         {
                                                            if(@value is null)
                                                               return null;

                                                            return global::Thinktecture.Tests.TestValueObject.Create(@value);
                                                         }

                                                         private TestValueObject(global::Thinktecture.Tests.Foo @value)
                                                         {
                                                            ValidateConstructorArguments(ref @value);

                                                            this._value = @value;
                                                         }

                                                         static partial void ValidateConstructorArguments(ref global::Thinktecture.Tests.Foo @value);

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            if (other is null)
                                                               return false;

                                                            if (global::System.Object.ReferenceEquals(this, other))
                                                               return true;

                                                            return global::Thinktecture.ComparerAccessors.Default<global::Thinktecture.Tests.Foo>.EqualityComparer.Equals(this._value, other._value);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            return global::System.HashCode.Combine(_typeHashCode, global::Thinktecture.ComparerAccessors.Default<global::Thinktecture.Tests.Foo>.EqualityComparer.GetHashCode(this._value));
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return this._value.ToString();
                                                         }
                                                      }
                                                   }

                                                   """);
   }

   [Fact]
   public void Should_generate_complex_class_with_8_members()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject]
                   	public partial class TestValueObject
                   	{
                         [ValueObjectMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
                         public readonly string _stringValue;

                         [ValueObjectMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
                         public readonly int _intValue;

                         public string ReferenceProperty { get; }
                         public string? NullableReferenceProperty { get; }
                         public int StructProperty { get; }
                         public int? NullableStructProperty { get; }

                         public int ExpressionBodyProperty => 42;

                         public int GetterExpressionProperty
                         {
                            get => 42;
                         }

                         public int GetterBodyProperty
                         {
                            get { return 42; }
                         }

                         public int SetterProperty
                         {
                            set { }
                         }
                      }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      AssertOutput(output, _GENERATED_HEADER + """

                                               namespace Thinktecture.Tests
                                               {
                                                  sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                     global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                     global::Thinktecture.IComplexValueObject
                                                  {
                                                     [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                     internal static void ModuleInit()
                                                     {
                                                        global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                                                                           {
                                                                                                                                                              o._stringValue,
                                                                                                                                                              o._intValue,
                                                                                                                                                              o.ReferenceProperty,
                                                                                                                                                              o.NullableReferenceProperty,
                                                                                                                                                              o.StructProperty,
                                                                                                                                                              o.NullableStructProperty
                                                                                                                                                           };

                                                        var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

                                                        foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
                                                        {
                                                           members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
                                                        }

                                                        var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                        var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

                                                        global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                     }

                                                     private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                     public static global::Thinktecture.ValidationError? Validate(
                                                        string @stringValue,
                                                        int @intValue,
                                                        string @referenceProperty,
                                                        string? @nullableReferenceProperty,
                                                        int @structProperty,
                                                        int? @nullableStructProperty,
                                                        out global::Thinktecture.Tests.TestValueObject? obj)
                                                     {
                                                        global::Thinktecture.ValidationError? validationError = null;
                                                        ValidateFactoryArguments(
                                                           ref validationError,
                                                           ref @stringValue,
                                                           ref @intValue,
                                                           ref @referenceProperty,
                                                           ref @nullableReferenceProperty,
                                                           ref @structProperty,
                                                           ref @nullableStructProperty);

                                                        if (validationError is null)
                                                        {
                                                           obj = new global::Thinktecture.Tests.TestValueObject(
                                                              @stringValue,
                                                              @intValue,
                                                              @referenceProperty,
                                                              @nullableReferenceProperty,
                                                              @structProperty,
                                                              @nullableStructProperty);
                                                           obj.FactoryPostInit();
                                                        }
                                                        else
                                                        {
                                                           obj = default;
                                                        }

                                                        return validationError;
                                                     }

                                                     public static global::Thinktecture.Tests.TestValueObject Create(
                                                        string @stringValue,
                                                        int @intValue,
                                                        string @referenceProperty,
                                                        string? @nullableReferenceProperty,
                                                        int @structProperty,
                                                        int? @nullableStructProperty)
                                                     {
                                                        var validationError = Validate(
                                                           @stringValue,
                                                           @intValue,
                                                           @referenceProperty,
                                                           @nullableReferenceProperty,
                                                           @structProperty,
                                                           @nullableStructProperty,
                                                           out global::Thinktecture.Tests.TestValueObject? obj);

                                                        if (validationError is not null)
                                                           throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                        return obj!;
                                                     }

                                                     public static bool TryCreate(
                                                        string @stringValue,
                                                        int @intValue,
                                                        string @referenceProperty,
                                                        string? @nullableReferenceProperty,
                                                        int @structProperty,
                                                        int? @nullableStructProperty,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                     {
                                                        return TryCreate(
                                                           @stringValue,
                                                           @intValue,
                                                           @referenceProperty,
                                                           @nullableReferenceProperty,
                                                           @structProperty,
                                                           @nullableStructProperty,
                                                           out obj,
                                                           out _);
                                                     }

                                                     public static bool TryCreate(
                                                        string @stringValue,
                                                        int @intValue,
                                                        string @referenceProperty,
                                                        string? @nullableReferenceProperty,
                                                        int @structProperty,
                                                        int? @nullableStructProperty,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                     {
                                                        validationError = Validate(
                                                           @stringValue,
                                                           @intValue,
                                                           @referenceProperty,
                                                           @nullableReferenceProperty,
                                                           @structProperty,
                                                           @nullableStructProperty,
                                                           out obj);

                                                        return validationError is null;
                                                     }

                                                     static partial void ValidateFactoryArguments(
                                                        ref global::Thinktecture.ValidationError? validationError,
                                                        [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @stringValue,
                                                        ref int @intValue,
                                                        [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @referenceProperty,
                                                        ref string? @nullableReferenceProperty,
                                                        ref int @structProperty,
                                                        ref int? @nullableStructProperty);

                                                     partial void FactoryPostInit();

                                                     private TestValueObject(
                                                        string @stringValue,
                                                        int @intValue,
                                                        string @referenceProperty,
                                                        string? @nullableReferenceProperty,
                                                        int @structProperty,
                                                        int? @nullableStructProperty)
                                                     {
                                                        ValidateConstructorArguments(
                                                           ref @stringValue,
                                                           ref @intValue,
                                                           ref @referenceProperty,
                                                           ref @nullableReferenceProperty,
                                                           ref @structProperty,
                                                           ref @nullableStructProperty);

                                                        this._stringValue = @stringValue;
                                                        this._intValue = @intValue;
                                                        this.ReferenceProperty = @referenceProperty;
                                                        this.NullableReferenceProperty = @nullableReferenceProperty;
                                                        this.StructProperty = @structProperty;
                                                        this.NullableStructProperty = @nullableStructProperty;
                                                     }

                                                     static partial void ValidateConstructorArguments(
                                                        ref string @stringValue,
                                                        ref int @intValue,
                                                        ref string @referenceProperty,
                                                        ref string? @nullableReferenceProperty,
                                                        ref int @structProperty,
                                                        ref int? @nullableStructProperty);

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                     public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        if (obj is null)
                                                           return other is null;

                                                        return obj.Equals(other);
                                                     }

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                     public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        return !(obj == other);
                                                     }

                                                     /// <inheritdoc />
                                                     public override bool Equals(object? other)
                                                     {
                                                        return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                     }

                                                     /// <inheritdoc />
                                                     public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        if (other is null)
                                                           return false;

                                                        if (global::System.Object.ReferenceEquals(this, other))
                                                           return true;

                                                        return global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer.Equals(this._stringValue, other._stringValue)
                                                            && global::Thinktecture.ComparerAccessors.Default<int>.EqualityComparer.Equals(this._intValue, other._intValue);
                                                     }

                                                     /// <inheritdoc />
                                                     public override int GetHashCode()
                                                     {
                                                        var hashCode = new global::System.HashCode();
                                                        hashCode.Add(_typeHashCode);
                                                        hashCode.Add(this._stringValue, global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer);
                                                        hashCode.Add(this._intValue, global::Thinktecture.ComparerAccessors.Default<int>.EqualityComparer);
                                                        return hashCode.ToHashCode();
                                                     }

                                                     /// <inheritdoc />
                                                     public override string ToString()
                                                     {
                                                        return $"{{ _stringValue = {this._stringValue}, _intValue = {this._intValue} }}";
                                                     }
                                                  }
                                               }

                                               """);
   }

   [Fact]
   public void Should_generate_complex_class_with_8_members_and_ValueObjectFactoryAttribute()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject]
                     [ValueObjectFactory<string>]
                   	public partial class TestValueObject
                   	{
                         [ValueObjectMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
                         public readonly string _stringValue;

                         [ValueObjectMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
                         public readonly int _intValue;

                         public string ReferenceProperty { get; }
                         public string? NullableReferenceProperty { get; }
                         public int StructProperty { get; }
                         public int? NullableStructProperty { get; }

                         public int ExpressionBodyProperty => 42;

                         public int GetterExpressionProperty
                         {
                            get => 42;
                         }

                         public int GetterBodyProperty
                         {
                            get { return 42; }
                         }

                         public int SetterProperty
                         {
                            set { }
                         }
                      }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(2);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                         global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                         global::Thinktecture.IComplexValueObject,
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                                                                               {
                                                                                                                                                                  o._stringValue,
                                                                                                                                                                  o._intValue,
                                                                                                                                                                  o.ReferenceProperty,
                                                                                                                                                                  o.NullableReferenceProperty,
                                                                                                                                                                  o.StructProperty,
                                                                                                                                                                  o.NullableStructProperty
                                                                                                                                                               };

                                                            var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

                                                            foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
                                                            {
                                                               members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
                                                            }

                                                            var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                            var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

                                                            global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                         }

                                                         private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                         public static global::Thinktecture.ValidationError? Validate(
                                                            string @stringValue,
                                                            int @intValue,
                                                            string @referenceProperty,
                                                            string? @nullableReferenceProperty,
                                                            int @structProperty,
                                                            int? @nullableStructProperty,
                                                            out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            global::Thinktecture.ValidationError? validationError = null;
                                                            ValidateFactoryArguments(
                                                               ref validationError,
                                                               ref @stringValue,
                                                               ref @intValue,
                                                               ref @referenceProperty,
                                                               ref @nullableReferenceProperty,
                                                               ref @structProperty,
                                                               ref @nullableStructProperty);

                                                            if (validationError is null)
                                                            {
                                                               obj = new global::Thinktecture.Tests.TestValueObject(
                                                                  @stringValue,
                                                                  @intValue,
                                                                  @referenceProperty,
                                                                  @nullableReferenceProperty,
                                                                  @structProperty,
                                                                  @nullableStructProperty);
                                                               obj.FactoryPostInit();
                                                            }
                                                            else
                                                            {
                                                               obj = default;
                                                            }

                                                            return validationError;
                                                         }

                                                         public static global::Thinktecture.Tests.TestValueObject Create(
                                                            string @stringValue,
                                                            int @intValue,
                                                            string @referenceProperty,
                                                            string? @nullableReferenceProperty,
                                                            int @structProperty,
                                                            int? @nullableStructProperty)
                                                         {
                                                            var validationError = Validate(
                                                               @stringValue,
                                                               @intValue,
                                                               @referenceProperty,
                                                               @nullableReferenceProperty,
                                                               @structProperty,
                                                               @nullableStructProperty,
                                                               out global::Thinktecture.Tests.TestValueObject? obj);

                                                            if (validationError is not null)
                                                               throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                            return obj!;
                                                         }

                                                         public static bool TryCreate(
                                                            string @stringValue,
                                                            int @intValue,
                                                            string @referenceProperty,
                                                            string? @nullableReferenceProperty,
                                                            int @structProperty,
                                                            int? @nullableStructProperty,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return TryCreate(
                                                               @stringValue,
                                                               @intValue,
                                                               @referenceProperty,
                                                               @nullableReferenceProperty,
                                                               @structProperty,
                                                               @nullableStructProperty,
                                                               out obj,
                                                               out _);
                                                         }

                                                         public static bool TryCreate(
                                                            string @stringValue,
                                                            int @intValue,
                                                            string @referenceProperty,
                                                            string? @nullableReferenceProperty,
                                                            int @structProperty,
                                                            int? @nullableStructProperty,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                         {
                                                            validationError = Validate(
                                                               @stringValue,
                                                               @intValue,
                                                               @referenceProperty,
                                                               @nullableReferenceProperty,
                                                               @structProperty,
                                                               @nullableStructProperty,
                                                               out obj);

                                                            return validationError is null;
                                                         }

                                                         static partial void ValidateFactoryArguments(
                                                            ref global::Thinktecture.ValidationError? validationError,
                                                            [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @stringValue,
                                                            ref int @intValue,
                                                            [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @referenceProperty,
                                                            ref string? @nullableReferenceProperty,
                                                            ref int @structProperty,
                                                            ref int? @nullableStructProperty);

                                                         partial void FactoryPostInit();

                                                         private TestValueObject(
                                                            string @stringValue,
                                                            int @intValue,
                                                            string @referenceProperty,
                                                            string? @nullableReferenceProperty,
                                                            int @structProperty,
                                                            int? @nullableStructProperty)
                                                         {
                                                            ValidateConstructorArguments(
                                                               ref @stringValue,
                                                               ref @intValue,
                                                               ref @referenceProperty,
                                                               ref @nullableReferenceProperty,
                                                               ref @structProperty,
                                                               ref @nullableStructProperty);

                                                            this._stringValue = @stringValue;
                                                            this._intValue = @intValue;
                                                            this.ReferenceProperty = @referenceProperty;
                                                            this.NullableReferenceProperty = @nullableReferenceProperty;
                                                            this.StructProperty = @structProperty;
                                                            this.NullableStructProperty = @nullableStructProperty;
                                                         }

                                                         static partial void ValidateConstructorArguments(
                                                            ref string @stringValue,
                                                            ref int @intValue,
                                                            ref string @referenceProperty,
                                                            ref string? @nullableReferenceProperty,
                                                            ref int @structProperty,
                                                            ref int? @nullableStructProperty);

                                                         /// <summary>
                                                         /// Compares two instances of <see cref="TestValueObject"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Instance to compare.</param>
                                                         /// <param name="other">Another instance to compare.</param>
                                                         /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                         public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            if (obj is null)
                                                               return other is null;

                                                            return obj.Equals(other);
                                                         }

                                                         /// <summary>
                                                         /// Compares two instances of <see cref="TestValueObject"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Instance to compare.</param>
                                                         /// <param name="other">Another instance to compare.</param>
                                                         /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                         public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            return !(obj == other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            if (other is null)
                                                               return false;

                                                            if (global::System.Object.ReferenceEquals(this, other))
                                                               return true;

                                                            return global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer.Equals(this._stringValue, other._stringValue)
                                                                && global::Thinktecture.ComparerAccessors.Default<int>.EqualityComparer.Equals(this._intValue, other._intValue);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            var hashCode = new global::System.HashCode();
                                                            hashCode.Add(_typeHashCode);
                                                            hashCode.Add(this._stringValue, global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer);
                                                            hashCode.Add(this._intValue, global::Thinktecture.ComparerAccessors.Default<int>.EqualityComparer);
                                                            return hashCode.ToHashCode();
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return $"{{ _stringValue = {this._stringValue}, _intValue = {this._intValue} }}";
                                                         }
                                                      }
                                                   }

                                                   """);

      AssertOutput(parsableOutput, _GENERATED_HEADER + """

                                                       namespace Thinktecture.Tests;

                                                       partial class TestValueObject :
                                                          global::System.IParsable<global::Thinktecture.Tests.TestValueObject>
                                                       {
                                                          private static global::Thinktecture.ValidationError? Validate<T>(string key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestValueObject? result)
                                                             where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>
                                                          {
                                                             return T.Validate(key, provider, out result);
                                                          }

                                                          /// <inheritdoc />
                                                          public static global::Thinktecture.Tests.TestValueObject Parse(string s, global::System.IFormatProvider? provider)
                                                          {
                                                             var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(s, provider, out var result);

                                                             if(validationError is null)
                                                                return result!;

                                                             throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestValueObject\".");
                                                          }

                                                          /// <inheritdoc />
                                                          public static bool TryParse(
                                                             string? s,
                                                             global::System.IFormatProvider? provider,
                                                             [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestValueObject result)
                                                          {
                                                             if(s is null)
                                                             {
                                                                result = default;
                                                                return false;
                                                             }

                                                             var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(s, provider, out result!);
                                                             return validationError is null;
                                                          }
                                                       }

                                                       """);
   }

   [Fact]
   public void Should_generate_complex_class_with_8_members_and_ValueObjectFactoryAttribute_and_UseForSerialization()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject]
                     [ValueObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
                   	public partial class TestValueObject
                   	{
                         [ValueObjectMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
                         public readonly string _stringValue;

                         [ValueObjectMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
                         public readonly int _intValue;

                         public string ReferenceProperty { get; }
                         public string? NullableReferenceProperty { get; }
                         public int StructProperty { get; }
                         public int? NullableStructProperty { get; }

                         public int ExpressionBodyProperty => 42;

                         public int GetterExpressionProperty
                         {
                            get => 42;
                         }

                         public int GetterBodyProperty
                         {
                            get { return 42; }
                         }

                         public int SetterProperty
                         {
                            set { }
                         }
                      }
                   }

                   """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      outputs.Should().HaveCount(2);

      var mainOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.g.cs")).Value;
      var parsableOutput = outputs.Single(kvp => kvp.Key.Contains("Thinktecture.Tests.TestValueObject.Parsable.g.cs")).Value;

      AssertOutput(mainOutput, _GENERATED_HEADER + """

                                                   namespace Thinktecture.Tests
                                                   {
                                                      sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                         global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                         global::Thinktecture.IComplexValueObject,
                                                         global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>,
                                                         global::Thinktecture.IValueObjectConvertable<string>
                                                      {
                                                         [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                         internal static void ModuleInit()
                                                         {
                                                            global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                                                                               {
                                                                                                                                                                  o._stringValue,
                                                                                                                                                                  o._intValue,
                                                                                                                                                                  o.ReferenceProperty,
                                                                                                                                                                  o.NullableReferenceProperty,
                                                                                                                                                                  o.StructProperty,
                                                                                                                                                                  o.NullableStructProperty
                                                                                                                                                               };

                                                            var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

                                                            foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
                                                            {
                                                               members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
                                                            }

                                                            var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                            var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

                                                            global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                         }

                                                         private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                         public static global::Thinktecture.ValidationError? Validate(
                                                            string @stringValue,
                                                            int @intValue,
                                                            string @referenceProperty,
                                                            string? @nullableReferenceProperty,
                                                            int @structProperty,
                                                            int? @nullableStructProperty,
                                                            out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            global::Thinktecture.ValidationError? validationError = null;
                                                            ValidateFactoryArguments(
                                                               ref validationError,
                                                               ref @stringValue,
                                                               ref @intValue,
                                                               ref @referenceProperty,
                                                               ref @nullableReferenceProperty,
                                                               ref @structProperty,
                                                               ref @nullableStructProperty);

                                                            if (validationError is null)
                                                            {
                                                               obj = new global::Thinktecture.Tests.TestValueObject(
                                                                  @stringValue,
                                                                  @intValue,
                                                                  @referenceProperty,
                                                                  @nullableReferenceProperty,
                                                                  @structProperty,
                                                                  @nullableStructProperty);
                                                               obj.FactoryPostInit();
                                                            }
                                                            else
                                                            {
                                                               obj = default;
                                                            }

                                                            return validationError;
                                                         }

                                                         public static global::Thinktecture.Tests.TestValueObject Create(
                                                            string @stringValue,
                                                            int @intValue,
                                                            string @referenceProperty,
                                                            string? @nullableReferenceProperty,
                                                            int @structProperty,
                                                            int? @nullableStructProperty)
                                                         {
                                                            var validationError = Validate(
                                                               @stringValue,
                                                               @intValue,
                                                               @referenceProperty,
                                                               @nullableReferenceProperty,
                                                               @structProperty,
                                                               @nullableStructProperty,
                                                               out global::Thinktecture.Tests.TestValueObject? obj);

                                                            if (validationError is not null)
                                                               throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                            return obj!;
                                                         }

                                                         public static bool TryCreate(
                                                            string @stringValue,
                                                            int @intValue,
                                                            string @referenceProperty,
                                                            string? @nullableReferenceProperty,
                                                            int @structProperty,
                                                            int? @nullableStructProperty,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                         {
                                                            return TryCreate(
                                                               @stringValue,
                                                               @intValue,
                                                               @referenceProperty,
                                                               @nullableReferenceProperty,
                                                               @structProperty,
                                                               @nullableStructProperty,
                                                               out obj,
                                                               out _);
                                                         }

                                                         public static bool TryCreate(
                                                            string @stringValue,
                                                            int @intValue,
                                                            string @referenceProperty,
                                                            string? @nullableReferenceProperty,
                                                            int @structProperty,
                                                            int? @nullableStructProperty,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                         {
                                                            validationError = Validate(
                                                               @stringValue,
                                                               @intValue,
                                                               @referenceProperty,
                                                               @nullableReferenceProperty,
                                                               @structProperty,
                                                               @nullableStructProperty,
                                                               out obj);

                                                            return validationError is null;
                                                         }

                                                         static partial void ValidateFactoryArguments(
                                                            ref global::Thinktecture.ValidationError? validationError,
                                                            [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @stringValue,
                                                            ref int @intValue,
                                                            [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @referenceProperty,
                                                            ref string? @nullableReferenceProperty,
                                                            ref int @structProperty,
                                                            ref int? @nullableStructProperty);

                                                         partial void FactoryPostInit();

                                                         private TestValueObject(
                                                            string @stringValue,
                                                            int @intValue,
                                                            string @referenceProperty,
                                                            string? @nullableReferenceProperty,
                                                            int @structProperty,
                                                            int? @nullableStructProperty)
                                                         {
                                                            ValidateConstructorArguments(
                                                               ref @stringValue,
                                                               ref @intValue,
                                                               ref @referenceProperty,
                                                               ref @nullableReferenceProperty,
                                                               ref @structProperty,
                                                               ref @nullableStructProperty);

                                                            this._stringValue = @stringValue;
                                                            this._intValue = @intValue;
                                                            this.ReferenceProperty = @referenceProperty;
                                                            this.NullableReferenceProperty = @nullableReferenceProperty;
                                                            this.StructProperty = @structProperty;
                                                            this.NullableStructProperty = @nullableStructProperty;
                                                         }

                                                         static partial void ValidateConstructorArguments(
                                                            ref string @stringValue,
                                                            ref int @intValue,
                                                            ref string @referenceProperty,
                                                            ref string? @nullableReferenceProperty,
                                                            ref int @structProperty,
                                                            ref int? @nullableStructProperty);

                                                         /// <summary>
                                                         /// Compares two instances of <see cref="TestValueObject"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Instance to compare.</param>
                                                         /// <param name="other">Another instance to compare.</param>
                                                         /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                         public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            if (obj is null)
                                                               return other is null;

                                                            return obj.Equals(other);
                                                         }

                                                         /// <summary>
                                                         /// Compares two instances of <see cref="TestValueObject"/>.
                                                         /// </summary>
                                                         /// <param name="obj">Instance to compare.</param>
                                                         /// <param name="other">Another instance to compare.</param>
                                                         /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                         public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            return !(obj == other);
                                                         }

                                                         /// <inheritdoc />
                                                         public override bool Equals(object? other)
                                                         {
                                                            return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                         }

                                                         /// <inheritdoc />
                                                         public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                         {
                                                            if (other is null)
                                                               return false;

                                                            if (global::System.Object.ReferenceEquals(this, other))
                                                               return true;

                                                            return global::Thinktecture.ComparerAccessors.StringOrdinalIgnoreCase.EqualityComparer.Equals(this._stringValue, other._stringValue)
                                                                && global::Thinktecture.ComparerAccessors.Default<int>.EqualityComparer.Equals(this._intValue, other._intValue);
                                                         }

                                                         /// <inheritdoc />
                                                         public override int GetHashCode()
                                                         {
                                                            var hashCode = new global::System.HashCode();
                                                            hashCode.Add(_typeHashCode);
                                                            hashCode.Add(this._stringValue, global::Thinktecture.ComparerAccessors.StringOrdinalIgnoreCase.EqualityComparer);
                                                            hashCode.Add(this._intValue, global::Thinktecture.ComparerAccessors.Default<int>.EqualityComparer);
                                                            return hashCode.ToHashCode();
                                                         }

                                                         /// <inheritdoc />
                                                         public override string ToString()
                                                         {
                                                            return $"{{ _stringValue = {this._stringValue}, _intValue = {this._intValue} }}";
                                                         }
                                                      }
                                                   }

                                                   """);

      AssertOutput(parsableOutput, _GENERATED_HEADER + """

                                                       namespace Thinktecture.Tests;

                                                       partial class TestValueObject :
                                                          global::System.IParsable<global::Thinktecture.Tests.TestValueObject>
                                                       {
                                                          private static global::Thinktecture.ValidationError? Validate<T>(string key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestValueObject? result)
                                                             where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>
                                                          {
                                                             return T.Validate(key, provider, out result);
                                                          }

                                                          /// <inheritdoc />
                                                          public static global::Thinktecture.Tests.TestValueObject Parse(string s, global::System.IFormatProvider? provider)
                                                          {
                                                             var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(s, provider, out var result);

                                                             if(validationError is null)
                                                                return result!;

                                                             throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestValueObject\".");
                                                          }

                                                          /// <inheritdoc />
                                                          public static bool TryParse(
                                                             string? s,
                                                             global::System.IFormatProvider? provider,
                                                             [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestValueObject result)
                                                          {
                                                             if(s is null)
                                                             {
                                                                result = default;
                                                                return false;
                                                             }

                                                             var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(s, provider, out result!);
                                                             return validationError is null;
                                                          }
                                                       }

                                                       """);
   }

   [Fact]
   public void Should_generate_complex_class_with_9_members()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                      [ComplexValueObject]
                   	public partial class TestValueObject
                   	{
                         public readonly string _value1;
                         public readonly string _value2;
                         public readonly string _value3;
                         public readonly string _value4;
                         public readonly string _value5;
                         public readonly string _value6;
                         public readonly string _value7;
                         public readonly string _value8;
                         public readonly string _value9;
                      }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      AssertOutput(output, _GENERATED_HEADER + """

                                               namespace Thinktecture.Tests
                                               {
                                                  sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
                                                     global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
                                                     global::Thinktecture.IComplexValueObject
                                                  {
                                                     [global::System.Runtime.CompilerServices.ModuleInitializer]
                                                     internal static void ModuleInit()
                                                     {
                                                        global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                                                                           {
                                                                                                                                                              o._value1,
                                                                                                                                                              o._value2,
                                                                                                                                                              o._value3,
                                                                                                                                                              o._value4,
                                                                                                                                                              o._value5,
                                                                                                                                                              o._value6,
                                                                                                                                                              o._value7,
                                                                                                                                                              o._value8,
                                                                                                                                                              o._value9
                                                                                                                                                           };

                                                        var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

                                                        foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
                                                        {
                                                           members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
                                                        }

                                                        var type = typeof(global::Thinktecture.Tests.TestValueObject);
                                                        var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

                                                        global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
                                                     }

                                                     private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

                                                     public static global::Thinktecture.ValidationError? Validate(
                                                        string @value1,
                                                        string @value2,
                                                        string @value3,
                                                        string @value4,
                                                        string @value5,
                                                        string @value6,
                                                        string @value7,
                                                        string @value8,
                                                        string @value9,
                                                        out global::Thinktecture.Tests.TestValueObject? obj)
                                                     {
                                                        global::Thinktecture.ValidationError? validationError = null;
                                                        ValidateFactoryArguments(
                                                           ref validationError,
                                                           ref @value1,
                                                           ref @value2,
                                                           ref @value3,
                                                           ref @value4,
                                                           ref @value5,
                                                           ref @value6,
                                                           ref @value7,
                                                           ref @value8,
                                                           ref @value9);

                                                        if (validationError is null)
                                                        {
                                                           obj = new global::Thinktecture.Tests.TestValueObject(
                                                              @value1,
                                                              @value2,
                                                              @value3,
                                                              @value4,
                                                              @value5,
                                                              @value6,
                                                              @value7,
                                                              @value8,
                                                              @value9);
                                                           obj.FactoryPostInit();
                                                        }
                                                        else
                                                        {
                                                           obj = default;
                                                        }

                                                        return validationError;
                                                     }

                                                     public static global::Thinktecture.Tests.TestValueObject Create(
                                                        string @value1,
                                                        string @value2,
                                                        string @value3,
                                                        string @value4,
                                                        string @value5,
                                                        string @value6,
                                                        string @value7,
                                                        string @value8,
                                                        string @value9)
                                                     {
                                                        var validationError = Validate(
                                                           @value1,
                                                           @value2,
                                                           @value3,
                                                           @value4,
                                                           @value5,
                                                           @value6,
                                                           @value7,
                                                           @value8,
                                                           @value9,
                                                           out global::Thinktecture.Tests.TestValueObject? obj);

                                                        if (validationError is not null)
                                                           throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

                                                        return obj!;
                                                     }

                                                     public static bool TryCreate(
                                                        string @value1,
                                                        string @value2,
                                                        string @value3,
                                                        string @value4,
                                                        string @value5,
                                                        string @value6,
                                                        string @value7,
                                                        string @value8,
                                                        string @value9,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
                                                     {
                                                        return TryCreate(
                                                           @value1,
                                                           @value2,
                                                           @value3,
                                                           @value4,
                                                           @value5,
                                                           @value6,
                                                           @value7,
                                                           @value8,
                                                           @value9,
                                                           out obj,
                                                           out _);
                                                     }

                                                     public static bool TryCreate(
                                                        string @value1,
                                                        string @value2,
                                                        string @value3,
                                                        string @value4,
                                                        string @value5,
                                                        string @value6,
                                                        string @value7,
                                                        string @value8,
                                                        string @value9,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
                                                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
                                                     {
                                                        validationError = Validate(
                                                           @value1,
                                                           @value2,
                                                           @value3,
                                                           @value4,
                                                           @value5,
                                                           @value6,
                                                           @value7,
                                                           @value8,
                                                           @value9,
                                                           out obj);

                                                        return validationError is null;
                                                     }

                                                     static partial void ValidateFactoryArguments(
                                                        ref global::Thinktecture.ValidationError? validationError,
                                                        [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value1,
                                                        [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value2,
                                                        [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value3,
                                                        [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value4,
                                                        [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value5,
                                                        [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value6,
                                                        [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value7,
                                                        [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value8,
                                                        [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @value9);

                                                     partial void FactoryPostInit();

                                                     private TestValueObject(
                                                        string @value1,
                                                        string @value2,
                                                        string @value3,
                                                        string @value4,
                                                        string @value5,
                                                        string @value6,
                                                        string @value7,
                                                        string @value8,
                                                        string @value9)
                                                     {
                                                        ValidateConstructorArguments(
                                                           ref @value1,
                                                           ref @value2,
                                                           ref @value3,
                                                           ref @value4,
                                                           ref @value5,
                                                           ref @value6,
                                                           ref @value7,
                                                           ref @value8,
                                                           ref @value9);

                                                        this._value1 = @value1;
                                                        this._value2 = @value2;
                                                        this._value3 = @value3;
                                                        this._value4 = @value4;
                                                        this._value5 = @value5;
                                                        this._value6 = @value6;
                                                        this._value7 = @value7;
                                                        this._value8 = @value8;
                                                        this._value9 = @value9;
                                                     }

                                                     static partial void ValidateConstructorArguments(
                                                        ref string @value1,
                                                        ref string @value2,
                                                        ref string @value3,
                                                        ref string @value4,
                                                        ref string @value5,
                                                        ref string @value6,
                                                        ref string @value7,
                                                        ref string @value8,
                                                        ref string @value9);

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
                                                     public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        if (obj is null)
                                                           return other is null;

                                                        return obj.Equals(other);
                                                     }

                                                     /// <summary>
                                                     /// Compares two instances of <see cref="TestValueObject"/>.
                                                     /// </summary>
                                                     /// <param name="obj">Instance to compare.</param>
                                                     /// <param name="other">Another instance to compare.</param>
                                                     /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
                                                     public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        return !(obj == other);
                                                     }

                                                     /// <inheritdoc />
                                                     public override bool Equals(object? other)
                                                     {
                                                        return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
                                                     }

                                                     /// <inheritdoc />
                                                     public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
                                                     {
                                                        if (other is null)
                                                           return false;

                                                        if (global::System.Object.ReferenceEquals(this, other))
                                                           return true;

                                                        return global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value1, other._value1)
                                                            && global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value2, other._value2)
                                                            && global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value3, other._value3)
                                                            && global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value4, other._value4)
                                                            && global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value5, other._value5)
                                                            && global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value6, other._value6)
                                                            && global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value7, other._value7)
                                                            && global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value8, other._value8)
                                                            && global::System.StringComparer.OrdinalIgnoreCase.Equals(this._value9, other._value9);
                                                     }

                                                     /// <inheritdoc />
                                                     public override int GetHashCode()
                                                     {
                                                        var hashCode = new global::System.HashCode();
                                                        hashCode.Add(_typeHashCode);
                                                        hashCode.Add(this._value1, global::System.StringComparer.OrdinalIgnoreCase);
                                                        hashCode.Add(this._value2, global::System.StringComparer.OrdinalIgnoreCase);
                                                        hashCode.Add(this._value3, global::System.StringComparer.OrdinalIgnoreCase);
                                                        hashCode.Add(this._value4, global::System.StringComparer.OrdinalIgnoreCase);
                                                        hashCode.Add(this._value5, global::System.StringComparer.OrdinalIgnoreCase);
                                                        hashCode.Add(this._value6, global::System.StringComparer.OrdinalIgnoreCase);
                                                        hashCode.Add(this._value7, global::System.StringComparer.OrdinalIgnoreCase);
                                                        hashCode.Add(this._value8, global::System.StringComparer.OrdinalIgnoreCase);
                                                        hashCode.Add(this._value9, global::System.StringComparer.OrdinalIgnoreCase);
                                                        return hashCode.ToHashCode();
                                                     }

                                                     /// <inheritdoc />
                                                     public override string ToString()
                                                     {
                                                        return $"{{ _value1 = {this._value1}, _value2 = {this._value2}, _value3 = {this._value3}, _value4 = {this._value4}, _value5 = {this._value5}, _value6 = {this._value6}, _value7 = {this._value7}, _value8 = {this._value8}, _value9 = {this._value9} }}";
                                                     }
                                                  }
                                               }

                                               """);
   }
}
