#if !NETSTANDARD2_1 && !NET5_0

// ReSharper disable All

namespace System.Diagnostics.CodeAnalysis
{
   [AttributeUsage(AttributeTargets.Parameter)]
   internal sealed class MaybeNullWhenAttribute : Attribute
   {
      public bool ReturnValue { get; }

      public MaybeNullWhenAttribute(bool returnValue)
      {
         ReturnValue = returnValue;
      }
   }

   [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
   internal sealed class NotNullWhenAttribute : Attribute
   {
      public bool ReturnValue { get; }

      public NotNullWhenAttribute(bool returnValue)
      {
         ReturnValue = returnValue;
      }
   }

   [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
   internal sealed class NotNullIfNotNullAttribute : Attribute
   {
      public string ParameterName { get; }

      public NotNullIfNotNullAttribute(string parameterName)
      {
         ParameterName = parameterName;
      }
   }

   [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
   internal sealed class AllowNullAttribute : Attribute
   {
   }

   [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
   internal sealed class MaybeNullAttribute : Attribute
   {
   }

   [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
   internal sealed class NotNullAttribute : Attribute
   {
   }
}
#endif
