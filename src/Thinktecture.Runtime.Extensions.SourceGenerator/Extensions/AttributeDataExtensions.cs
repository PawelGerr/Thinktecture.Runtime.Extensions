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

      public static string? FindComparer(this AttributeData attributeData)
      {
         return GetStringParameterValue(attributeData, "Comparer");
      }

      public static string? FindKeyComparer(this AttributeData attributeData)
      {
         return GetStringParameterValue(attributeData, "KeyComparer");
      }

      private static string? GetStringParameterValue(AttributeData attributeData, string name)
      {
         var value = attributeData.FindNamedAttribute(name).Value?.ToString();

         return String.IsNullOrWhiteSpace(value) ? null : value?.Trim();
      }

      private static TypedConstant FindNamedAttribute(this AttributeData attributeData, string name)
      {
         return attributeData.NamedArguments.FirstOrDefault(a => a.Key == name).Value;
      }
   }
}
