using Microsoft.CodeAnalysis.Operations;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class ObjectCreationOperationExtensions
{
   public static bool? FindSkipIComparable(this IObjectCreationOperation operation)
   {
      return GetBooleanParameterValue(operation.Initializer, Constants.Attributes.Properties.SKIP_ICOMPARABLE);
   }

   public static string? FindKeyMemberName(this IObjectCreationOperation operation)
   {
      return GetStringParameterValue(operation.Initializer, Constants.Attributes.Properties.KEY_MEMBER_NAME);
   }

   public static bool? FindSkipKeyMember(this IObjectCreationOperation operation)
   {
      return GetBooleanParameterValue(operation.Initializer, Constants.Attributes.Properties.SKIP_KEY_MEMBER);
   }

   public static bool? FindSkipEqualityComparison(this IObjectCreationOperation operation)
   {
      return GetBooleanParameterValue(operation.Initializer, Constants.Attributes.Properties.SKIP_EQUALITY_COMPARISON);
   }

   public static bool? FindAllowDefaultStructs(this IObjectCreationOperation operation)
   {
      return GetBooleanParameterValue(operation.Initializer, Constants.Attributes.Properties.ALLOW_DEFAULT_STRUCTS);
   }

   public static bool? FindHasCorrespondingConstructor(this IObjectCreationOperation operation)
   {
      return GetBooleanParameterValue(operation.Initializer, Constants.Attributes.Properties.HAS_CORRESPONDING_CONSTRUCTOR);
   }

   public static AccessModifier? FindKeyMemberAccessModifier(this IObjectCreationOperation operation)
   {
      var modifier = (AccessModifier?)GetIntegerParameterValue(operation.Initializer, Constants.Attributes.Properties.KEY_MEMBER_ACCESS_MODIFIER);

      if (modifier is null || !modifier.Value.IsValid())
         return null;

      return modifier.Value;
   }

   public static MemberKind? FindKeyMemberKind(this IObjectCreationOperation operation)
   {
      var kind = (MemberKind?)GetIntegerParameterValue(operation.Initializer, Constants.Attributes.Properties.KEY_MEMBER_KIND);

      if (kind is null || !kind.Value.IsValid())
         return null;

      return kind.Value;
   }

   public static bool HasDefaultStringComparison(this IObjectCreationOperation operation)
   {
      return operation.Initializer is not null
             && GetEnumParameterValue<StringComparison>(operation, Constants.Attributes.Properties.DEFAULT_STRING_COMPARISON) is not null;
   }

   private static T? GetEnumParameterValue<T>(IObjectCreationOperation initializer, string name)
      where T : struct, Enum
   {
      return initializer.Initializer?.Initializers.FindInitialization(name) is int value ? value.GetValidValue<T>() : null;
   }

   private static bool? GetBooleanParameterValue(IObjectOrCollectionInitializerOperation? initializer, string name)
   {
      return (bool?)initializer?.Initializers.FindInitialization(name);
   }

   private static int? GetIntegerParameterValue(IObjectOrCollectionInitializerOperation? initializer, string name)
   {
      var obj = initializer?.Initializers.FindInitialization(name);

      return obj switch
      {
         int i => i,
         byte b => b,
         short s => s,
         long l when l >= int.MinValue && l <= int.MaxValue => (int)l,
         Enum e => Convert.ToInt32(e, System.Globalization.CultureInfo.InvariantCulture),
         _ => null
      };
   }

   private static string? GetStringParameterValue(IObjectOrCollectionInitializerOperation? initializer, string name)
   {
      var value = (string?)initializer?.Initializers.FindInitialization(name);

      return String.IsNullOrWhiteSpace(value) ? null : value?.Trim();
   }

   private static object? FindInitialization(this ImmutableArray<IOperation> initializations, string name)
   {
      if (initializations.IsDefaultOrEmpty)
         return null;

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
