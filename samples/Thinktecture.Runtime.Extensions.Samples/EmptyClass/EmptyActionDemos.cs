namespace Thinktecture.EmptyClass;

// ReSharper disable UnusedParameter.Local
public class EmptyActionDemos
{
   public static void Demo()
   {
      Method_1(Empty.Action);
      Method_2(Empty.Action);
      Method_3(Empty.Action);
      Method_4(Empty.Action);
   }

   private static void Method_1(Action action)
   {
   }

   private static void Method_2(Action<string> action)
   {
   }

   private static void Method_3(Action<int> action)
   {
   }

   private static void Method_4(Action<string, int> action)
   {
   }
}
