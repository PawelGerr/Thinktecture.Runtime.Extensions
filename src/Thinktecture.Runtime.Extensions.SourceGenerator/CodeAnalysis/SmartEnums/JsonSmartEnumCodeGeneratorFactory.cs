using System.Runtime.CompilerServices;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class JsonSmartEnumCodeGeneratorFactory
   : JsonKeyedSerializerCodeGeneratorFactoryBase,
     IEquatable<JsonSmartEnumCodeGeneratorFactory>
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new JsonSmartEnumCodeGeneratorFactory();

   public override string CodeGeneratorName => "SystemTextJson-SmartEnum-CodeGenerator";

   private JsonSmartEnumCodeGeneratorFactory()
      : base(false)
   {
   }

   public bool Equals(JsonSmartEnumCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public override bool Equals(object? obj) => ReferenceEquals(this, obj);

   public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
}
