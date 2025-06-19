namespace VoteApiLibrary.Models;

public class Scrutin
{
    private readonly List<Candidate> _candidates = new();
    public ScrutinStatus Status { get; private set; } = ScrutinStatus.Open;
    public IReadOnlyList<Candidate> Candidates => _candidates.AsReadOnly();
    public int Round { get; private set; } = 1;

    public void AddCandidate(string candidateName)
    {
        if (Status == ScrutinStatus.Closed)
            throw new InvalidOperationException("Cannot add candidates to a closed scrutin");
        
        if (_candidates.Any(c => c.Name == candidateName))
            throw new ArgumentException("Candidate already exists");

        _candidates.Add(new Candidate(candidateName));
    }

    public void AddVotes(string candidateName, int votes)
    {
        if (Status == ScrutinStatus.Closed)
            throw new InvalidOperationException("Cannot add votes to a closed scrutin");

        var candidate = _candidates.FirstOrDefault(c => c.Name == candidateName);
        if (candidate == null)
            throw new ArgumentException("Candidate not found");

        candidate.AddVotes(votes);
    }

    public void Close()
    {
        Status = ScrutinStatus.Closed;
        CalculatePercentages();
    }

    public void StartSecondRound(IEnumerable<Candidate> qualifiedCandidates)
    {
        if (Round >= 2)
            throw new InvalidOperationException("Maximum of 2 rounds allowed");

        Round = 2;
        Status = ScrutinStatus.Open;
        
        _candidates.Clear();
        foreach (var candidate in qualifiedCandidates)
        {
            _candidates.Add(new Candidate(candidate.Name));
        }
    }

    private void CalculatePercentages()
    {
        var totalVotes = _candidates.Sum(c => c.Votes);
        foreach (var candidate in _candidates)
        {
            candidate.CalculatePercentage(totalVotes);
        }
    }
}