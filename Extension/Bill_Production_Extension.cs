using System;
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

        public static bool ShouldDoNowExt(this Bill_Production _this)
        {
    
            if (_this.suspended) return false;
            var extendable = (IExtendable)_this;

            if (_this.repeatMode == BillRepeatMode.Forever)
            {
                extendable.IsPaused = false;
                return true;
            }
            if (_this.repeatMode == BillRepeatMode.RepeatCount)
            {
                extendable.IsPaused = false;
                return _this.repeatCount > 0;
            }

            if (_this.repeatMode == BillRepeatMode.TargetCount)
            {
                var currentCount = _this.recipe.WorkerCounter.CountProducts(_this);
                

                if (currentCount >= _this.targetCount)
                {
                    extendable.IsPaused = true;
                }

                else if (currentCount <= extendable.MinCount)
                {
                    extendable.IsPaused = false;
                }

                return !extendable.IsPaused;
            }

            throw new InvalidOperationException();
        }


        public static void DrawConfigInterfaceExt(this Bill_Production _this, Rect baseRect, Color baseColor)
        {
            // counter label
            GUI.color = baseColor;
            var rect = new Rect(28f, 32f, 100f, 30f);
            var counterLabel = _this.RepeatInfoText;

            //// min count label
            //if (_this.repeatMode == BillRepeatMode.TargetCount)
            //{
            //    counterLabel = counterLabel.Insert(0, _extendable.MinCount + "/");
            //}

            Widgets.Label(rect, counterLabel);
      

            var widgetRow = new WidgetRow(baseRect.xMax, baseRect.y + 29f, UIDirection.LeftThenUp, 99999f, 4f);
            CreateAssignPawnButton(_this, baseColor);
            CreateDetailButton(_this, widgetRow, baseColor);
            CreateHaulModeButton(_this, widgetRow, baseColor);
            CreateRepeatButton(_this, widgetRow, baseColor);
            CreatePlusMinusButton(_this,widgetRow, baseColor);
            CreateOverlayLabel(_this, baseRect);
        }


        private static void CreateAssignPawnButton(Bill_Production _this, Color baseColor)
        {
            var extendable = (IExtendable)_this;
            GUI.color = baseColor;
            var rect2 = new Rect(210, 0f, 100f, 24f);
            var workerLabel = "AlcoholV.Anybody".Translate();
            if (extendable.AssignedPawn != null) workerLabel = extendable.AssignedPawn.NameStringShort.CapitalizeFirst().Truncate(100f);
            if (Widgets.ButtonText(rect2, workerLabel))
            {
                var list = new List<FloatMenuOption>();
                list.Add(new FloatMenuOption("AlcoholV.Anybody".Translate(), delegate { extendable.AssignedPawn = null; }, MenuOptionPriority.Default, null, null, 0f, null));
                foreach (var colonist in _this.GetSortedSatisfyWorker())
                {
                    list.Add(new FloatMenuOption(colonist.NameStringShort, delegate { extendable.AssignedPawn = colonist; }, MenuOptionPriority.Default, null, null, 0f, null));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        private static void CreateDetailButton(Bill_Production _this, WidgetRow widgetRow, Color baseColor)
        {
            GUI.color = baseColor;
            if (widgetRow.ButtonIcon(TexButton.Detail, "Details".Translate()))
            {
                Find.WindowStack.Add(new Dialog_BillConfig(_this, ((Thing)_this.billStack.billGiver).Position));
            }
        }

        private static void CreateHaulModeButton(Bill_Production _this, WidgetRow widgetRow, Color baseColor)
        {
            GUI.color = baseColor;
            var label = ("BillStoreMode_" + _this.storeMode).Translate();
            Texture2D tex = null;

            switch (_this.storeMode)
            {
                case BillStoreMode.BestStockpile:
                    tex = TexButton.BestStockpile;
                    break;
                case BillStoreMode.DropOnFloor:
                    tex = TexButton.DropOnFloor;
                    break;
            }

            if (widgetRow.ButtonIcon(tex, label))
            {
                var list = new List<FloatMenuOption>();
                var enumerator = Enum.GetValues(typeof (BillStoreMode)).GetEnumerator();
                {
                    while (enumerator.MoveNext())
                    {
                        var billStoreMode = (BillStoreMode) (byte) enumerator.Current;
                        var smLocal = billStoreMode;
                        list.Add(new FloatMenuOption(("BillStoreMode_" + billStoreMode).Translate(), delegate { _this.storeMode = smLocal; }, MenuOptionPriority.Default, null, null, 0f, null));
                    }
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        private static void CreateRepeatButton(Bill_Production _this, WidgetRow widgetRow, Color baseColor)
        {
            GUI.color = baseColor;
            Texture2D tex = null;
            switch (_this.repeatMode)
            {
                case BillRepeatMode.RepeatCount:
                    tex = TexButton.RepeatCount;
                    break;
                case BillRepeatMode.TargetCount:
                    tex = TexButton.TargetCount;
                    break;
                case BillRepeatMode.Forever:
                    tex = TexButton.Forever;
                    break;
            }

            if (widgetRow.ButtonIcon(tex, _this.repeatMode.GetLabel()))
            {
                BillRepeatModeUtility.MakeConfigFloatMenu(_this);
            }
        }

        private static void CreatePlusMinusButton(Bill_Production _this, WidgetRow widgetRow, Color baseColor)
        {
            GUI.color = baseColor;
            widgetRow.Gap(8f);
            // + 버튼
            if (widgetRow.ButtonIcon(TexButton.Plus, null))
            {
                if (_this.repeatMode == BillRepeatMode.Forever)
                {
                    _this.repeatMode = BillRepeatMode.RepeatCount;
                    _this.repeatCount = 1;
                }
                else if (_this.repeatMode == BillRepeatMode.TargetCount)
                {
                    _this.targetCount += _this.recipe.targetCountAdjustment;
                }
                else if (_this.repeatMode == BillRepeatMode.RepeatCount)
                {
                    _this.repeatCount++;
                }
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                if (TutorSystem.TutorialMode && _this.repeatMode == BillRepeatMode.RepeatCount)
                {
                    TutorSystem.Notify_Event(_this.recipe.defName + "-RepeatCountSetTo-" + _this.repeatCount);
                }
            }

            GUI.color = baseColor;
            // - 버튼
            if (widgetRow.ButtonIcon(TexButton.Minus, null))
            {
                if (_this.repeatMode == BillRepeatMode.Forever)
                {
                    _this.repeatMode = BillRepeatMode.RepeatCount;
                    _this.repeatCount = 1;
                }
                else if (_this.repeatMode == BillRepeatMode.TargetCount)
                {
                    _this.targetCount = Mathf.Max(0, _this.targetCount - _this.recipe.targetCountAdjustment);
                }
                else if (_this.repeatMode == BillRepeatMode.RepeatCount)
                {
                    _this.repeatCount = Mathf.Max(0, _this.repeatCount - 1);
                }
                SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
                if (TutorSystem.TutorialMode && _this.repeatMode == BillRepeatMode.RepeatCount)
                {
                    TutorSystem.Notify_Event(_this.recipe.defName + "-RepeatCountSetTo-" + _this.repeatCount);
                }
            }
        }

        private static void CreateOverlayLabel(Bill_Production _this, Rect baseRect)
        {
            var extendable = (IExtendable)_this;
            if (_this.suspended || !extendable.IsPaused) return;

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            var rect = new Rect(baseRect.x + baseRect.width/2f - 70f, baseRect.y + baseRect.height/2f - 20f, 140f, 40f);
            GUI.DrawTexture(rect, TexUI.GrayTextBG);
            Widgets.Label(rect, "AlcoholV.Paused".Translate());

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }
    }
}