using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using VoteApiLibrary.Models;
using VoteApiLibrary.Services;

namespace VoteApiLibrary.Tests.StepDefinitions;

[Binding]
public class VoteStepDefinition
{
    private Scrutin _scrutin = null!;
    private ScrutinResult? _result;
    private IScrutinCalculator _calculator = null!;
    private Exception? _exception;

    [Given(@"I have a scrutin system")]
    public void GivenIHaveAScrutinSystem()
    {
        _scrutin = new Scrutin();
        _calculator = new ScrutinCalculator();
    }

    [Given(@"I have candidates:")]
    public void GivenIHaveCandidates(Table table)
    {
        foreach (var row in table.Rows)
        {
            _scrutin.AddCandidate(row["Name"]);
        }
    }

    [Given(@"I have a second round with candidates:")]
    public void GivenIHaveASecondRoundWithCandidates(Table table)
    {
        var candidates = table.CreateSet<CandidateData>()
            .Select(c => new Candidate(c.Name))
            .ToList();
        
        _scrutin.StartSecondRound(candidates);
    }

    [Given(@"the votes are:")]
    public void GivenTheVotesAre(Table table)
    {
        foreach (var row in table.Rows)
        {
            var candidateName = row["Candidate"];
            var votes = int.Parse(row["Votes"]);
            _scrutin.AddVotes(candidateName, votes);
        }
    }

    [When(@"I close the scrutin")]
    public void WhenICloseTheScrutin()
    {
        _scrutin.Close();
    }

    [When(@"I calculate the result")]
    public void WhenICalculateTheResult()
    {
        _result = _calculator.CalculateResult(_scrutin);
    }

    [When(@"I try to calculate the result without closing")]
    public void WhenITryToCalculateTheResultWithoutClosing()
    {
        try
        {
            _result = _calculator.CalculateResult(_scrutin);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"the winner should be ""(.*)""")]
    public void ThenTheWinnerShouldBe(string expectedWinner)
    {
        _result.Should().NotBeNull();
        _result!.HasWinner.Should().BeTrue();
        _result.Winner!.Name.Should().Be(expectedWinner);
    }

    [Then(@"there should be no winner")]
    public void ThenThereShouldBeNoWinner()
    {
        _result.Should().NotBeNull();
        _result!.HasWinner.Should().BeFalse();
    }

    [Then(@"a second round should be required")]
    public void ThenASecondRoundShouldBeRequired()
    {
        _result.Should().NotBeNull();
        _result!.RequiresSecondRound.Should().BeTrue();
    }

    [Then(@"there should be a tie")]
    public void ThenThereShouldBeATie()
    {
        _result.Should().NotBeNull();
        _result!.IsTie.Should().BeTrue();
    }

    [Then(@"the qualified candidates should be ""(.*)"" and ""(.*)""")]
    public void ThenTheQualifiedCandidatesShouldBeAnd(string candidate1, string candidate2)
    {
        _result.Should().NotBeNull();
        var topTwoCandidates = _result!.Candidates.Take(2).ToList();
        topTwoCandidates.Should().HaveCount(2);
        topTwoCandidates.Select(c => c.Name).Should().Contain(new[] { candidate1, candidate2 });
    }

    [Then(@"the candidate ""(.*)"" should have (.*)% of votes")]
    public void ThenTheCandidateShouldHaveOfVotes(string candidateName, double expectedPercentage)
    {
        _result.Should().NotBeNull();
        var candidate = _result!.Candidates.FirstOrDefault(c => c.Name == candidateName);
        candidate.Should().NotBeNull();
        candidate!.Percentage.Should().BeApproximately(expectedPercentage, 0.1);
    }

    [Then(@"I should get an error ""(.*)""")]
    public void ThenIShouldGetAnError(string expectedMessage)
    {
        _exception.Should().NotBeNull();
        _exception!.Message.Should().Be(expectedMessage);
    }

    public class CandidateData
    {
        public string Name { get; set; } = string.Empty;
    }
}