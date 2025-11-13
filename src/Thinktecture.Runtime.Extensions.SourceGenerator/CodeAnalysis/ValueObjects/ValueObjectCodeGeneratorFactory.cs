using System.Runtime.CompilerServices;
using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ValueObjectCodeGeneratorFactory
   : ICodeGeneratorFactory<KeyedValueObjectSourceGeneratorState>,
     ICodeGeneratorFactory<ComplexValueObjectSourceGeneratorState>,
     IEquatable<ValueObjectCodeGeneratorFactory>
{
   public static readonly ValueObjectCodeGeneratorFactory Instance = new();

   public string CodeGeneratorName => "ValueObject-CodeGenerator";

   private ValueObjectCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(KeyedValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new KeyedValueObjectCodeGenerator(state, stringBuilder);
   }

   public CodeGeneratorBase Create(ComplexValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new ComplexValueObjectCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<KeyedValueObjectSourceGeneratorState>? other) => ReferenceEquals(this, other);
   public bool Equals(ICodeGeneratorFactory<ComplexValueObjectSourceGeneratorState>? other) => ReferenceEquals(this, other);
   public bool Equals(ValueObjectCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public override bool Equals(object? obj) => ReferenceEquals(this, obj);

   public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
}
