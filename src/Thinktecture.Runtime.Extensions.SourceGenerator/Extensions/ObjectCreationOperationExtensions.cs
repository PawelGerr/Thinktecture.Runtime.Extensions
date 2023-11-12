using Microsoft.CodeAnalysis.Operations;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class ObjectCreationOperationExtensions
{
   public static bool? FindIsValidatable(this IObjectCreationOperation operation)
   {
      return GetBooleanParameterValue(operation.Initializer, "IsValidatable");
   }

   public static string? FindKeyPropertyName(this IObjectCreationOperation operation)
   {
      return GetStringParameterValue(operation.Initializer, Constants.Attributes.SmartEnum.Properties.KEY_PROPERTY_NAME);
   }

   private static bool? GetBooleanParameterValue(IObjectOrCollectionInitializerOperation? initializer, string name)
   {
      return (bool?)initializer?.Initializers.FindInitialization(name);
   }

   private static string? GetStringParameterValue(IObjectOrCollectionInitializerOperation? initializer, string name)
   {
      var value = (string?)initializer?.Initializers.FindInitialization(name);

      return String.IsNullOrWhiteSpace(value) ? null : value?.Trim();
   }

   private static object? FindInitialization(this ImmutableArray<IOperation> initializations, string name)
   {
      for (var i = 0; i < initializations.Length; i++)
      {
         var init = initializations[i];

         if (init.Kind != OperationKind.SimpleAssignment || init is not ISimpleAssignmentOperation simpleAssignment)
            continue;

         if (simpleAssignment.Target.Kind != OperationKind.PropertyReference || simpleAssignment.Target is not IPropertyReferenceOperation propRef)
            continue;

         if (propRef.Property.Name != name)
            continue;

         return simpleAssignment.Value.ConstantValue.HasValue ? simpleAssignment.Value.ConstantValue.Value : null;
      }

      return null;
   }
}
