Feature: Reporting
  As a church administrator
  I want to generate attendance reports
  So that I can analyze church attendance patterns

  Scenario: Generate PDF report for date range
    Given the application is running
    And attendance data exists for the last 7 days
    When I generate a report from 7 days ago to today
    Then a PDF report should be generated
    And the report should contain attendance data

  Scenario: Report includes age group breakdown
    Given the application is running
    And the following members exist:
      | name          | ageGroup | category |
      | Juan Dela Cruz| Men      | Member   |
      | Maria Santos  | Women    | Member   |
      | Young Person  | YAN      | Visitor  |
    And attendance is recorded for today with all members present
    When I generate a report for today
    Then the report should show age group counts

  Scenario: Report highlights first timers
    Given the application is running
    And a member "First Timer" exists with age group "Men" category "Visitor" and first attended date of today
    And attendance is recorded for today with "First Timer" present
    When I generate a report for today
    Then "First Timer" should be highlighted in the report
