using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AlcoholV.Extension
{
    internal static class Bill_Extension
    {
        public static IEnumerable<Pawn> GetSortedSatisfyWorker(this Bill _this)
        {
            var satisfyWorkers = Find.VisibleMap.mapPawns.FreeColonists.Where(_this.recipe.PawnSatisfiesSkillRequirements).Where(p => p.workSettings.WorkIsActive(_this.GetWorkType()));
            var sortedWorkers = satisfyWorkers.OrderByDescending(p => p.skills.GetSkill(_this.recipe.workSkill).Level);
            return sortedWorkers;
        }

        public static WorkTypeDef GetWorkType(this Bill _this)
        {
            var thing = _this.billStack.billGiver as Thing;
            if (thing == null) return null;

            var allDefsListForReading = DefDatabase<WorkGiverDef>.AllDefsListForReading;
            return (from t in allDefsListForReading where (t.fixedBillGiverDefs != null) && t.fixedBillGiverDefs.Contains(thing.def) select t.workType).FirstOrDefault();
        }
    }
}