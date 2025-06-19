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

        // Séparer les candidats valides du vote blanc
        var validCandidates = scrutin.GetValidCandidates().OrderByDescending(c => c.Percentage).ToList();
        var voteBlancCandidate = scrutin.GetVoteBlancCandidate();

        if (!validCandidates.Any() || validCandidates.All(c => c.Votes == 0))
        {
            return result;
        }

        var topCandidate = validCandidates.First();

        // Cas du dernier tour (tour 2)
        if (scrutin.Round == 2)
        {
            var topCandidates = validCandidates.Where(c => c.Percentage == topCandidate.Percentage).ToList();
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

        // Premier tour : vérifier si quelqu'un a > 50% (en excluant le vote blanc)
        var totalValidVotes = validCandidates.Sum(c => c.Votes);
        var topCandidatePercentageWithoutBlank = totalValidVotes > 0 ? (double)topCandidate.Votes / totalValidVotes * 100 : 0;

        if (topCandidatePercentageWithoutBlank > 50)
        {
            result.Winner = topCandidate;
        }
        else
        {
            // Deuxième tour nécessaire - déterminer les candidats qualifiés
            result.RequiresSecondRound = true;
            result.QualifiedCandidates = GetQualifiedCandidatesForSecondRound(validCandidates);
        }

        return result;
    }

    private List<Candidate> GetQualifiedCandidatesForSecondRound(List<Candidate> validCandidates)
    {
        if (validCandidates.Count <= 2)
        {
            return validCandidates.ToList();
        }

        var topCandidate = validCandidates[0];
        var secondCandidate = validCandidates[1];
        
        // Vérifier s'il y a égalité entre 2ème et 3ème
        var candidatesWithSamePercentageAsSecond = validCandidates
            .Where(c => Math.Abs(c.Percentage - secondCandidate.Percentage) < 0.01)
            .ToList();

        if (candidatesWithSamePercentageAsSecond.Count > 1)
        {
            // Égalité : on prend le premier + tous ceux qui ont le même pourcentage que le 2ème
            var qualified = new List<Candidate> { topCandidate };
            qualified.AddRange(candidatesWithSamePercentageAsSecond);
            return qualified.Distinct().ToList();
        }

        // Pas d'égalité : on prend les 2 premiers
        return new List<Candidate> { topCandidate, secondCandidate };
    }
}