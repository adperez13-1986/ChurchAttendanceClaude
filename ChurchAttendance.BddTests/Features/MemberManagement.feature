Feature: Member Management
  As a church administrator
  I want to manage church members
  So that I can track attendance accurately

  Scenario: Add a new member
    Given the application is running
    When I add a member with name "Juan Dela Cruz" age group "Men" and category "Member"
    Then the member "Juan Dela Cruz" should exist in the system
    And the member should be active

  Scenario: Add a member with first attended date
    Given the application is running
    When I add a member with name "Maria Santos" age group "Women" category "Visitor" and first attended date "2024-01-15"
    Then the member "Maria Santos" should have first attended date "2024-01-15"

  Scenario: Edit a member
    Given the application is running
    And a member "Mark Reyes" exists with age group "YAN" and category "Visitor"
    When I update the member "Mark Reyes" category to "Member"
    Then the member "Mark Reyes" should have category "Member"

  Scenario: Deactivate a member
    Given the application is running
    And a member "Sofia Garcia" exists with age group "CYN" and category "Member"
    When I deactivate the member "Sofia Garcia"
    Then the member "Sofia Garcia" should be inactive
    And the member "Sofia Garcia" should not appear in attendance lists

  Scenario Outline: Add members with different age groups
    Given the application is running
    When I add a member with name "<name>" age group "<ageGroup>" and category "Member"
    Then the member "<name>" should have age group "<ageGroup>"

    Examples:
      | name           | ageGroup |
      | Test Men       | Men      |
      | Test Women     | Women    |
      | Test YAN       | YAN      |
      | Test CYN       | CYN      |
      | Test Children  | Children |
      | Test Infants   | Infants  |
