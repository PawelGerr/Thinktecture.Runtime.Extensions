using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
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
      public Func<int> NonNullableDelegate { get; set; } = () => 0;
      public Func<int>? NullableDelegate { get; set; }
   }

   private static readonly OpenApiSchema _schema = new();

   private static SchemaFilterContext CreateContext(MemberInfo member)
   {
      // The evaluator reads the member only, but passing real arguments keeps the test independent of that detail.
      // A real ISchemaGenerator would drag in SchemaGeneratorOptions plus a data contract resolver for no gain.
      return new SchemaFilterContext(typeof(SampleType), schemaGenerator: null!, new SchemaRepository(), memberInfo: member);
   }

   private static MemberInfo GetMember(string memberName)
   {
      // GetMember (not GetProperty) because SampleType.NonNullableField is a field.
      return typeof(SampleType).GetMember(memberName, BindingFlags.Public | BindingFlags.Instance).Single();
   }

   [Theory]
   // Non-nullable reference types (classes, object, arrays, delegates, interfaces) are required. Interfaces are a
   // regression case: they are reference types, but "Type.IsClass" is false for them. Using "IsClass" wrongly excluded
   // non-nullable interface-typed members from the required set, contradicting the documented
   // "non-nullable reference type is required" contract.
   [InlineData(nameof(SampleType.NonNullableReference), true)]
   [InlineData(nameof(SampleType.ObjectReference), true)]
   [InlineData(nameof(SampleType.ArrayReference), true)]
   [InlineData(nameof(SampleType.NonNullableDelegate), true)]
   [InlineData(nameof(SampleType.NonNullableInterface), true)]
   [InlineData(nameof(SampleType.NonNullableField), true)]
   // Nullable reference types and all value types (including non-nullable structs) are not required.
   [InlineData(nameof(SampleType.NullableReference), false)]
   [InlineData(nameof(SampleType.NullableInterface), false)]
   [InlineData(nameof(SampleType.NullableDelegate), false)]
   [InlineData(nameof(SampleType.ValueType), false)]
   [InlineData(nameof(SampleType.NullableValueType), false)]
   [InlineData(nameof(SampleType.DateTimeValue), false)]
   public void Should_evaluate_required_state_from_member_type_and_nullability(string memberName, bool expected)
   {
      var evaluator = new DefaultRequiredMemberEvaluator();
      var member = GetMember(memberName);

      evaluator.IsRequired(_schema, CreateContext(member), member).Should().Be(expected);
   }

   [Fact]
   public async Task Should_not_throw_when_IsRequired_is_called_concurrently()
   {
      // Regression: DefaultRequiredMemberEvaluator holds a single NullabilityInfoContext, which is not thread-safe. As
      // a registered singleton it serves parallel swagger generations, so its NullabilityInfoContext.Create calls must
      // be synchronized. Without synchronization the concurrent access throws InvalidOperationException from the
      // non-concurrent internal cache. That race exists only while the internal cache is cold, so each round uses a
      // fresh evaluator and a Barrier that makes all workers hit the cold cache simultaneously. Detection of a missing
      // lock stays probabilistic; this is a high-probability smoke test, not a proof.
      var members = typeof(SampleType)
                    .GetMembers(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m is PropertyInfo or FieldInfo)
                    .ToArray();

      var contexts = members.Select(CreateContext).ToArray();
      var workerCount = Math.Max(2, Environment.ProcessorCount);

      var act = async () =>
      {
         for (var round = 0; round < 200; round++)
         {
            var evaluator = new DefaultRequiredMemberEvaluator();
            using var barrier = new Barrier(workerCount);

            var tasks = Enumerable.Range(0, workerCount)
                                  .Select(_ => Task.Factory.StartNew(() =>
                                  {
                                     barrier.SignalAndWait();

                                     for (var i = 0; i < members.Length; i++)
                                        evaluator.IsRequired(_schema, contexts[i], members[i]);
                                  }, TaskCreationOptions.LongRunning))
                                  .ToArray();

            await Task.WhenAll(tasks);
         }
      };

      await act.Should().NotThrowAsync();
   }
}
