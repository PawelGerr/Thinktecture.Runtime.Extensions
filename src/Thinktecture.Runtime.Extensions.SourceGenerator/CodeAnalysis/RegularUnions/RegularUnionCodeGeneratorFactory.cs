using System.Runtime.CompilerServices;
using System.Text;

namespace Thinktecture.CodeAnalysis.RegularUnions;

public sealed class RegularUnionCodeGeneratorFactory
   : ICodeGeneratorFactory<RegularUnionSourceGenState>,
     IEquatable<RegularUnionCodeGeneratorFactory>
{
   public static readonly ICodeGeneratorFactory<RegularUnionSourceGenState> Instance = new RegularUnionCodeGeneratorFactory();

   public string CodeGeneratorName => "RegularUnion-CodeGenerator";

   private RegularUnionCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(RegularUnionSourceGenState state, StringBuilder stringBuilder)
   {
      return new RegularUnionCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<RegularUnionSourceGenState>? other) => ReferenceEquals(this, other);
   public bool Equals(RegularUnionCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public override bool Equals(object? obj) => ReferenceEquals(this, obj);

   public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
}
