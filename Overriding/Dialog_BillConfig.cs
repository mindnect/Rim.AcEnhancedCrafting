using System;
using System.Collections.Generic;
using System.Text;
using AlcoholV.Extension;
using RimWorld;
using UnityEngine;
using Verse;
using Source = RimWorld.Dialog_BillConfig;

namespace AlcoholV.Overriding
{
    public class Dialog_BillConfig : Source
    {
        public IExtendable extendable;
        public RimWorld.Bill_Production bill;
        public IntVec3 billGiverPos;
        public Vector2 scrollPosition;

        public Dialog_BillConfig(RimWorld.Bill_Production bill, IntVec3 billGiverPos) : base(bill, billGiverPos)
        {
            extendable = (IExtendable) bill;
            this.bill = bill;
            this.billGiverPos = billGiverPos;
        }

        public override void DoWindowContents(Rect inRect)
        {
            AddRenameButton(inRect);
            AddRenamedLabel();

            #region Origianl

            Text.Font = GameFont.Small;
            var rect2 = new Rect(0f, 50f, 180f, inRect.height - 50f);
            var listing_Standard = new Listing_Standard(rect2);

            if (bill.suspended)
            {
                if (listing_Standard.ButtonText("Suspended".Translate(), null))
                    bill.suspended = false;
            }
            else if (listing_Standard.ButtonText("NotSuspended".Translate(), null))
            {
                bill.suspended = true;
            }

            #endregion

            AddAssignWorkerButton(listing_Standard);

            if (listing_Standard.ButtonText(bill.repeatMode.GetLabel(), null))
                BillRepeatModeUtility.MakeConfigFloatMenu(bill);

            var label = ("BillStoreMode_" + bill.storeMode).Translate();
            if (listing_Standard.ButtonText(label, null))
            {
                var list = new List<FloatMenuOption>();
                var enumerator = Enum.GetValues(typeof(BillStoreMode)).GetEnumerator();
                {
                    while (enumerator.MoveNext())
                    {
                        var billStoreMode = (BillStoreMode) (byte) enumerator.Current;
                        var smLocal = billStoreMode;
                        list.Add(new FloatMenuOption(("BillStoreMode_" + billStoreMode).Translate(), delegate { bill.storeMode = smLocal; }, MenuOptionPriority.Default, null, null, 0f, null));
                    }
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }

            listing_Standard.Gap(12f);
            if (bill.repeatMode == BillRepeatMode.RepeatCount)
            {
                listing_Standard.Label("RepeatCount".Translate(bill.RepeatInfoText));
                listing_Standard.IntSetter(ref bill.repeatCount, 1, "1", 42f);
                listing_Standard.IntAdjuster(ref bill.repeatCount, 1, 25, 1); // changed
                listing_Standard.IntAdjuster(ref bill.repeatCount, 10, 250, 1);
            }
            else if (bill.repeatMode == BillRepeatMode.TargetCount)
            {
                var text = "CurrentlyHave".Translate() + ": ";
                text += bill.recipe.WorkerCounter.CountProducts(bill);
                text += " / ";
                text += bill.targetCount >= 999999 ? "Infinite".Translate().ToLower() : bill.targetCount.ToString();
                var text2 = bill.recipe.WorkerCounter.ProductsDescription(bill);
                if (!text2.NullOrEmpty())
                {
                    var text3 = text;
                    text = string.Concat(text3, "\n", "CountingProducts".Translate(), ": ", text2);
                }
                listing_Standard.Label(text);
                // changed 
                // todo : extract method
                listing_Standard.IntSetter(ref bill.targetCount, 1, "1", 42f);
                listing_Standard.IntAdjuster(ref bill.targetCount, 1, 25, 1);
                listing_Standard.IntAdjuster(ref bill.targetCount, 10, 250, 1);
            }
            listing_Standard.Gap(12f);
            listing_Standard.Label("IngredientSearchRadius".Translate() + ": " + bill.ingredientSearchRadius.ToString("F0"));
            bill.ingredientSearchRadius = listing_Standard.Slider(bill.ingredientSearchRadius, 3f, 100f, null); // changed
            if (bill.ingredientSearchRadius >= 100f)
                bill.ingredientSearchRadius = 999f;

            //listing_Standard.Gap(-4f); // ∞∏ ¿ÃªÛ
            if (bill.recipe.workSkill != null)
            {
                listing_Standard.Label("AllowedSkillRange".Translate(bill.recipe.workSkill.label.ToLower()));
                listing_Standard.IntRange(ref bill.allowedSkillRange, 0, 20);
            }


            AddMinBarButton(listing_Standard);

            #region original

            listing_Standard.End();
            var rect3 = new Rect(rect2.xMax + 6f, 50f, 280f, -1f);
            rect3.yMax = inRect.height - CloseButSize.y - 6f;
            ThingFilterUI.DoThingFilterConfigWindow(rect3, ref scrollPosition, bill.ingredientFilter, bill.recipe.fixedIngredientFilter, 4);
            var rect4 = new Rect(rect3.xMax + 6f, rect3.y + 30f, 0f, 0f);
            rect4.xMax = inRect.xMax;
            rect4.yMax = inRect.height - CloseButSize.y - 6f;
            var stringBuilder = new StringBuilder();
            if (bill.recipe.description != null)
            {
                stringBuilder.AppendLine(bill.recipe.description);
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendLine("WorkAmount".Translate() + ": " + bill.recipe.WorkAmountTotal(null).ToStringWorkAmount());
            stringBuilder.AppendLine();
            for (var i = 0; i < bill.recipe.ingredients.Count; i++)
            {
                var ingredientCount = bill.recipe.ingredients[i];
                if (!ingredientCount.filter.Summary.NullOrEmpty())
                    stringBuilder.AppendLine(bill.recipe.IngredientValueGetter.BillRequirementsDescription(ingredientCount));
            }
            stringBuilder.AppendLine();
            var text4 = bill.recipe.IngredientValueGetter.ExtraDescriptionLine(bill.recipe);
            if (text4 != null)
            {
                stringBuilder.AppendLine(text4);
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendLine("MinimumSkills".Translate());
            stringBuilder.AppendLine(bill.recipe.MinSkillString);
            Text.Font = GameFont.Small;
            var text5 = stringBuilder.ToString();
            if (Text.CalcHeight(text5, rect4.width) > rect4.height)
                Text.Font = GameFont.Tiny;
            Widgets.Label(rect4, text5);
            Text.Font = GameFont.Small;
            if (bill.recipe.products.Count == 1)
                Widgets.InfoCardButton(rect4.x, rect3.y, bill.recipe.products[0].thingDef);

            #endregion
        }

        private void AddMinBarButton(Listing_Standard listing_Standard)
        {
            if (bill.repeatMode != BillRepeatMode.TargetCount) return;
            listing_Standard.Gap(12f);
            listing_Standard.Label("AlcoholV.MinLimit".Translate() + ": " + extendable.MinCount);
            extendable.MinCount = (int) listing_Standard.Slider(extendable.MinCount, 0f, bill.targetCount, "AlcoholV.MinLimitLabel".Translate());

            // button
            var t = extendable.MinCount;
            listing_Standard.IntSetter(ref t, bill.targetCount, "max".Translate());
            listing_Standard.IntAdjuster(ref t, 1, 25, 0); // changed
            if (t > bill.targetCount) t = bill.targetCount;
            extendable.MinCount = t;
        }

        private void AddAssignWorkerButton(Listing_Standard listing_Standard)
        {
            var workerLabel = "AlcoholV.Anybody".Translate();
            if (extendable.AssignedPawn != null) workerLabel = extendable.AssignedPawn.NameStringShort.CapitalizeFirst().Truncate(listing_Standard.ColumnWidth);

            if (listing_Standard.ButtonText(workerLabel, null))
            {
                var list = new List<FloatMenuOption>();
                list.Add(new FloatMenuOption("AlcoholV.Anybody".Translate(), delegate { extendable.AssignedPawn = null; }, MenuOptionPriority.Default, null, null, 0f, null));
                foreach (var colonist in bill.GetSortedSatisfyWorker())
                    list.Add(new FloatMenuOption(colonist.NameStringShort, delegate { extendable.AssignedPawn = colonist; }, MenuOptionPriority.Default, null, null, 0f, null));
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        private void AddRenameButton(Rect inRect)
        {
            var rect = new Rect(inRect.width - 80f, 0f, 30f, 30f);
            TooltipHandler.TipRegion(rect, new TipSignal("RenameBill".Translate()));
            if (Widgets.ButtonImage(rect, TexButton.Rename))
                Find.WindowStack.Add(new Dialog_Rename(extendable));
        }

        private void AddRenamedLabel()
        {
            Text.Font = GameFont.Medium;
            var rect = new Rect(0f, 0f, 400f, 50f);
            Widgets.Label(rect, extendable.Name.Truncate(400f));
        }
    }
}