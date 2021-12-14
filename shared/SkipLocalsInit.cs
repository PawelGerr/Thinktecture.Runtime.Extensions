[module: System.Runtime.CompilerServices.SkipLocalsInit]

#if !NET5_0 && !NET6_0
namespace System.Runtime.CompilerServices;

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
#endif
