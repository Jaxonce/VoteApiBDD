Feature: Scrutin Calculation
    En tant que client de l'API à la clôture d'un scrutin majoritaire
    Je veux calculer le résultat du scrutin
    Pour obtenir le vainqueur du vote

Background:
    Given I have a scrutin system

Scenario: Candidat avec plus de 50% des voix remporte le scrutin
    Given I have candidates:
        | Name    |
        | Alice   |
        | Bob     |
        | Charlie |
    And the votes are:
        | Candidate | Votes |
        | Alice     | 60    |
        | Bob       | 30    |
        | Charlie   | 10    |
    When I close the scrutin
    And I calculate the result
    Then the winner should be "Alice"
    And the candidate "Alice" should have 60.0% of votes
    And the candidate "Bob" should have 30.0% of votes
    And the candidate "Charlie" should have 10.0% of votes

Scenario: Aucun candidat n'a plus de 50% - deuxième tour requis
    Given I have candidates:
        | Name    |
        | Alice   |
        | Bob     |
        | Charlie |
    And the votes are:
        | Candidate | Votes |
        | Alice     | 45    |
        | Bob       | 35    |
        | Charlie   | 20    |
    When I close the scrutin
    And I calculate the result
    Then there should be no winner
    And a second round should be required
    And the qualified candidates should be "Alice" and "Bob"

Scenario: Victoire au deuxième tour
    Given I have a second round with candidates:
        | Name  |
        | Alice |
        | Bob   |
    And the votes are:
        | Candidate | Votes |
        | Alice     | 55    |
        | Bob       | 45    |
    When I close the scrutin
    And I calculate the result
    Then the winner should be "Alice"
    And the candidate "Alice" should have 55.0% of votes

Scenario: Égalité au deuxième tour - pas de vainqueur
    Given I have a second round with candidates:
        | Name  |
        | Alice |
        | Bob   |
    And the votes are:
        | Candidate | Votes |
        | Alice     | 50    |
        | Bob       | 50    |
    When I close the scrutin
    And I calculate the result
    Then there should be no winner
    And there should be a tie

Scenario: Scrutin non clôturé - impossible de calculer le résultat
    Given I have candidates:
        | Name  |
        | Alice |
        | Bob   |
    And the votes are:
        | Candidate | Votes |
        | Alice     | 60    |
        | Bob       | 40    |
    When I try to calculate the result without closing
    Then I should get an error "Scrutin must be closed to calculate result"
