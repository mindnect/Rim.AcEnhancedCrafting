using System;
using System.Linq;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Dialog_BillConfig = AlcoholV.Overriding.Dialog_BillConfig;

namespace AlcoholV.Extension
{
    internal static class Bill_Production_Extension
    {
        private const int Shift = 5; // Shift Increment
        private const int Ctrl = 10; // Ctrl Increment

        public static bool ShouldDoNowExt(this Bill_Production _this)
        {
            if (_this.suspended) return false;
            var extendable = (IExtendable) _this;

            if (_this.repeatMode == BillRepeatModeDefOf.Forever)
            {
                extendable.IsPaused = false;
                return true;
            }
            if (_this.repeatMode == BillRepeatModeDefOf.RepeatCount)
            {
                extendable.IsPaused = false;
                return _this.repeatCount > 0;
            }

            if (_this.repeatMode == BillRepeatModeDefOf.TargetCount)
            {
                var currentCount = _this.recipe.WorkerCounter.CountProducts(_this);


                if (currentCount >= _this.targetCount)
                    extendable.IsPaused = true;

                else if (currentCount <= extendable.MinStock)
                    extendable.IsPaused = false;

                return !extendable.IsPaused;
            }

            throw new InvalidOperationException();
        }


        public static void DoConfigInterfaceExt(this Bill_Production _this, Rect baseRect, Color baseColor)
        {
            // counter label
            var rect = new Rect(28f, 32f, 100f, 30f);
            var counterLabel = _this.RepeatInfoText;

            // min count label
            GUI.color = new Color(1f, 1f, 1f, 0.65f);
            if (_this.repeatMode == BillRepeatModeDefOf.TargetCount)
            {
                var labelWidget = new WidgetRow(28, 32, UIDirection.RightThenDown);
                var extendable = (IExtendable) _this;
                counterLabel = extendable.MinStock + "/" + counterLabel;
                var str = counterLabel.Split('/');

                // min
                labelWidget.Label(str[0] + "/");
                // current
                GUI.color = new Color(1f, 1f, 1f, 1f);
                labelWidget.Label(str[1]);
                GUI.color = new Color(1f, 1f, 1f, 0.65f);


                // max
                labelWidget.Label("/" + str[2]);
            }
            else
            {
                Widgets.Label(rect, counterLabel);
            }

            GUI.color = baseColor;

            var widgetRow = new WidgetRow(baseRect.xMax, baseRect.y + 29f, UIDirection.LeftThenUp, 99999f, 4f);

            CreateAssignPawnButton(_this);
            CreateDetailButton(_this, widgetRow);
            //CreateHaulModeButton(_this, widgetRow);
            CreateRepeatButton(_this, widgetRow);
            CreatePlusMinusButton(_this, widgetRow);
            CreateOverlayLabel(_this, baseRect);
        }


        private static void CreateAssignPawnButton(Bill_Production _this)
        {
            var extendable = (IExtendable) _this;
            var rect2 = new Rect(210, 0f, 100f, 24f);
            var workerLabel = "Anybody".Translate();
            if (extendable.AssignedPawn != null) workerLabel = extendable.AssignedPawn.NameStringShort.CapitalizeFirst().Truncate(100f);
            if (Widgets.ButtonText(rect2, workerLabel))
            {
                var list = new List<FloatMenuOption>();
                list.Add(new FloatMenuOption("Anybody".Translate(), delegate { extendable.AssignedPawn = null; }));
                foreach (var colonist in _this.GetSortedSatisfyWorker())
                    list.Add(new FloatMenuOption(colonist.NameStringShort, delegate { extendable.AssignedPawn = colonist; }));
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        private static void CreateDetailButton(Bill_Production _this, WidgetRow widgetRow)
        {
            if (widgetRow.ButtonIcon(TexButton.Detail, "Details".Translate()))
                Find.WindowStack.Add(new Dialog_BillConfig(_this, ((Thing) _this.billStack.billGiver).Position));
        }
        
        private static void CreateHaulModeButton(Bill_Production _this, WidgetRow widgetRow)
        {
            var label = ("BillStoreMode_" + _this.storeMode).Translate();
            Texture2D tex = null;

            if (_this.storeMode == BillStoreModeDefOf.BestStockpile)
                    tex = TexButton.BestStockpile;
            else if (_this.storeMode == BillStoreModeDefOf.DropOnFloor)
                    tex = TexButton.DropOnFloor;

            if (widgetRow.ButtonIcon(tex, label))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (BillStoreModeDef current in from bsm in DefDatabase<BillStoreModeDef>.AllDefs
                                                     orderby bsm.listOrder
                                                     select bsm)
                {
                    BillStoreModeDef smLocal = current;
					list.Add(new FloatMenuOption(("BillStoreMode_" + current).Translate(), delegate
					{
                       _this.storeMode = smLocal;
					}, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        private static void CreateRepeatButton(Bill_Production _this, WidgetRow widgetRow)
        {
            Texture2D tex = null;
            if (_this.repeatMode == BillRepeatModeDefOf.RepeatCount)
                tex = TexButton.RepeatCount;
            else if (_this.repeatMode == BillRepeatModeDefOf.TargetCount)
                tex = TexButton.TargetCount;
            else if (_this.repeatMode == BillRepeatModeDefOf.Forever)
                tex = TexButton.Forever;

            if (widgetRow.ButtonIcon(tex, _this.repeatMode.GetLabel()))
                BillRepeatModeUtility.MakeConfigFloatMenu(_this);
        }

        private static void CreatePlusMinusButton(Bill_Production _this, WidgetRow widgetRow)
        {
            var extendable = (IExtendable) _this;

            // catch shfit click
            var isCtrl = Event.current.control;
            var isShfit = Event.current.shift;
            var isAlt = Event.current.alt;

            var modifier = 1;

            if (isShfit) modifier *= Shift;
            if (isCtrl) modifier *= Ctrl;

            widgetRow.Gap(8f);
            // + 버튼
            if (widgetRow.ButtonIcon(TexButton.Plus, "ShiftLabel".Translate()))
            {
                if (isAlt && _this.recipe.WorkerCounter.CanCountProducts(_this))
                {
                    _this.repeatMode = BillRepeatModeDefOf.TargetCount;
                }

                if (_this.repeatMode == BillRepeatModeDefOf.Forever)
                {
                    _this.repeatMode = BillRepeatModeDefOf.RepeatCount;
                    _this.repeatCount = 1;
                }
                else if (_this.repeatMode == BillRepeatModeDefOf.TargetCount)
                {
                    // catch alt and change min stock
                    if (isAlt) extendable.MinStock = Mathf.Min(_this.targetCount, extendable.MinStock + _this.recipe.targetCountAdjustment*modifier); // compare max value
                    else _this.targetCount += _this.recipe.targetCountAdjustment*modifier;
                }
                else if (_this.repeatMode == BillRepeatModeDefOf.RepeatCount)
                {
                    _this.repeatCount += modifier;
                }
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                if (TutorSystem.TutorialMode && (_this.repeatMode == BillRepeatModeDefOf.RepeatCount))
                    TutorSystem.Notify_Event(_this.recipe.defName + "-RepeatCountSetTo-" + _this.repeatCount);
            }

            // - 버튼
            if (widgetRow.ButtonIcon(TexButton.Minus, "ShiftLabel".Translate()))
            {
                if (isAlt && _this.recipe.WorkerCounter.CanCountProducts(_this))
                {
                    _this.repeatMode = BillRepeatModeDefOf.TargetCount;
                }

                if (_this.repeatMode == BillRepeatModeDefOf.Forever)
                {
                    _this.repeatMode = BillRepeatModeDefOf.RepeatCount;
                    _this.repeatCount = 1;
                }
                else if (_this.repeatMode == BillRepeatModeDefOf.TargetCount)
                {
                    // catch alt and change min stock
                    if (isAlt) extendable.MinStock = Mathf.Max(0, extendable.MinStock - _this.recipe.targetCountAdjustment*modifier); // catch alt and change min stock
                    else
                    {
                        _this.targetCount = Mathf.Max(0, _this.targetCount - _this.recipe.targetCountAdjustment*modifier);
                        extendable.MinStock = Mathf.Min(extendable.MinStock, _this.targetCount);
                    }
                }
                else if (_this.repeatMode == BillRepeatModeDefOf.RepeatCount)
                {
                    _this.repeatCount = Mathf.Max(0, _this.repeatCount - modifier);
                }
                SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
                if (TutorSystem.TutorialMode && (_this.repeatMode == BillRepeatModeDefOf.RepeatCount))
                    TutorSystem.Notify_Event(_this.recipe.defName + "-RepeatCountSetTo-" + _this.repeatCount);
            }
        }

        private static void CreateOverlayLabel(Bill_Production _this, Rect baseRect)
        {
            var extendable = (IExtendable) _this;
            if (_this.suspended || !extendable.IsPaused) return;

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            var rect = new Rect(baseRect.x + baseRect.width/2f - 70f, baseRect.y + baseRect.height/2f - 20f, 140f, 40f);
            GUI.DrawTexture(rect, TexUI.GrayTextBG);
            Widgets.Label(rect, "Paused".Translate());

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }
    }
}