using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ValueObjectCodeGeneratorFactory
   : ICodeGeneratorFactory<KeyedValueObjectSourceGeneratorState>,
     ICodeGeneratorFactory<ComplexValueObjectSourceGeneratorState>
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

   public bool Equals(ICodeGeneratorFactory<KeyedValueObjectSourceGeneratorState>? other)
   {
      return ReferenceEquals(this, other);
   }

   public bool Equals(ICodeGeneratorFactory<ComplexValueObjectSourceGeneratorState> other)
   {
      return ReferenceEquals(this, other);
   }
}
