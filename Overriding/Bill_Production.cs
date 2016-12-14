using AlcoholV.Extension;
using UnityEngine;
using Verse;
using Source = RimWorld.Bill_Production;

namespace AlcoholV.Overriding
{
    public class Bill_Production : Source, IExtendable
    {
        private Pawn _assignedPawn;
        private string _billName;
        private int _minStock;
        private bool _isPaused;

        public Bill_Production()
        {
        }

        public Bill_Production(RecipeDef recipe) : base(recipe)
        {
            Name = recipe.label;
            MinStock = targetCount;
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set { _isPaused = value; }
        }

        public int MinStock
        {
            get { return _minStock; }
            set { _minStock = value; }
        }

        public Pawn AssignedPawn
        {
            get { return _assignedPawn; }
            set { _assignedPawn = value; }
        }

        public string Name
        {
            get { return _billName; }
            set { _billName = value; }
        }

        public override bool ShouldDoNow()
        {
            return this.ShouldDoNowExt();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.LookReference(ref _assignedPawn, "assignedPawn", false);
            Scribe_Values.LookValue(ref _billName, "billName", null);
            Scribe_Values.LookValue(ref _minStock, "minStock", 0);
            Scribe_Values.LookValue(ref _isPaused, "isPaused", false);
        }

        protected override void DoConfigInterface(Rect baseRect, Color baseColor)
        {
            this.DoConfigInterfaceExt(baseRect, baseColor);
        }
    }
}