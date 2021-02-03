[module: System.Runtime.CompilerServices.SkipLocalsInit]

#if !NET5_0
#pragma warning disable CA1812
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
   [AttributeUsage(AttributeTargets.Module
                   | AttributeTargets.Class
                   | AttributeTargets.Struct
                   | AttributeTargets.Interface
                   | AttributeTargets.Constructor
                   | AttributeTargets.Method
                   | AttributeTargets.Property
                   | AttributeTargets.Event, Inherited = false)]
   internal sealed class SkipLocalsInitAttribute : Attribute
   {
   }
}

#endif
