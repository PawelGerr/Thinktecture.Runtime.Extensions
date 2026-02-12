namespace Thinktecture;

/// <summary>
/// Base class for <see cref="ValueObjectAttribute{TKey}"/> and <see cref="ComplexValueObjectAttribute"/>.
/// </summary>
public abstract class ValueObjectAttributeBase : Attribute
{
   /// <summary>
   /// Indication whether the methods "Create", "Validate" and "TryCreate" should be generated or not.
   /// </summary>
   /// <remarks>
   /// Setting this property to <c>true</c> has the following implications beyond skipping factory methods:
   /// <list type="bullet">
   /// <item><description>No <c>TypeConverter</c> attribute is emitted.</description></item>
   /// <item><description>The <c>IObjectFactory&lt;T&gt;</c> interface is not implemented.</description></item>
   /// <item><description>The conversion operator from key type to value object is not generated (it relies on the factory method internally).</description></item>
   /// <item><description><see cref="ValueObjectAttribute{TKey}.SkipIParsable"/> is forced to <c>true</c>.</description></item>
   /// <item><description><see cref="ValueObjectAttribute{TKey}.SkipISpanParsable"/> is forced to <c>true</c>.</description></item>
   /// <item><description>Arithmetic operators (<see cref="ValueObjectAttribute{TKey}.AdditionOperators"/>, <see cref="ValueObjectAttribute{TKey}.SubtractionOperators"/>, <see cref="ValueObjectAttribute{TKey}.MultiplyOperators"/>, <see cref="ValueObjectAttribute{TKey}.DivisionOperators"/>) are forced to <c>None</c>.</description></item>
   /// <item><description>No serialization converters (System.Text.Json, Newtonsoft.Json, MessagePack) are generated — unless an <see cref="ObjectFactoryAttribute{T}"/> with <see cref="ObjectFactoryAttribute.UseForSerialization"/> is present.</description></item>
   /// </list>
   /// </remarks>
   public bool SkipFactoryMethods { get; set; }

   /// <summary>
   /// Access modifier of the constructor.
   /// Default is <see cref="AccessModifier.Private"/>.
   /// </summary>
   public AccessModifier ConstructorAccessModifier { get; set; }

   /// <summary>
   /// The name of the factory method "Create".
   /// </summary>
   public string CreateFactoryMethodName
   {
      get => field ?? "Create";
      set => field = String.IsNullOrWhiteSpace(value) ? null : value.Trim();
   }

   /// <summary>
   /// The name of the factory method "TryCreate".
   /// </summary>
   public string TryCreateFactoryMethodName
   {
      get => field ?? "TryCreate";
      set => field = String.IsNullOrWhiteSpace(value) ? null : value.Trim();
   }

   /// <summary>
   /// Indication whether the generator should skip the implementation of the method <see cref="object.ToString"/> or not.
   /// </summary>
   public bool SkipToString { get; set; }

   /// <summary>
   /// By default, explicit creation of a struct value objects using keyword <c>default</c> or via parameterless constructor is disallowed.
   /// Set this property to <c>true</c> to allow creation of value objects with default values.
   ///
   /// This setting has no effect if:
   /// - value object is not a struct
   /// </summary>
   public bool AllowDefaultStructs { get; set; }

   /// <summary>
   /// The name of the static property containing the <c>default</c> instance of the struct.
   /// Default name is "Empty" (analogous to <c>Guid.Empty</c>).
   ///
   /// This setting has no effect on:
   /// - value objects that are classes
   /// </summary>
   public string DefaultInstancePropertyName
   {
      get => field ?? "Empty";
      set;
   }

   /// <summary>
   /// Specifies the serialization frameworks to be generated.
   /// Default is <see cref="SerializationFrameworks.All"/>.
   /// </summary>
   public SerializationFrameworks SerializationFrameworks { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <c>IEquatable{T}</c> and any comparison operators.
   /// This includes the <c>Equals</c> and <c>GetHashCode</c> methods.
   /// </summary>
   public bool SkipEqualityComparison { get; set; }

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectAttributeBase"/>.
   /// </summary>
   private protected ValueObjectAttributeBase()
   {
      ConstructorAccessModifier = AccessModifier.Private;
      SerializationFrameworks = SerializationFrameworks.All;
   }
}
