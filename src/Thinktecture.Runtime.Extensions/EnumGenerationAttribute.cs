using System.Numerics;

namespace Thinktecture;

/// <summary>
/// Settings to be used by enum source generator.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class EnumGenerationAttribute : Attribute
{
   private string? _keyPropertyName;

   /// <summary>
   /// The name of the key property. Default name is 'Key'.
   /// </summary>
   public string KeyPropertyName
   {
      get => _keyPropertyName ?? "Key";
      set => _keyPropertyName = value;
   }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IComparable{T}"/> or not.
   ///
   /// This setting has no effect if:
   /// - the key is not <see cref="IComparable{T}"/> itself.
   /// </summary>
   public bool SkipIComparable { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IParsable{T}"/> or not.
   ///
   /// This setting has no effect if:
   /// - the key is neither a <see cref="string"/> nor an <see cref="IParsable{T}"/> itself.
   /// </summary>
   public bool SkipIParsable { get; set; }

   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IComparisonOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect if:
   /// - the key is not an <see cref="IComparisonOperators{TSelf,TOther,TResult}"/> itself.
   /// </summary>
   public OperatorsGeneration ComparisonOperators { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of <see cref="IFormattable"/> or not.
   ///
   /// This setting has no effect if:
   /// - the key is not an <see cref="IFormattable"/> itself.
   /// </summary>
   public bool SkipIFormattable { get; set; }

   /// <summary>
   /// Indication whether the generator should skip the implementation of the method <see cref="object.ToString"/> or not.
   /// </summary>
   public bool SkipToString { get; set; }
}
