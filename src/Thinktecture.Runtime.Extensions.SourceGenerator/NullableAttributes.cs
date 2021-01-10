// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
   [AttributeUsageAttribute(AttributeTargets.Parameter)]
   internal sealed class MaybeNullWhenAttribute : Attribute
   {
      public bool ReturnValue { get; }

      public MaybeNullWhenAttribute(bool returnValue)
      {
         ReturnValue = returnValue;
      }
   }
}
