using System;
using Microsoft.CodeAnalysis;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class AttributeDataExtensions
   {
      public static string? FindKeyPropertyName(this AttributeData attributeData)
      {
         return GetStringParameterValue(attributeData, "KeyPropertyName");
      }

      public static string? FindEqualityComparer(this AttributeData attributeData)
      {
         return GetStringParameterValue(attributeData, "EqualityComparer");
      }

      public static string? FindKeyComparer(this AttributeData attributeData)
      {
         return GetStringParameterValue(attributeData, "KeyComparer");
      }

      public static bool? FindSkipFactoryMethods(this AttributeData attributeData)
      {
         return GetBooleanParameterValue(attributeData, "SkipFactoryMethods");
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
}
