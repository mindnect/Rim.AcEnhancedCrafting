using AlcoholV.Extension;
using UnityEngine;
using Verse;
using Source = RimWorld.Bill_ProductionWithUft;


namespace AlcoholV.Overriding
{
    public class Bill_ProductionWithUft : Source, IExtendable
    {
        private Pawn _assignedPawn;
        private string _name;
        private int _minCount;
        private bool _isPaused;

        public Bill_ProductionWithUft()
        {
        }

        public Bill_ProductionWithUft(RecipeDef recipe) : base(recipe)
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

        protected override void DoConfigInterface(Rect baseRect, Color baseColor)
        {
            this.DoConfigInterfaceExt(baseRect, baseColor);
        }
    }
}