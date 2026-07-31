using System;
using System.Collections.Generic;
using Thinktecture.Internal;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// A hand-written IObjectFactoryOwner, not an [ObjectFactory<ReadOnlySpan<byte>>] annotation: the MessagePack and
// Newtonsoft.Json code generators would select the ReadOnlySpan<byte> factory as the serialization value type and emit
// a formatter/converter attribute with a ref struct as its generic argument, which does not compile (backlog area A6
// aligns the generators with the runtime). The type simulates a class whose only conversion mechanism is a
// ReadOnlySpan<byte> object factory, so the runtime factories must exclude every ref-struct factory instead of only
// ReadOnlySpan<char>. Because the type has no key-based metadata either, no formatter or converter remains for it.
//
// The explicit interface member must keep the "global::" qualification. TypeExtensions.FindObjectFactoryMetadata looks
// the property up by the reflection name "global::Thinktecture.Internal.IObjectFactoryOwner.ObjectFactories", which is
// the name the source generator produces. A plain "IObjectFactoryOwner.ObjectFactories" declaration yields a reflection
// name without the prefix and is never found, which would make the tests pass for the wrong reason.
// ReSharper disable once InconsistentNaming
public class ClassWithByteSpanObjectFactoryMetadata : IObjectFactoryOwner
{
   static IReadOnlyList<ObjectFactoryMetadata> global::Thinktecture.Internal.IObjectFactoryOwner.ObjectFactories { get; } =
   [
      new ObjectFactoryMetadata
      {
         ValueType = typeof(ReadOnlySpan<byte>),
         ValidationErrorType = typeof(ValidationError),
         UseForSerialization = SerializationFrameworks.All,
         UseWithEntityFramework = false,
         UseForModelBinding = false,
         ConvertFromKeyExpressionViaConstructor = null
      }
   ];
}
