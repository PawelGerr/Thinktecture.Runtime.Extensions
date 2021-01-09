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

   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
   internal sealed class MemberNotNullWhenAttribute : Attribute
   {
      public bool ReturnValue { get; }
      public string[] Members { get; }

      public MemberNotNullWhenAttribute(bool returnValue, string member)
      {
         ReturnValue = returnValue;
         Members = new[] { member };
      }

      public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
      {
         ReturnValue = returnValue;
         Members = members;
      }
   }
}
