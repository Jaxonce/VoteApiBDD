using VoteApiLibrary.Models;

namespace VoteApiLibrary.Services;

public class ScrutinCalculator : IScrutinCalculator
{
    public ScrutinResult CalculateResult(Scrutin scrutin)
    {
        if (scrutin.Status != ScrutinStatus.Closed)
            throw new InvalidOperationException("Scrutin must be closed to calculate result");

        var result = new ScrutinResult
        {
            Candidates = scrutin.Candidates.OrderByDescending(c => c.Percentage).ToList(),
            Round = scrutin.Round
        };

        if (!scrutin.Candidates.Any() || scrutin.Candidates.All(c => c.Votes == 0))
        {
            return result;
        }

        var topCandidate = result.Candidates.First();

        // Cas du dernier tour (tour 2)
        if (scrutin.Round == 2)
        {
            var topCandidates = result.Candidates.Where(c => c.Percentage == topCandidate.Percentage).ToList();
            if (topCandidates.Count > 1)
            {
                result.IsTie = true;
            }
            else
            {
                result.Winner = topCandidate;
            }
            return result;
        }

        // Premier tour : vérifier si quelqu'un a > 50%
        if (topCandidate.Percentage > 50)
        {
            result.Winner = topCandidate;
        }
        else
        {
            // Deuxième tour nécessaire avec les 2 meilleurs candidats
            result.RequiresSecondRound = true;
        }

        return result;
    }
}