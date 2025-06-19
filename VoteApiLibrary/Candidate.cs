// filepath: VoteApiLibrary/Models/Candidate.cs
namespace VoteApiLibrary.Models;

public class Candidate
{
    public string Name { get; }
    public int Votes { get; private set; }
    public double Percentage { get; private set; }

    public Candidate(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void AddVotes(int votes)
    {
        if (votes < 0) throw new ArgumentException("Votes cannot be negative");
        Votes += votes;
    }

    public void CalculatePercentage(int totalVotes)
    {
        Percentage = totalVotes > 0 ? (double)Votes / totalVotes * 100 : 0;
    }
}