using System.Linq.Expressions;
using System.Reflection;

namespace Thinktecture;

internal static class PropertyInfoExtensions
{
   private const string _EF_EXTENSION_TYPE_NAME = "System.Reflection.PropertyInfoExtensions, Microsoft.EntityFrameworkCore";
   private const string _EF_IS_CANDIDATE_PROPERTY = "IsCandidateProperty";

   private static readonly Func<PropertyInfo, bool> _isCandidateProperty;

   static PropertyInfoExtensions()
   {
      var efExtensionType = Type.GetType(_EF_EXTENSION_TYPE_NAME)
                            ?? throw new Exception($"The Entity Framework type '{_EF_EXTENSION_TYPE_NAME}' not found. The internal EF API may have been changed.");

      var isCandidatePropertyMethodInfo = efExtensionType.GetMethod(_EF_IS_CANDIDATE_PROPERTY, BindingFlags.Static | BindingFlags.Public)
                                          ?? throw new Exception($"The method '{_EF_IS_CANDIDATE_PROPERTY}' not found. The internal EF API may have been changed.");

      var parameter = Expression.Parameter(typeof(PropertyInfo));
      var methodCall = Expression.Call(null, isCandidatePropertyMethodInfo, parameter, Expression.Constant(true), Expression.Constant(true));

      _isCandidateProperty = Expression.Lambda<Func<PropertyInfo, bool>>(methodCall, parameter).Compile();
   }

   public static bool IsCandidateProperty(this PropertyInfo propertyInfo)
   {
      return _isCandidateProperty(propertyInfo);
   }
}
