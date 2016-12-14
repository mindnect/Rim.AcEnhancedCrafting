using System.Reflection;
using AlcoholV.Extension;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Source = RimWorld.Bill;

namespace AlcoholV.Detouring
{
    public abstract class Bill : Source
    {
        [Detour(typeof(Source), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public override bool PawnAllowedToStartAnew(Pawn p)
        {
            var iBill = this as IExtendable;
            if (iBill?.AssignedPawn != null)
                if (p != iBill.AssignedPawn) return false;
            if (recipe.workSkill != null)
            {
                var level = p.skills.GetSkill(recipe.workSkill).Level;
                if ((level < allowedSkillRange.min) || (level > allowedSkillRange.max))
                    return false;
            }
            return true;
        }

        [Detour(typeof(Source), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public new Rect DoInterface(float x, float y, float width, int index)
        {
            var rect = new Rect(x, y, width, 53f);
            if (!StatusString.NullOrEmpty())
                rect.height += 17f;
            var white = Color.white;
            if (!ShouldDoNow())
                white = new Color(1f, 0.7f, 0.7f, 0.7f);
            GUI.color = white;
            Text.Font = GameFont.Small;
            if (index%2 == 0)
                Widgets.DrawAltRect(rect);
            GUI.BeginGroup(rect);
            var butRect = new Rect(0f, 0f, 24f, 24f);
            if ((billStack.IndexOf(this) > 0) && Widgets.ButtonImage(butRect, TexButton.ReorderUp, white))
            {
                billStack.Reorder(this, -1);
                SoundDefOf.TickHigh.PlayOneShotOnCamera();
            }
            if (billStack.IndexOf(this) < billStack.Count - 1)
            {
                var butRect2 = new Rect(0f, 24f, 24f, 24f);
                if (Widgets.ButtonImage(butRect2, TexButton.ReorderDown, white))
                {
                    billStack.Reorder(this, 1);
                    SoundDefOf.TickLow.PlayOneShotOnCamera();
                }
            }
            var rect2 = new Rect(28f, 0f, rect.width - 48f - 20f, 48f);

            // change bill name
            var iBill = this as IExtendable;
            Widgets.Label(rect2, iBill != null ? iBill.Name.Truncate(rect.width - 180f) : LabelCap);

            DoConfigInterface(rect.AtZero(), white);
            var rect3 = new Rect(rect.width - 24f, 0f, 24f, 24f);
            if (Widgets.ButtonImage(rect3, TexButton.DeleteX, white))
                billStack.Delete(this);
            var butRect3 = new Rect(rect3);
            butRect3.x -= butRect3.width + 4f;
            if (Widgets.ButtonImage(butRect3, TexButton.Suspend, white))
                suspended = !suspended;
            if (!StatusString.NullOrEmpty())
            {
                Text.Font = GameFont.Tiny;
                var rect4 = new Rect(24f, rect.height - 17f, rect.width - 24f, 17f);
                Widgets.Label(rect4, StatusString);
            }
            GUI.EndGroup();
            if (suspended)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                var rect5 = new Rect(rect.x + rect.width/2f - 70f, rect.y + rect.height/2f - 20f, 140f, 40f);
                GUI.DrawTexture(rect5, TexUI.GrayTextBG);
                Widgets.Label(rect5, "SuspendedCaps".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            return rect;
        }
    }
}