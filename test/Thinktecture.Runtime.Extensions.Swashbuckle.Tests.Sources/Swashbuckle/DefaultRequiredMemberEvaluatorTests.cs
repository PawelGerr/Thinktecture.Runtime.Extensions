using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Thinktecture.Swashbuckle.Internal.ComplexValueObjects;

namespace Thinktecture.Runtime.Tests.Swashbuckle;

public class DefaultRequiredMemberEvaluatorTests
{
   private sealed class SampleType
   {
      public string NonNullableReference { get; set; } = "";
      public string? NullableReference { get; set; }
      public int ValueType { get; set; }
      public int? NullableValueType { get; set; }
      public object ObjectReference { get; set; } = new();
      public string[] ArrayReference { get; set; } = [];
      public DateTime DateTimeValue { get; set; }
      public string NonNullableField = "";
      public IReadOnlyList<int> NonNullableInterface { get; set; } = [];
      public IReadOnlyList<int>? NullableInterface { get; set; }
   }

   [Fact]
   public void Should_treat_non_nullable_interface_typed_member_as_required()
   {
      // Regression: interfaces are reference types, but "Type.IsClass" is false for them. Using
      // "IsClass" wrongly excluded non-nullable interface-typed members from the required set,
      // contradicting the documented "non-nullable reference type is required" contract.
      var evaluator = new DefaultRequiredMemberEvaluator();

      var nonNullableInterface = typeof(SampleType).GetProperty(nameof(SampleType.NonNullableInterface))!;
      var nullableInterface = typeof(SampleType).GetProperty(nameof(SampleType.NullableInterface))!;

      evaluator.IsRequired(null!, null!, nonNullableInterface).Should().BeTrue();
      evaluator.IsRequired(null!, null!, nullableInterface).Should().BeFalse();
   }

   [Fact]
   public async Task Should_not_throw_when_IsRequired_is_called_concurrently()
   {
      // Regression: DefaultRequiredMemberEvaluator holds a single NullabilityInfoContext, which is
      // not thread-safe. As a registered singleton it serves parallel swagger generations, so its
      // NullabilityInfoContext.Create calls must be synchronized. Without synchronization the
      // concurrent access throws InvalidOperationException from the non-concurrent internal cache.
      var evaluator = new DefaultRequiredMemberEvaluator();

      var members = typeof(SampleType)
                    .GetMembers(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m is PropertyInfo or FieldInfo)
                    .ToArray();

      var tasks = Enumerable.Range(0, Environment.ProcessorCount * 4)
                            .Select(_ => Task.Run(() =>
                            {
                               for (var iteration = 0; iteration < 2_000; iteration++)
                               {
                                  foreach (var member in members)
                                     evaluator.IsRequired(null!, null!, member);
                               }
                            }))
                            .ToArray();

      var act = () => Task.WhenAll(tasks);

      await act.Should().NotThrowAsync();
   }
}
