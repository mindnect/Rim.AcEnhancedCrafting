using System.Reflection;
using RimWorld;
using Verse;
using Bill_Production = AlcoholV.Overriding.Bill_Production;
using Bill_ProductionWithUft = AlcoholV.Overriding.Bill_ProductionWithUft;
using Source = RimWorld.BillUtility;
namespace AlcoholV.Detour
{
	public static class BillUtility
	{
        [Detour(typeof(Source), bindingFlags = BindingFlags.Static | BindingFlags.Public)]
        public static RimWorld.Bill MakeNewBill(this RecipeDef recipe)
		{
			if (recipe.UsesUnfinishedThing)
			{
                return new Bill_ProductionWithUft(recipe);
            }
			
			return new Bill_Production(recipe);
		}
	}
}
