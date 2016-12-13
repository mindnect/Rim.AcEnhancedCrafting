using System.Reflection;
using AlcoholV.Overriding;
using Verse;
using Source = RimWorld.BillUtility;

namespace AlcoholV.Detouring
{
    public static class BillUtility
    {
        [Detour(typeof(Source), bindingFlags = BindingFlags.Static | BindingFlags.Public)]
        public static RimWorld.Bill MakeNewBill(this RecipeDef recipe)
        {
            if (recipe.UsesUnfinishedThing)
                return new Bill_ProductionWithUft(recipe);

            return new Bill_Production(recipe);
        }
    }
}