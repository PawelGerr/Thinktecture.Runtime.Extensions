#if NETSTANDARD2_0
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
internal sealed class AllowNullAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class NotNullWhenAttribute : Attribute
{
   public bool ReturnValue { get; }

   public NotNullWhenAttribute(bool returnValue)
   {
      ReturnValue = returnValue;
   }
}

[AttributeUsage(AttributeTargets.Parameter)]
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

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
internal sealed class MemberNotNullAttribute : Attribute
{
   public string[] Members { get; }

   public MemberNotNullAttribute(string member)
   {
      Members = new[] { member };
   }

   public MemberNotNullAttribute(params string[] members)
   {
      Members = members;
   }
}

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true)]
internal sealed class NotNullIfNotNullAttribute : Attribute
{
   public NotNullIfNotNullAttribute(string parameterName)
   {
      ParameterName = parameterName;
   }

   public string ParameterName { get; }
}
#endif
