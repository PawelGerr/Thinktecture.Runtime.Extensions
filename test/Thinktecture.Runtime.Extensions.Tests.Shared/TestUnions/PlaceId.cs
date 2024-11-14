namespace Thinktecture.Runtime.Tests.TestUnions;

[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class PlaceId
{
   public class Unknown : PlaceId
   {
      public static readonly Unknown Instance = new();

      private Unknown()
      {
      }
   };

   [ValueObject<int>]
   public partial class CountryId : PlaceId;

   public abstract class AbstractRegionId : PlaceId;

   public class RegionId : PlaceId
   {
      // ReSharper disable once MemberHidesStaticFromOuterClass
      public new class InnerRegionId : PlaceId;

      public class InnerRegionId2 : object;
   }

   public class InnerRegionId : RegionId;

   public class SpecialRegionId : AbstractRegionId;
}
