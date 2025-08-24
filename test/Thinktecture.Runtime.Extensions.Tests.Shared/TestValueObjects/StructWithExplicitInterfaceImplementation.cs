using System;
using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

public struct StructWithExplicitInterfaceImplementation : IParsable<StructWithExplicitInterfaceImplementation>
{
   static StructWithExplicitInterfaceImplementation IParsable<StructWithExplicitInterfaceImplementation>.Parse(
      string s,
      IFormatProvider? provider)
   {
      throw new NotImplementedException();
   }

   static bool IParsable<StructWithExplicitInterfaceImplementation>.TryParse(
      [NotNullWhen(true)] string? s,
      IFormatProvider? provider,
      out StructWithExplicitInterfaceImplementation result)
   {
      throw new NotImplementedException();
   }
}
