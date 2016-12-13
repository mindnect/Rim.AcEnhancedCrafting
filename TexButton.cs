using UnityEngine;
using Verse;

namespace AlcoholV
{
    [StaticConstructorOnStartup]
    internal class TexButton
    {
        // mod icon
        public static readonly Texture2D Detail = ContentFinder<Texture2D>.Get("Detail", true);
        public static readonly Texture2D RepeatCount = ContentFinder<Texture2D>.Get("RepeatCount", true);
        public static readonly Texture2D TargetCount = ContentFinder<Texture2D>.Get("TargetCount", true);
        public static readonly Texture2D Forever = ContentFinder<Texture2D>.Get("Forever", true);
        public static readonly Texture2D DropOnFloor = ContentFinder<Texture2D>.Get("DropOnFloor", true);
        public static readonly Texture2D BestStockpile = ContentFinder<Texture2D>.Get("BestStockpile", true);

        // todo : fix memory usage
        // original icon
        public static readonly Texture2D DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
        public static readonly Texture2D ReorderUp = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp", true);
        public static readonly Texture2D ReorderDown = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown", true);
        public static readonly Texture2D Plus = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
        public static readonly Texture2D Minus = ContentFinder<Texture2D>.Get("UI/Buttons/Minus", true);
        public static readonly Texture2D Suspend = ContentFinder<Texture2D>.Get("UI/Buttons/Suspend", true);
        public static readonly Texture2D Rename = ContentFinder<Texture2D>.Get("UI/Buttons/Rename", true);
    }
}