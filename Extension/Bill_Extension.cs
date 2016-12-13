using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Dialog_BillConfig = AlcoholV.Overriding.Dialog_BillConfig;

namespace AlcoholV
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
            return _this.recipe.requiredGiverWorkType;
            //var thing = _this.billStack.billGiver as Thing;
            //if (thing != null)
            //{
            //    WorkTypeDef workTypeDef = null;
            //    var allDefsListForReading = DefDatabase<WorkGiverDef>.AllDefsListForReading;
            //    foreach (WorkGiverDef t in allDefsListForReading)
            //    {
            //        if (t.fixedBillGiverDefs != null && t.fixedBillGiverDefs.Contains(thing.def))
            //        {
            //            workTypeDef = t.workType;
            //            break;
            //        }
            //    }
            //    return workTypeDef;
            //}
            //return null;
        }
    }
}