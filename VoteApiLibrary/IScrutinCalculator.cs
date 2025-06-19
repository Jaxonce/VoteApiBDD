using VoteApiLibrary.Models;

namespace VoteApiLibrary.Services;

public interface IScrutinCalculator
{
    ScrutinResult CalculateResult(Scrutin scrutin);
}