using System.Runtime.CompilerServices;
using DiffEngine;

namespace Thinktecture.Runtime.Tests;

internal class TestModuleInit
{
   [ModuleInitializer]
   internal static void ModuleInit()
   {
      DiffRunner.Disabled = true;
   }
}
