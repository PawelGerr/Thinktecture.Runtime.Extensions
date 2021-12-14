#if !NET5_0 && !NET6_0
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
   // ReSharper disable once UnusedType.Global
   [AttributeUsage(AttributeTargets.Method)]
   internal sealed class ModuleInitializerAttribute : Attribute
   {
   }
}
#endif
