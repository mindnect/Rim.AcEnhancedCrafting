using RimWorld;
using Verse;
using Verse.Sound;

namespace AlcoholV.Extension
{
    internal static class Listing_Standard_Extension
    {
        public static float Slider(this Listing_Standard _this, float val, float min, float max, string tooltip = null)
        {
            var rect = _this.GetRect(18f);
            var result = Widgets.HorizontalSlider(rect, val, min, max);
            if (!tooltip.NullOrEmpty())
                TooltipHandler.TipRegion(rect, tooltip);
            _this.Gap(_this.verticalSpacing);
            return result;
        }

        public static void IntAdjuster(this Listing_Standard _this, ref int val, int smallChange, int largeChange, int min = 0)
        {
            const float gap = 2f;
            var btnWidth = (_this.ColumnWidth - 3*gap)/4f;
            //var blank = _this.ColumnWidth - btnWidth*4f - gap*2;

            var rect = _this.GetRect(24f);
            rect.width = btnWidth;

            // minus
            if (Widgets.ButtonText(rect, "-" + largeChange, true, false, true))
            {
                SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
                val -= largeChange;
                if (val < min)
                    val = min;
            }
            rect.x += rect.width + gap;
            if (Widgets.ButtonText(rect, "-" + smallChange, true, false, true))
            {
                SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
                val -= smallChange;
                if (val < min)
                    val = min;
            }

            // plus
            rect.x += rect.width + gap;
            if (Widgets.ButtonText(rect, "+" + smallChange, true, false, true))
            {
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                val += smallChange;
                if (val < min)
                    val = min;
            }
            rect.x += rect.width + gap;
            if (Widgets.ButtonText(rect, "+" + largeChange, true, false, true))
            {
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
                val += largeChange;
                if (val < min)
                    val = min;
            }
            _this.Gap(_this.verticalSpacing);
        }
    }
}