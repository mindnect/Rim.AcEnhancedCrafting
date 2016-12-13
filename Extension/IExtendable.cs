using Verse;

namespace AlcoholV.Extension
{
    public interface IExtendable
    {
        Pawn AssignedPawn { get; set; }
        string Name { get; set; }
        int MinCount { get; set; }
        bool IsPaused { get; set; }
    }
}