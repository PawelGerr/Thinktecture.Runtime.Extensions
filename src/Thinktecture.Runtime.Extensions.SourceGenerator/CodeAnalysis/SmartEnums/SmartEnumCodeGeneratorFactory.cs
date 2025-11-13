using System.Runtime.CompilerServices;
using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class SmartEnumCodeGeneratorFactory
   : ICodeGeneratorFactory<SmartEnumSourceGeneratorState>,
     IEquatable<SmartEnumCodeGeneratorFactory>
{
   public static readonly ICodeGeneratorFactory<SmartEnumSourceGeneratorState> Instance = new SmartEnumCodeGeneratorFactory();

   public string CodeGeneratorName => "SmartEnum-CodeGenerator";

   private SmartEnumCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(SmartEnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new SmartEnumCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<SmartEnumSourceGeneratorState>? other) => ReferenceEquals(this, other);
   public bool Equals(SmartEnumCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public override bool Equals(object? obj) => ReferenceEquals(this, obj);

   public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
}
