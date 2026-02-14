Feature: Attendance Tracking
  As a church administrator
  I want to record attendance for services
  So that I can monitor church growth

  Scenario: Record attendance for Sunday service
    Given the application is running
    And the following members exist:
      | name          | ageGroup | category |
      | Juan Dela Cruz| Men      | Member   |
      | Maria Santos  | Women    | Member   |
    When I record attendance for today with service type "SundayService" for members:
      | name           |
      | Juan Dela Cruz |
      | Maria Santos   |
    Then the attendance count for today should be 2

  Scenario: Record attendance for prayer meeting
    Given the application is running
    And a member "Juan Dela Cruz" exists with age group "Men" and category "Member"
    When I record attendance for today with service type "PrayerMeeting" for members:
      | name           |
      | Juan Dela Cruz |
    Then the attendance for today with service type "PrayerMeeting" should have 1 member

  Scenario: Detect first timer
    Given the application is running
    And a member "New Person" exists with age group "Men" category "Visitor" and first attended date of today
    When I load the attendance list for today
    Then "New Person" should be marked as a first timer

  Scenario: Attendance upsert replaces existing record
    Given the application is running
    And a member "Juan Dela Cruz" exists with age group "Men" and category "Member"
    And a member "Maria Santos" exists with age group "Women" and category "Member"
    And attendance is recorded for today with service type "SundayService" for "Juan Dela Cruz"
    When I record attendance for today with service type "SundayService" for members:
      | name         |
      | Maria Santos |
    Then the attendance for today with service type "SundayService" should have 1 member

  Scenario: Only active members appear in attendance list
    Given the application is running
    And a member "Active Person" exists with age group "Men" and category "Member"
    And a member "Inactive Person" exists with age group "Women" and category "Member"
    And the member "Inactive Person" is deactivated
    When I load the attendance list for today
    Then "Active Person" should appear in the attendance list
    And "Inactive Person" should not appear in the attendance list
