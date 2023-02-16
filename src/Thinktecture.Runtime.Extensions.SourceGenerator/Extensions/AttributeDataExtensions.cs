using Microsoft.CodeAnalysis;

namespace Thinktecture;

public static class AttributeDataExtensions
{
   public static string? FindKeyPropertyName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, "KeyPropertyName");
   }

   public static string? FindDefaultInstancePropertyName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, "DefaultInstancePropertyName");
   }

   public static bool? FindSkipFactoryMethods(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "SkipFactoryMethods");
   }

   public static bool? FindEmptyStringInFactoryMethodsYieldsNull(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "EmptyStringInFactoryMethodsYieldsNull");
   }

   public static bool? FindNullInFactoryMethodsYieldsNull(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "NullInFactoryMethodsYieldsNull");
   }

   public static bool? FindSkipIComparable(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "SkipIComparable");
   }

   public static bool? FindSkipIParsable(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "SkipIParsable");
   }

   public static bool? FindSkipToString(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "SkipToString");
   }

   public static (ITypeSymbol ComparerType, ITypeSymbol ItemType)? GetComparerTypes(this AttributeData attributeData)
   {
      var attrs = attributeData.AttributeClass?.TypeArguments;

      if (attrs?.Length != 2)
         return null;

      return (attrs.Value[0], attrs.Value[1]);
   }

   private static string? GetStringParameterValue(AttributeData attributeData, string name)
   {
      var value = (string?)attributeData.FindNamedAttribute(name).Value;

      return String.IsNullOrWhiteSpace(value) ? null : value?.Trim();
   }

   private static bool? GetBooleanParameterValue(AttributeData attributeData, string name)
   {
      return (bool?)attributeData.FindNamedAttribute(name).Value;
   }

   private static TypedConstant FindNamedAttribute(this AttributeData attributeData, string name)
   {
      return attributeData.NamedArguments.FirstOrDefault(a => a.Key == name).Value;
   }
}
