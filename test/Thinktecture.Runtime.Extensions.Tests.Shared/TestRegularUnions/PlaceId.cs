namespace Thinktecture.Runtime.Tests.TestRegularUnions;

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

   public abstract class AbstractRegionId : PlaceId
   {
      private AbstractRegionId()
      {
      }

      public sealed class SpecialRegionId : AbstractRegionId;
   }

   public class RegionId : PlaceId
   {
      private RegionId()
      {
      }

      public sealed class InnerRegionId : PlaceId;

      public class InnerRegionId2 : object;
   }
}
