using System.Runtime.CompilerServices;
using DiffEngine;
using VerifyXunit;

namespace Thinktecture.Runtime.Tests;

internal class TestModuleInit
{
   [ModuleInitializer]
   internal static void ModuleInit()
   {
      DiffRunner.Disabled = true;

      Verifier.UseProjectRelativeDirectory("../Thinktecture.Runtime.Extensions.Swashbuckle.3_0.Tests");
   }
}
