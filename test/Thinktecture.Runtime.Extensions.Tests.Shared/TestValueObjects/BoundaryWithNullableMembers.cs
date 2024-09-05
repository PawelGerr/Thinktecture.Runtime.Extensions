using System;
using System.Threading.Tasks;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public sealed partial class BoundaryWithNullableMembers
{
   public string? Prop1 { get; }
   public Func<string?, Task<string?>?>? Prop2 { get; }

   static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string? prop1, ref Func<string?, Task<string?>?>? prop2)
   {
   }
}
