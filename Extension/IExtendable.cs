using Verse;

namespace AlcoholV.Extension
{
    // It does not fit the class design but tricky way for compatibility.
    public interface IExtendable
    {
        Pawn AssignedPawn { get; set; }
        string Name { get; set; }
        int MinStock { get; set; }
        bool IsPaused { get; set; }
    }
}