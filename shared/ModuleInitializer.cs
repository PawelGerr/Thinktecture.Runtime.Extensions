#if !NET5_0_OR_GREATER
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
