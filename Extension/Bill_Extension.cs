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
            var requireWorkType = _this.GetWorkType();
            var satisfyWorkers = Find.VisibleMap.mapPawns.FreeColonists.Where(_this.recipe.PawnSatisfiesSkillRequirements).Where(p => p.workSettings.WorkIsActive(requireWorkType));

            if ((requireWorkType.workTags & WorkTags.Hauling) !=0)
            {
                return satisfyWorkers.OrderByDescending(p => p.GetStatValue(StatDefOf.MoveSpeed));
            }
            return satisfyWorkers.OrderByDescending(p => p.skills.GetSkill(_this.recipe.workSkill).Level);
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