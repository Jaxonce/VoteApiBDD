namespace VoteApiLibrary.Models;

public class ScrutinResult
{
    public Candidate? Winner { get; set; }
    public IReadOnlyList<Candidate> Candidates { get; set; } = new List<Candidate>();
    public List<Candidate> QualifiedCandidates { get; set; } = new List<Candidate>();
    public bool HasWinner => Winner != null;
    public bool RequiresSecondRound { get; set; }
    public bool IsTie { get; set; }
    public int Round { get; set; } = 1;
}