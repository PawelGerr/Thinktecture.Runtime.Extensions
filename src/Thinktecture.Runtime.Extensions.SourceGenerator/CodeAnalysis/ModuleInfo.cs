namespace Thinktecture.CodeAnalysis;

public readonly struct ModuleInfo
{
   public string Name { get; }

   public ModuleInfo(string name)
   {
      Name = name;
   }
}
