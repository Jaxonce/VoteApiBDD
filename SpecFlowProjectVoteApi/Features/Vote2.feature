Feature: Additional Scenarios for Edge Cases
Gestion des cas particuliers non couverts dans les critères initiaux

    Background:
        Given I have a scrutin system

    Scenario: Égalité entre 2ème et 3ème candidat au premier tour
        Given I have candidates:
          | Name    |
          | Alice   |
          | Bob     |
          | Charlie |
          | David   |
        And the votes are:
          | Candidate | Votes |
          | Alice     | 40    |
          | Bob       | 25    |
          | Charlie   | 25    |
          | David     | 10    |
        When I close the scrutin
        And I calculate the result
        Then there should be no winner
        And a second round should be required
        And the qualified candidates should be "Alice", "Bob" and "Charlie"

    Scenario: Gestion du vote blanc
        Given I have candidates:
          | Name       |
          | Alice      |
          | Bob        |
          | Vote Blanc |
        And the votes are:
          | Candidate  | Votes |
          | Alice      | 30    |
          | Bob        | 25    |
          | Vote Blanc | 45    |
        When I close the scrutin
        And I calculate the result
        Then there should be no winner
        And a second round should be required
        And the qualified candidates should be "Alice" and "Bob"

    Scenario: Vote blanc majoritaire au deuxième tour - candidat valide gagne
        Given I have a second round with candidates:
          | Name       |
          | Alice      |
          | Bob        |
          | Vote Blanc |
        And the votes are:
          | Candidate  | Votes |
          | Alice      | 40    |
          | Bob        | 25    |
          | Vote Blanc | 60    |
        When I close the scrutin
        And I calculate the result
        Then the winner should be "Alice"

    Scenario: Égalité entre tous les candidats au 2ème tour
    Given I have a second round with candidates:
        | Name    |
        | Alice   |
        | Bob     |
        | Charlie |
    And the votes are:
        | Candidate | Votes |
        | Alice     | 30    |
        | Bob       | 30    |
        | Charlie   | 30    |
    When I close the scrutin
    And I calculate the result
    Then there should be no winner
    And there should be a tie