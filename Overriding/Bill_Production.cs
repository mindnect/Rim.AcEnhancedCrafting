using AlcoholV.Extension;
using UnityEngine;
using Verse;
using Source = RimWorld.Bill_Production;

namespace AlcoholV.Overriding
{
    public class Bill_Production : Source, IExtendable
    {
        private Pawn _assignedPawn;
        private string _name;
        private int _minCount;
        private bool _isPaused;


        public Bill_Production()
        {
        }

        public Bill_Production(RecipeDef recipe) : base(recipe)
        {
            Name = recipe.label;
            MinCount = targetCount;
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set { _isPaused = value; }
        }

        public int MinCount
        {
            get { return _minCount; }
            set { _minCount = value; }
        }

        public Pawn AssignedPawn
        {
            get { return _assignedPawn; }
            set { _assignedPawn = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public override bool ShouldDoNow()
        {
            return this.ShouldDoNowExt();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.LookReference(ref _assignedPawn, "AssignedPawn", false);
            Scribe_Values.LookValue(ref _name, "billName", null);
            Scribe_Values.LookValue(ref _minCount, "minCount", 0);
            Scribe_Values.LookValue(ref _isPaused, "isPaused", false);
        }

        protected override void DrawConfigInterface(Rect baseRect, Color baseColor)
        {
            this.DrawConfigInterfaceExt(baseRect, baseColor);
        }
    }
}