using System.Reflection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle.Internal.ComplexValueObjects;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class DefaultRequiredMemberEvaluator : IRequiredMemberEvaluator
{
   private readonly NullabilityInfoContext _nullabilityInfoContext = new();

   // "NullabilityInfoContext" is not thread-safe: its "Create" methods use a non-concurrent
   // internal cache and can throw when called concurrently. This evaluator is registered as a
   // singleton and serves all (potentially parallel) swagger document generations, so access is
   // serialized with a lock.
   private readonly object _nullabilityInfoContextLock = new();

   /// <inheritdoc />
   public bool IsRequired(OpenApiSchema schema, SchemaFilterContext context, MemberInfo member)
   {
      Type type;
      NullabilityInfo nullabilityInfo;

      lock (_nullabilityInfoContextLock)
      {
         (type, nullabilityInfo) = member switch
         {
            PropertyInfo propertyInfo => (propertyInfo.PropertyType, _nullabilityInfoContext.Create(propertyInfo)),
            FieldInfo fieldInfo => (fieldInfo.FieldType, _nullabilityInfoContext.Create(fieldInfo)),
            _ => throw new ArgumentException($"Assignable member of a complex value object must be a field or a property but found '{member.GetType().FullName}'.", nameof(member))
         };
      }

      if (typeof(IDisallowDefaultValue).IsAssignableFrom(type))
         return true;

      // Use "ReadState" instead of "WriteState" because the members are read-only
      if (type.IsClass && nullabilityInfo.ReadState == NullabilityState.NotNull)
         return true;

      // Is struct, nullable struct or nullable reference type
      return false;
   }
}
